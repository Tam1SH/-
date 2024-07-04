
/* tslint:disable */
/* eslint-disable */

import BaseQueryApi, { type ApiUseQueryOptions, type UseQueryReturnWrapperType } from "./BaseQueryApi"
import { PersonalAccountManagerApi, AuthenticateApi, ReportsApi, type ConvertCurrencyModel, type CreateAccountModel, type PersonalAccount, type TransferFundsModel, type LoginModel, type RefreshTokenResponse, type RegisterModel, type GetTransactionHistoryModel, type TransactionHistoryResponse } from "./g/api/index"


export default class QueryApi<
	TResult, 
	TQueryHooksAPIBuilderOptions = ApiUseQueryOptions<TResult>
> extends BaseQueryApi<TResult, TQueryHooksAPIBuilderOptions> 
{
	
			
	convertCurrency(args: (() => ConvertCurrencyModel) | ConvertCurrencyModel): UseQueryReturnWrapperType<number> {
		return this._useQuery(
			() => this.options.apiFactory.create(PersonalAccountManagerApi),
			PersonalAccountManagerApi.prototype.convertCurrency,
			args
		)
	}

			
	createPersonalAccount(args: (() => CreateAccountModel) | CreateAccountModel): UseQueryReturnWrapperType<any> {
		return this._useQuery(
			() => this.options.apiFactory.create(PersonalAccountManagerApi),
			PersonalAccountManagerApi.prototype.createPersonalAccount,
			args
		)
	}

			
	getPersonalAccounts(args: (() => number) | number): UseQueryReturnWrapperType<Array<PersonalAccount>> {
		return this._useQuery(
			() => this.options.apiFactory.create(PersonalAccountManagerApi),
			PersonalAccountManagerApi.prototype.getPersonalAccounts,
			args
		)
	}

			
	transferFunds(args: (() => TransferFundsModel) | TransferFundsModel): UseQueryReturnWrapperType<any> {
		return this._useQuery(
			() => this.options.apiFactory.create(PersonalAccountManagerApi),
			PersonalAccountManagerApi.prototype.transferFunds,
			args
		)
	}

			
	login(args: (() => LoginModel) | LoginModel): UseQueryReturnWrapperType<any> {
		return this._useQuery(
			() => this.options.apiFactory.create(AuthenticateApi),
			AuthenticateApi.prototype.login,
			args
		)
	}

			
	logout(args: () => undefined = () => {}): UseQueryReturnWrapperType<any> {
		return this._useQuery(
			() => this.options.apiFactory.create(AuthenticateApi),
			AuthenticateApi.prototype.logout,
			args
		)
	}

			
	refreshToken(args: () => undefined = () => {}): UseQueryReturnWrapperType<RefreshTokenResponse> {
		return this._useQuery(
			() => this.options.apiFactory.create(AuthenticateApi),
			AuthenticateApi.prototype.refreshToken,
			args
		)
	}

			
	register(args: (() => RegisterModel) | RegisterModel): UseQueryReturnWrapperType<any> {
		return this._useQuery(
			() => this.options.apiFactory.create(AuthenticateApi),
			AuthenticateApi.prototype.register,
			args
		)
	}

			
	getTransactionHistory(args: (() => GetTransactionHistoryModel) | GetTransactionHistoryModel): UseQueryReturnWrapperType<Array<TransactionHistoryResponse>> {
		return this._useQuery(
			() => this.options.apiFactory.create(ReportsApi),
			ReportsApi.prototype.getTransactionHistory,
			args
		)
	}

}

    