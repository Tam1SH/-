import process from 'process'
import { generateResultCode, moveFiles, writeToFile } from './generateCode'
import * as path from 'node:path'

const args = process.argv.slice(2)

if (args.includes('--help')) {
    console.log(`
    This script generates a wrapper for rtk-query over an API that is generated using openapi-generator.
    Usage: npm index.ts [react|vue]
    `)
    process.exit(0)
}

if (args.length === 0) {
    console.error('Please provide "react" or "vue" as an argument')
    process.exit(1)
}

const arg = args[0]

if (arg !== 'react' && arg !== 'vue') {
    console.error(`Unknown argument: ${arg}. Please provide "react" or "vue"`)
    process.exit(1)
} 

const formattedCode = await generateResultCode()
const filePath = path.join(__dirname, '../src/shared/api/QueryApi.ts')


await writeToFile(filePath, formattedCode)
await moveFiles(arg)