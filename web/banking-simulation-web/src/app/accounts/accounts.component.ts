import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { MatTableModule, MatTableDataSource } from '@angular/material/table';

import { Observable, combineLatest, BehaviorSubject, of } from 'rxjs';
import { filter, mergeMap, map, tap } from 'rxjs/operators';

import { AccountHolderService } from '../core/services/account-holder.service';
import { AccountService } from '../core/services/account.service';

import { Account } from '../core/models/account.model';
import { AccountHolder } from '../core/models/accountHolder.model';

@Component({
  selector: 'app-accounts',
  standalone: true,
  imports: [CommonModule, RouterModule, MatTableModule],
  templateUrl: './accounts.component.html',
  styleUrl: './accounts.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AccountsComponent {
  private accountHolder$: Observable<AccountHolder | null> = this.accountHolderService.accountHolder$
  .pipe(
    filter(accountHolder => !!accountHolder)
  );
  private reloadAccounts$: Observable<boolean> = this.accountService.reload$;
  
  private cachedAccountBs = new BehaviorSubject<Account[]>([]);
  private cachedAccounts$: Observable<Account[]> = this.cachedAccountBs.asObservable();

  accounts$: Observable<Account[]> = combineLatest([this.accountHolder$, this.reloadAccounts$]).pipe(
    mergeMap(([accountHolder, _]) => {
      if (!accountHolder?.publicIdentifier) {
        return [];
      }

      return this.accountService.getByAccountHolderId(accountHolder?.publicIdentifier); 
    }),
    tap((accounts: Account[]) => this.cachedAccountBs.next(accounts))
  );

  totalBalance$: Observable<number> = this.cachedAccounts$.pipe(
    map((accounts: Account[]) => {
      let sum = 0;
      accounts.forEach(account => sum += account.balance);
      return sum;
    })
  );

  displayedColumns = ['id', 'type', 'linkedAccountId', 'balance'];

  constructor(private accountHolderService: AccountHolderService,
    private accountService: AccountService) { }
}
