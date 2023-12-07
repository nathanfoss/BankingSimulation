import { Routes } from '@angular/router';

import { AccountComponent } from './account/account.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { HomeComponent } from './home/home.component';
import { TransferComponent } from './transfer/transfer.component';

export const routes: Routes = [
    { path: 'home', component: HomeComponent },
    { path: 'transfer', component: TransferComponent },
    { path: 'account/:accountId', component: AccountComponent },
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: '**', component: PageNotFoundComponent }
];
