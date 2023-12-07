import { Component, OnDestroy, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';

import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Observable, Subject, map, mergeMap, takeUntil, tap, withLatestFrom, debounceTime } from 'rxjs';

import { AccountHolderService } from '../core/services/account-holder.service';
import { AccountService } from '../core/services/account.service';

import { NewAccountModel } from '../core/models/newAccount.model';
import { AccountTypeEnum } from '../core/enums/accountType.enum';
import { AccountHolder } from '../core/models/accountHolder.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MatDividerModule, MatInputModule, MatFormFieldModule, MatButtonModule, MatSelectModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent implements OnInit, OnDestroy {
  private openAccountSubject = new Subject<boolean>();
  private loginSubject = new Subject<boolean>();
  private destroySubject = new Subject<boolean>();
  private destroy$ = this.destroySubject.asObservable();
  private reloadAccountHolder = new Subject<boolean>();

  loginForm = new FormGroup({
    identifier: new FormControl('', [Validators.required])
  });

  openAccountForm = new FormGroup({
    accountHolderName: new FormControl('', [Validators.required]),
    accountHolderPublicIdentifier: new FormControl('', [Validators.required])
  });

  login$: Observable<boolean> = this.loginSubject.asObservable()
  .pipe(
    debounceTime(250),
    withLatestFrom(this.loginForm.valueChanges),
    mergeMap(([hasValue, values]) => this.accountHolderService.getByPublicIdentifier(values.identifier ?? '')),
    tap((accountHolder: AccountHolder) => {
      if (!accountHolder) {
        this.snackBar.open('Account not found. Please try again or open a new account.', 'Dismiss');
      }
    }),
    map(_ => true),
    takeUntil(this.destroy$)
  );

  addAccount$: Observable<boolean> = this.openAccountSubject.asObservable()
  .pipe(
    debounceTime(250),
    withLatestFrom(this.openAccountForm.valueChanges),
    mergeMap(([hasValue, values]) => {
      const newAccount = {...values} as NewAccountModel;
      newAccount.accountTypeId = AccountTypeEnum.Savings;
      return this.accountService.add(newAccount);
    }),
    tap(_ => this.snackBar.open('Welcome! Your new account has been created!', 'Dismiss')),
    map(_ => true),
    tap(_ => this.reloadAccountHolder.next(true)),
    takeUntil(this.destroy$)
  );

  reloadAccountHolder$: Observable<boolean> = this.reloadAccountHolder.asObservable()
  .pipe(
    withLatestFrom(this.openAccountForm.valueChanges),
    mergeMap(([hasValue, values]) => this.accountHolderService.getByPublicIdentifier(values.accountHolderPublicIdentifier ?? '')),
    map(_ => true),
    takeUntil(this.destroy$)
  );

  constructor(private accountHolderService: AccountHolderService,
    private accountService: AccountService,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
      this.addAccount$.subscribe();
      this.login$.subscribe();
      this.reloadAccountHolder$.subscribe();
  }

  ngOnDestroy(): void {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }

  openAccount() {
    this.openAccountSubject.next(true);
  }

  login() {
    this.loginSubject.next(true);
  }
}
