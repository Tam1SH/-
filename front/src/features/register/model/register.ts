import type { UseQueryReturnType } from '@tanstack/vue-query'
import { type ApiError } from '@/shared/api/g/api'
import { useQueryAPI } from '@/shared/api/hooks'
import { useUserStore } from '@/app/userStore'


export type JwtToken = string

export type BaseRegisterParams = {
	registerHook: UseQueryReturnType<any, ApiError>

}

export async function baseLogin({
	registerHook,
}: BaseRegisterParams) {
	
	const result = await registerHook.refetch()
	
	// if (result.data) {
	// 	setTokenFor.user?.(result.data.token!)
	// }
	// else {
	// 	throw new Error(`Could not login, error: ${result.error?.message ?? result.error}`)
	// }
}

export type UseRegisterForParamsFunc = () => ({
	login: string, 
	password: string
})

export function useRegisterForUser(func: UseRegisterForParamsFunc) {	

	
	const Auth = useQueryAPI({ enabled: false }).register(func)

	return {
		hook: Auth
	}
}