import type { UseQueryReturnType } from '@tanstack/vue-query'
import { type ApiError } from '@/shared/api/g/api'
import { useQueryAPI } from '@/shared/api/hooks'
import type { LoginResponse } from '@/shared/api/g/api/models/LoginResponse'
import { useUserStore } from '@/app/userStore'


export type JwtToken = string

export type BaseLoginParams = {
	loginHook: UseQueryReturnType<LoginResponse, ApiError>
	setTokenFor: {
		user?: (token: JwtToken) => void,
	}
}

export async function baseLogin({
	loginHook,
	setTokenFor
}: BaseLoginParams) {
	
	const result = await loginHook.refetch()
	
	if (result.data) {
		setTokenFor.user?.(result.data.token!)
	}
	else {
		throw new Error(`Could not login, error: ${result.error?.message ?? result.error}`)
	}
}

export type UseLoginForUserParamsFunc = () => ({
	login: string, 
	password: string
})

export function useLoginForUser(func: UseLoginForUserParamsFunc) {	

	const store = useUserStore()

	const Auth = useQueryAPI({ enabled: false }).login(func)

	return {
		hook: Auth,
		fn: () => baseLogin({
			loginHook: Auth.query,
			setTokenFor: {
				user: (token) => store.setToken(token)
			}
		})
	}
}