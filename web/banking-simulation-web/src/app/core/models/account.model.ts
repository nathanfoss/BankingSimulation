import { AccountTypeEnum } from "../enums/accountType.enum";
import { AccountHolder } from "./accountHolder.model";
import { AccountType } from "./accountType.model";

export interface Account {
    id: string;
    accountHolderId: string;
    accountHolder: AccountHolder;
    accountTypeId: AccountTypeEnum;
    accountType: AccountType;
    linkedAccountId: string;
    linkedAccount: Account;
    balance: number;
}