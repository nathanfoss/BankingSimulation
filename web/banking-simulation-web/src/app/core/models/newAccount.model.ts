import { AccountTypeEnum } from "../enums/accountType.enum";

export interface NewAccountModel {
    accountHolderName: string;
    accountHolderPublicIdentifier: string;
    accountTypeId: AccountTypeEnum;
    linkedAccountId: string;
}