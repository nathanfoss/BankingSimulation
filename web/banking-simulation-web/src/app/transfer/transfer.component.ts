import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { BehaviorSubject, Subject, Observable, combineLatest } from 'rxjs';
import { filter, mergeMap, tap, takeUntil, debounceTime, map, withLatestFrom } from 'rxjs/operators'

import { AccountHolderService } from '../core/services/account-holder.service';
import { AccountService } from '../core/services/account.service';
import { MoneyService } from '../core/services/money.service';

import { AccountHolder } from '../core/models/accountHolder.model';
import { Account } from '../core/models/account.model';
import { TransferModel } from '../core/models/transfer.model';

@Component({
  selector: 'app-transfer',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MatFormFieldModule, MatSelectModule, MatInputModule, MatButtonModule],
  templateUrl: './transfer.component.html',
  styleUrl: './transfer.component.scss'
})
export class TransferComponent implements OnInit, OnDestroy {
  private accountHolder$ = this.accountHolderService.accountHolder$.pipe(
    filter((holder: AccountHolder | null) => !!holder)
  );

  private destroySubject = new Subject<boolean>();
  private destroy$ = this.destroySubject.asObservable();

  private submitSubject = new Subject<boolean>();
  private refreshBs = new BehaviorSubject<boolean>(true);

  private accountsBs = new BehaviorSubject<Account[]>([]);
  private cachedAccounts$ = this.accountsBs.asObservable();
  
  accounts$: Observable<Account[]> = combineLatest([this.accountHolder$, this.refreshBs.asObservable()]).pipe(
    mergeMap(([accountHolder, _]) => this.accountService.getByAccountHolderId(accountHolder?.publicIdentifier ?? '')),
    tap((accounts: Account[]) => this.accountsBs.next(accounts))
    );
    
  formGroup = new FormGroup({
    fromAccountId: new FormControl('', [Validators.required]),
    toAccountId: new FormControl('', [Validators.required]),
    amount: new FormControl(0, [Validators.required]),
  });

  selectedAccount$: Observable<Account | undefined> = combineLatest([this.cachedAccounts$, this.formGroup.controls.fromAccountId.valueChanges]).pipe(
    debounceTime(250),
    map(([accounts, fromAccountId]) => accounts.find(a => a.id === fromAccountId))
  );

  availableBalance$: Observable<number> = this.selectedAccount$.pipe(
    map((account: Account | undefined) => account?.balance ?? 0)
  );

  submit$ = this.submitSubject.asObservable().pipe(
    withLatestFrom(this.formGroup.valueChanges),
    debounceTime(250),
    mergeMap(([_, values]) => {
      return this.moneyService.transfer({...values} as TransferModel)
    }),
    tap(_ => this.refreshBs.next(true)),
    tap(_ => this.snackBar.open('Transfer completed successfully', 'Dismiss')),
    tap(_ => this.formGroup.reset())
,    takeUntil(this.destroy$)
  );

  toAccountOptions$: Observable<Account[]> = combineLatest([this.cachedAccounts$, this.selectedAccount$]).pipe(
    map(([accounts, fromAccount]) => accounts.filter(a => a.id !== fromAccount?.id))
  )

  constructor(private accountHolderService: AccountHolderService,
    private accountService: AccountService,
    private moneyService: MoneyService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.submit$.subscribe();
  }

  ngOnDestroy(): void {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }

  submit(): void {
    this.submitSubject.next(true);
  }
}
