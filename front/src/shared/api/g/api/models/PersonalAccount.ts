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

import { mapValues } from '../runtime';
/**
 * 
 * @export
 * @interface PersonalAccount
 */
export interface PersonalAccount {
    /**
     * 
     * @type {number}
     * @memberof PersonalAccount
     */
    id?: number;
    /**
     * 
     * @type {number}
     * @memberof PersonalAccount
     */
    userId?: number;
    /**
     * 
     * @type {number}
     * @memberof PersonalAccount
     */
    amount?: number;
    /**
     * 
     * @type {string}
     * @memberof PersonalAccount
     */
    currency?: string;
}

/**
 * Check if a given object implements the PersonalAccount interface.
 */
export function instanceOfPersonalAccount(value: object): value is PersonalAccount {
    return true;
}

export function PersonalAccountFromJSON(json: any): PersonalAccount {
    return PersonalAccountFromJSONTyped(json, false);
}

export function PersonalAccountFromJSONTyped(json: any, ignoreDiscriminator: boolean): PersonalAccount {
    if (json == null) {
        return json;
    }
    return {
        
        'id': json['id'] == null ? undefined : json['id'],
        'userId': json['userId'] == null ? undefined : json['userId'],
        'amount': json['amount'] == null ? undefined : json['amount'],
        'currency': json['currency'] == null ? undefined : json['currency'],
    };
}

export function PersonalAccountToJSON(value?: PersonalAccount | null): any {
    if (value == null) {
        return value;
    }
    return {
        
        'id': value['id'],
        'userId': value['userId'],
        'amount': value['amount'],
        'currency': value['currency'],
    };
}

