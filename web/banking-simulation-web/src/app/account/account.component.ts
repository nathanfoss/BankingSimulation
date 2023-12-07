import { Component, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Params } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import { BehaviorSubject, Observable, Subject, combineLatest } from 'rxjs';
import { map, mergeMap, takeUntil, tap } from 'rxjs/operators';

import { AccountService } from '../core/services/account.service';
import { AccountLogService } from '../core/services/account-log.service';

import { Account } from '../core/models/account.model';
import { AccountLog } from '../core/models/accountLog.model';

import { DepositMoneyComponent } from '../deposit-money/deposit-money.component';
import { WithdrawMoneyComponent } from '../withdraw-money/withdraw-money.component';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [CommonModule, RouterModule, MatTableModule, MatDividerModule, MatButtonModule, MatIconModule, DepositMoneyComponent, WithdrawMoneyComponent],
  templateUrl: './account.component.html',
  styleUrl: './account.component.scss'
})
export class AccountComponent implements OnDestroy {
  private destroySubject = new Subject<boolean>();
  private destroy$ = this.destroySubject.asObservable();

  private reloadBs = new BehaviorSubject<boolean>(true);
  private reload$ = this.reloadBs.asObservable();

  private accountId$: Observable<string> = this.route.params.pipe(
    map((routeParams: Params) => routeParams['accountId'])
  );

  account$: Observable<Account> = combineLatest([this.reload$, this.accountId$]).pipe(
    mergeMap(([_, id]) => this.accountService.getById(id))
  );

  accountLogs$: Observable<AccountLog[]> = combineLatest([this.reload$, this.accountId$]).pipe(
    mergeMap(([_, id]) => this.accountLogService.getByAccount(id))
  );

  displayedColumns = ['eventType', 'metadata', 'createdDate']

  constructor (private route: ActivatedRoute,
    private accountService: AccountService,
    private accountLogService: AccountLogService,
    private dialog: MatDialog) { }

  ngOnDestroy(): void {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }

  deposit(accountId: string): void{
    const dialogRef = this.dialog.open(DepositMoneyComponent, {
      data: { accountId: accountId }
    });

    dialogRef.afterClosed().pipe(
      tap(_ => this.reloadBs.next(true)),
      takeUntil(this.destroy$)
    ).subscribe();
  }

  withdraw(accountId: string): void {
    const dialogRef = this.dialog.open(WithdrawMoneyComponent, {
      data: { accountId: accountId }
    });

    dialogRef.afterClosed().pipe(
      tap(_ => this.reloadBs.next(true)),
      takeUntil(this.destroy$)
    ).subscribe();
  }

  refresh(): void {
    this.reloadBs.next(true);
  }
}
