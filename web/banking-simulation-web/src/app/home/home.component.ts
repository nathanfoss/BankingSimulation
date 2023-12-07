import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatDividerModule } from '@angular/material/divider';

import { Observable } from 'rxjs';

import { AccountHolderService } from '../core/services/account-holder.service';

import { AccountHolder } from '../core/models/accountHolder.model';

import { LoginComponent } from '../login/login.component';
import { OpenAccountComponent } from '../open-account/open-account.component';
import { AccountsComponent } from '../accounts/accounts.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MatDividerModule, LoginComponent, OpenAccountComponent, AccountsComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent {
  accountHolder$: Observable<AccountHolder | null> = this.accountHolderService.accountHolder$;

  constructor(private accountHolderService: AccountHolderService) { }
}
