import { useUserStore } from "@/app/userStore"
import { ApiFactory, createFactory } from "./ApiFactory"
import type { ApiUseQueryOptions, BaseQueryApiOptionsWithoutFactory } from "./BaseQueryApi"
import QueryApi from "./QueryApi"
import { AuthenticateApi } from "./g/api"


export function useQueryAPI<
	TResult, 
	TOptions = ApiUseQueryOptions<TResult>
>(options: BaseQueryApiOptionsWithoutFactory<TOptions> = {}): QueryApi<TResult, TOptions> {
	
	return !options.apiFactory 
		? new QueryApi({ ...options, apiFactory: useFactoryApi() })
		: new QueryApi({ ...options, apiFactory: options.apiFactory })
}

export function useFactoryApi() {

	const userStore = useUserStore()
	
	return createFactory({
		accessToken: () => userStore.token ?? '',
		refreshAccessToken: async () => {
			const factory = new ApiFactory({ accessToken: userStore.token ?? '' })
			const api = factory.create(AuthenticateApi)
			const { accessToken } = await api.refreshToken()

			if (accessToken) {
				userStore.setToken(accessToken)
			}
			
			return accessToken ?? ''
		}
	})
}