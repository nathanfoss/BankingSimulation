# BankingSimulation
We provide a full-featured digital banking experience. Our APIs support creating new checking and savings accounts and all of your money management needs. Deposit, Withdraw, and Transfer money seemlessly between accounts. We also provide your personal account view and a full history for each of your accounts. Open your new account today!

## Business Requirements
Provide checking and savings account options for clients (checking accounts must be linked to a valid savings account). Deposits, withdrawals, and transfers can occur on any account as long as there are sufficient funds to execute the transaction.

Each customer can view all of their assigned accounts and view a full transaction history for each account.

## Architecture
- Console / Web Application Leveraging the Domain-Driven Design (DDD) Pattern.
- CQRS Powered by MediatR
- Automated testing provided by XUnit
- Quality gateways provided by husky commit hooks
- In-memory DB provided by EF Core
- Event-Driven Architecture using background services
- Web front-end powered by Angular Material

## Runbook
### Web
- Run the BankingSimulation.API project
- Navigate to web/banking-simulation-web. Use the Angular cli to serve the UI

### Console
- Run the BankingSimulation.App project. This service will provision accounts and perform a series of transactions and log the output to the console
