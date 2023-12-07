import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { HttpService } from './http.service';
import { TransactionModel } from '../models/transaction.model';
import { TransferModel } from '../models/transfer.model';

@Injectable({ providedIn: 'root' })
export class MoneyService extends HttpService {
    constructor(private httpClient: HttpClient) {
        super(httpClient);
    }

    deposit(model: TransactionModel): Observable<boolean> {
        return this.httpClient.put(`${this.baseUrl}/api/Money/deposit`, model, { responseType: 'text' })
        .pipe(map(_ => true));
    }

    withdraw(model: TransactionModel): Observable<boolean> {
        return this.httpClient.put(`${this.baseUrl}/api/Money/withdraw`, model, { responseType: 'text' })
        .pipe(map(_ => true));
    }

    transfer(model: TransferModel): Observable<boolean> {
        return this.httpClient.put(`${this.baseUrl}/api/Money/transfer`, model, { responseType: 'text' })
        .pipe(map(_ => true));
    }
}