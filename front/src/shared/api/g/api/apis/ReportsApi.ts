/* tslint:disable */
/* eslint-disable */
/**
 * back, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


import * as runtime from '../runtime';
import type {
  GetTransactionHistoryModel,
  TransactionHistoryResponse,
} from '../models/index';
import {
    GetTransactionHistoryModelFromJSON,
    GetTransactionHistoryModelToJSON,
    TransactionHistoryResponseFromJSON,
    TransactionHistoryResponseToJSON,
} from '../models/index';

export interface GetTransactionHistoryRequest {
    getTransactionHistoryModel?: GetTransactionHistoryModel;
}

/**
 * 
 */
export class ReportsApi extends runtime.BaseAPI {

    /**
     */
    async getTransactionHistoryRaw(requestParameters: GetTransactionHistoryRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<Array<TransactionHistoryResponse>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.accessToken) {
            const token = this.configuration.accessToken;
            const tokenString = await token("Bearer", []);

            if (tokenString) {
                headerParameters["Authorization"] = `Bearer ${tokenString}`;
            }
        }
        const response = await this.request({
            path: `/api/Reports/transaction-history`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: GetTransactionHistoryModelToJSON(requestParameters['getTransactionHistoryModel']),
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(TransactionHistoryResponseFromJSON));
    }

    /**
     */
    async getTransactionHistory(getTransactionHistoryModel?: GetTransactionHistoryModel, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<Array<TransactionHistoryResponse>> {
        const response = await this.getTransactionHistoryRaw({ getTransactionHistoryModel: getTransactionHistoryModel }, initOverrides);
        return await response.value();
    }

}