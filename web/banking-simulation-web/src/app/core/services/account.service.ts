import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { Observable, BehaviorSubject } from "rxjs";
import { tap } from 'rxjs/operators';

import { HttpService } from "./http.service";
import { Account } from "../models/account.model";
import { NewAccountModel } from "../models/newAccount.model";

@Injectable({ providedIn: 'root' })
export class AccountService extends HttpService {
    private reloadBs: BehaviorSubject<boolean> = new BehaviorSubject(true);

    reload$: Observable<boolean> = this.reloadBs.asObservable();

    constructor(private httpClient: HttpClient) {
        super(httpClient);
     }

    getById(id: string): Observable<Account> {
        return this.httpClient.get<Account>(`${this.baseUrl}/api/Account/${id}`);
    }

    getByAccountHolderId(accountHolderId: string): Observable<Account[]> {
        return this.httpClient.get<Account[]>(`${this.baseUrl}/api/Account/accountHolder/${accountHolderId}`);
    }

    add(account: NewAccountModel): Observable<string> {
        return this.httpClient.post<string>(`${this.baseUrl}/api/Account`, account).pipe(
            tap(_ => this.reloadBs.next(true))
        );
    }
}