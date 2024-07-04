import * as ts from "typescript"
import fs from 'node:fs'
import * as path from 'node:path'
import fsExtra from 'fs-extra'

export type ClassDecl = {
	name: string, 
	members: FunctionDecl[]
}

export type FunctionDecl = {
	name: string,
	parameters: { name: string, type: ts.TypeNode }[],
	returnType: ts.TypeNode
}
	
export function getClasses() {
	const classes: ClassDecl[] = []

	const files = fs.readdirSync('./GenApi/dist/ts/api/apis/').filter(file => file.endsWith('.ts'))

	files.forEach(file => {
		const sourceFile = ts.createSourceFile(
			file,
			fs.readFileSync(path.join('./GenApi/dist/ts/api/apis/', file)).toString(),
			ts.ScriptTarget.ES2015,
			true
		)

		function collectClassesWithMethodsDeclarations(node: ts.Node) {
			if (ts.isClassDeclaration(node) && node.name) {
				const className = node.name.text
				const members = node.members
					.filter(member => ts.isMethodDeclaration(member))
					.map(member => {
						if (ts.isMethodDeclaration(member) && member.name) {
							const parameters = member.parameters
								.slice(0, -1)
								.map(param => ({
									name: param.name.getText(),
									type: param.type ? param.type : ts.factory.createKeywordTypeNode(ts.SyntaxKind.AnyKeyword)
								}))
							
							//Promise<T> => T
							let returnType = member.type ? member.type : ts.factory.createKeywordTypeNode(ts.SyntaxKind.AnyKeyword)

							if (ts.isTypeReferenceNode(returnType) && returnType.typeName.getText() === 'Promise' && returnType.typeArguments) {
								returnType = returnType.typeArguments[0]
							}

							return { name: member.name.getText(), parameters, returnType }
						}
					})
					.filter(member => member !== undefined)
					.filter(member => !member!.name.endsWith('Raw')) as FunctionDecl[]
		
				classes.push({ name: className, members })
			}
			ts.forEachChild(node, collectClassesWithMethodsDeclarations)
		}


		collectClassesWithMethodsDeclarations(sourceFile)
	})

	return classes
}

export function createQueryApi(classes: ClassDecl[]) {

	const methods = classes.flatMap(classDecl =>
		classDecl.members.map(member => {
			const argsType = member.parameters.length === 0 
				? `() => undefined = () => {}` 
				: member.parameters.length === 1 
					? (() => {
						const type = member.parameters[0].type.getFullText()
						return `(() =>${type}) |${type}`
					})()
					: (() => {
						const params = `[${member.parameters.map(p => `${p.name}:${p.type.getFullText()}`).join(', ')}]`
						return `(() => ${params}) |${params}`
					})()
			const args = `args: ${argsType}`

			const body = `return this._useQuery(
			() => this.options.apiFactory.create(${classDecl.name}),
			${classDecl.name}.prototype.${member.name},
			args
		)`

			return `
	${member.name}(${args}): UseQueryReturnWrapperType<${member.returnType.getFullText()}> {
		${body}
	}`
		})
	)

	const classCode = `
export default class QueryApi<
	TResult, 
	TQueryHooksAPIBuilderOptions = ApiUseQueryOptions<TResult>
> extends BaseQueryApi<TResult, TQueryHooksAPIBuilderOptions> 
{
	${methods.map(method => `
			${method}
`).join('')}
}
`

	return classCode
}


export async function generateResultCode() {

    const classes = getClasses()

    const primitiveTypesAndDefaultTypes = ["any", "unknown", "boolean", "number", "string", "symbol", "void", "null", "undefined", "Date", "Blob"]

	const types = new Set(classes.flatMap(classDecl =>
		classDecl.members.flatMap(member => {
			return [
				...member.parameters.map(param => param.type.getText().replace(/Array<(.*)>/, '$1').replace(/ \| null/g, '')),
				member.returnType.getText().replace(/Array<(.*)>/, '$1').replace(/ \| null/g, ''),
			]
		})
	).filter(type => !primitiveTypesAndDefaultTypes.includes(type)))

    const classNames = classes.map(classDecl => classDecl.name).join(', ')
	const typeNames = Array.from(types).length > 0 ? `, ${Array.from(types).map(type => 'type ' + type).join(', ')}` : ''

    const code = createQueryApi(classes)
    const codeWithImports = `
/* tslint:disable */
/* eslint-disable */

import BaseQueryApi, { type ApiUseQueryOptions, type UseQueryReturnWrapperType } from "./BaseQueryApi"
import { ${classNames}${typeNames} } from "./g/api/index"

${code}
    `
    // const formattedCode = await prettier.format(codeWithImports, { parser: "typescript", useTabs: true })

    return codeWithImports
}


export async function writeToFile(filePath: string, formattedCode: string) {
    try {
        await fs.promises.writeFile(filePath, formattedCode)
        console.log(`Saved file: ${filePath}`)
    } catch (err) {
        console.error("Error on write:", err)
    }
}

async function generateBaseQueryApiTemplate(type: Type) {
	
    const template = await fs.promises.readFile(path.join(__dirname, './templates/BaseQueryApi.template'), 'utf-8')

    let importPath = ''
	
    if (type === 'react') {
        importPath = "'@tanstack/vue-query'"
    } else if (type === 'vue') {
        importPath = "'@tanstack/vue-query'"
    }

    const result = template.replace('#importPath', importPath)

    return result
}


export type Type = 'react' | 'vue'

export async function moveFiles(type: Type) {
    try {
	
        await fsExtra.copy('./GenApi/dist/ts', './src/shared/api/g', { overwrite: true })
        await fsExtra.copy('./api_generator/templates/ApiFactory.template', './src/shared/api/ApiFactory.ts', { overwrite: true })

		
        const baseQueryApiContent = await generateBaseQueryApiTemplate(type)

        await fs.promises.writeFile(path.join(__dirname, '../src/shared/api/BaseQueryApi.ts'), baseQueryApiContent)

        console.log('Successfully moved files.')
    } catch (err) {
        console.error('Error on moving the folder:', err)
    }
}