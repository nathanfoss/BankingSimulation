import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { BehaviorSubject, Observable, tap } from "rxjs";

import { HttpService } from "./http.service";

import { AccountHolder } from "../models/accountHolder.model";

@Injectable({ providedIn: 'root' })
export class AccountHolderService extends HttpService {
    private accountHolderBs: BehaviorSubject<AccountHolder | null> = new BehaviorSubject<AccountHolder | null>(null);
    accountHolder$: Observable<AccountHolder | null> = this.accountHolderBs.asObservable();

    constructor(private httpClient: HttpClient) {
        super(httpClient);
    }

    update(accountHolder: AccountHolder): void {
        this.accountHolderBs.next(accountHolder);
    }

    getByPublicIdentifier(identifier: string): Observable<AccountHolder> {
        return this.httpClient.get<AccountHolder>(`${this.baseUrl}/api/AccountHolder/identifier/${identifier}`)
        .pipe(
            tap(accountHolder => this.accountHolderBs.next(accountHolder))
        );
    }
}