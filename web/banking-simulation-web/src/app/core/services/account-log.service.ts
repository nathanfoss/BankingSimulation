import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { HttpService } from './http.service';
import { AccountLog } from '../models/accountLog.model';

@Injectable({ providedIn: 'root' })
export class AccountLogService extends HttpService {
    constructor(private httpClient: HttpClient) {
        super(httpClient);
    }

    getByAccount(accountId: string): Observable<AccountLog[]> {
        return this.httpClient.get<AccountLog[]>(`${this.baseUrl}/api/AccountLog/account/${accountId}`);
    }
}