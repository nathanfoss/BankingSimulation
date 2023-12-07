import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';

import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';

import { Observable, Subject, combineLatest } from 'rxjs';
import { filter, withLatestFrom, mergeMap, map, tap, takeUntil, debounceTime } from 'rxjs/operators';

import { AccountHolder } from '../core/models/accountHolder.model';
import { AccountType } from '../core/models/accountType.model';
import { NewAccountModel } from '../core/models/newAccount.model';

import { AccountService } from '../core/services/account.service';
import { AccountHolderService } from '../core/services/account-holder.service';
import { AccountTypeService } from '../core/services/account-type.service';

@Component({
  selector: 'app-open-account',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MatInputModule, MatButtonModule, MatFormFieldModule, MatSelectModule],
  templateUrl: './open-account.component.html',
  styleUrl: './open-account.component.scss'
})
export class OpenAccountComponent implements OnInit, OnDestroy {
  private openAccountSubject = new Subject<boolean>();
  private destroySubject = new Subject<boolean>();
  private destroy$ = this.destroySubject.asObservable();
  private accountHolder$: Observable<AccountHolder | null> = this.accountHolderService.accountHolder$.pipe(
    filter(value => !!value)
  );
  
  accountTypes$: Observable<AccountType[]> = this.accountTypeService.getAll();

  openAccountForm = new FormGroup({
    accountTypeId: new FormControl('', [Validators.required]),
    linkedAccountId: new FormControl()
  });

  accountMetadata$ = combineLatest([this.openAccountForm.valueChanges, this.accountHolder$]);

  addAccount$: Observable<boolean> = this.openAccountSubject.asObservable()
  .pipe(
    debounceTime(250),
    withLatestFrom(this.accountMetadata$),
    mergeMap(([_, [values, accountHolder]]) => {
      const newAccount = {
        accountHolderName: accountHolder?.fullName ?? '',
        accountHolderPublicIdentifier: accountHolder?.publicIdentifier ?? '',
        accountTypeId: +(values.accountTypeId ?? 0),
        linkedAccountId: values.linkedAccountId
      } as NewAccountModel;

      return this.accountService.add(newAccount);
    }),
    map(_ => true),
    tap(_ => this.openAccountForm.reset()),
    takeUntil(this.destroy$)
  );

  constructor(private accountHolderService: AccountHolderService,
    private accountService: AccountService,
    private accountTypeService: AccountTypeService) { }

  ngOnInit(): void {
    this.addAccount$.subscribe();
  }

  ngOnDestroy(): void {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }

  openAccount(): void {
    this.openAccountSubject.next(true);
  }
}
