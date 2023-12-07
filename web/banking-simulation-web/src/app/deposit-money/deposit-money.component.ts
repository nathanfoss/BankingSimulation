import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Subject, debounceTime, mergeMap, takeUntil, tap, withLatestFrom } from 'rxjs';

import { MoneyService } from '../core/services/money.service';

@Component({
  selector: 'app-deposit-money',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, MatFormFieldModule,
    MatInputModule, FormsModule, MatButtonModule, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose],
  templateUrl: './deposit-money.component.html',
  styleUrl: './deposit-money.component.scss'
})
export class DepositMoneyComponent implements OnInit, OnDestroy {
  private destroySubject = new Subject<boolean>();
  private destroy$ = this.destroySubject.asObservable();

  private submitSubject = new Subject<boolean>();

  amount = new FormControl(0, [Validators.required, Validators.min(0.01)]);
  
  deposit$ = this.submitSubject.asObservable().pipe(
    debounceTime(250),
    withLatestFrom(this.amount.valueChanges),
    mergeMap(([_, amount]) => this.moneyService.deposit({
      accountId: this.data.accountId,
      amount: amount ?? 0
    })),
    tap(_ => this.snackBar.open('Deposit successful!', 'Dismiss')),
    tap(_ => this.close()),
    takeUntil(this.destroy$)
  )

  constructor(
    public dialogRef: MatDialogRef<DepositMoneyComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { accountId : string},
    private moneyService: MoneyService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.deposit$.subscribe();
  }

  ngOnDestroy(): void {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }

  submit() {
    this.submitSubject.next(true);
  }

  close() {
    this.dialogRef.close();
  }
}
