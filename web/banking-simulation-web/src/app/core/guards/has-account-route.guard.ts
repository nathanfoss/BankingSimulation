import { inject } from '@angular/core';
import { Router } from '@angular/router';

import { of } from 'rxjs';
import { mergeMap } from 'rxjs/operators';

import { AccountHolderService } from '../services/account-holder.service';
import { AccountHolder } from '../models/accountHolder.model';


export const hasAccountGuard = () => {
    const accountHolderService = inject(AccountHolderService);
    const router = inject(Router);

    return accountHolderService.accountHolder$.pipe(
        mergeMap((accountHolder: AccountHolder | null) => {
            if (!accountHolder) {
                return router.navigate(['home']);
            }

            return of(true);
        })
    )
}