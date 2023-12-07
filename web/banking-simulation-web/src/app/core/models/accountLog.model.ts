
import { AccountEventTypeEnum } from "../enums/accountEventType.enum";
import { Account } from "./account.model";
import { AccountEventType } from "./accountEventType.model";

export interface AccountLog {
    id: string;
    accountId: string;
    account: Account;
    eventTypeId: AccountEventTypeEnum;
    eventType: AccountEventType;
    createdDate: Date;
    metadata: any;
}