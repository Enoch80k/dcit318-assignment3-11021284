using System;
using System.Collections.Generic;

// a. Record type for Transaction
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// b. Transaction processor interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// c. Three transaction processor implementations
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[BankTransfer] Processing {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d} (Id: {transaction.Id}).");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[MobileMoney] {transaction.Amount:C} spent on {transaction.Category} (Id: {transaction.Id}) via Mobile Money.");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[CryptoWallet] {transaction.Amount:C} spent on {transaction.Category} (Id: {transaction.Id}) via Crypto Wallet.");
    }
}

// d. Base Account class
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Transaction of {transaction.Amount:C} applied. New balance: {Balance:C}");
    }
}

// e. Sealed SavingsAccount
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction of {transaction.Amount:C} applied. Updated balance: {Balance:C}");
        }
    }
}

// f. FinanceApp integration & simulation
public class FinanceApp
{
    private List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        // i. SavingsAccount
        var savingsAccount = new SavingsAccount("SA123456", 1000m);

        // ii. Transactions
        var t1 = new Transaction(1, DateTime.Now, 120.00m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 250.00m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 90.00m, "Entertainment");

        // iii. Processors
        ITransactionProcessor mobileProcessor = new MobileMoneyProcessor();
        ITransactionProcessor bankProcessor = new BankTransferProcessor();
        ITransactionProcessor cryptoProcessor = new CryptoWalletProcessor();

        mobileProcessor.Process(t1);
        bankProcessor.Process(t2);
        cryptoProcessor.Process(t3);

        // iv. Apply to account
        savingsAccount.ApplyTransaction(t1);
        savingsAccount.ApplyTransaction(t2);
        savingsAccount.ApplyTransaction(t3);

        // v. Record transactions
        _transactions.AddRange(new[] { t1, t2, t3 });

        Console.WriteLine("All transactions recorded:");
        foreach (var t in _transactions)
        {
            Console.WriteLine($"\t{t}");
        }
    }

    // Main entry
    public static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
