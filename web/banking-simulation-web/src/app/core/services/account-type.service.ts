import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { HttpService } from "./http.service";
import { AccountType } from '../models/accountType.model';

@Injectable({ providedIn: 'root' })
export class AccountTypeService extends HttpService {
    constructor(private httpClient: HttpClient) {
        super(httpClient);
    }

    getAll(): Observable<AccountType[]> {
        return this.httpClient.get<AccountType[]>(`${this.baseUrl}/api/AccountType/all`);
    }
}