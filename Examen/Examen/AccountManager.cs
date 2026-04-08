using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examen
{
    public class Account
    {
        public string Owner { get; set; }
        public string CardNumber { get; set; }
        public string AccountNumber { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int FailedLoginAttempts { get; set; }
        public decimal Balance { get; set; }

        public Dictionary<string, int> DepositCounts { get; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> DepositTotals { get; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> TransferCounts { get; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> TransferTotals { get; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> WithdrawCounts { get; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> WithdrawTotals { get; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> RechargeCounts { get; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> RechargeTotals { get; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> ServicePaymentCounts { get; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> ServicePaymentTotals { get; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> TagRechargeCounts { get; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> TagRechargeTotals { get; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> PurchaseCounts { get; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> PurchaseTotals { get; } = new Dictionary<string, decimal>();
        public int TotalPurchaseItems { get; set; }
        public List<PurchaseRecord> PurchaseHistory { get; } = new List<PurchaseRecord>();
        public List<ReturnRecord> ReturnHistory { get; } = new List<ReturnRecord>();
        public decimal TotalRefunded => ReturnHistory.Sum(r => r.RefundAmount);
        public decimal TotalDamageCharges => ReturnHistory.Sum(r => r.DamageCharge);

        public int TotalDeposits => DepositCounts.Values.Sum();
        public decimal TotalDeposited => DepositTotals.Values.Sum();
        public int TotalTransfers => TransferCounts.Values.Sum();
        public decimal TotalTransferred => TransferTotals.Values.Sum();
        public int TotalWithdrawals => WithdrawCounts.Values.Sum();
        public decimal TotalWithdrawn => WithdrawTotals.Values.Sum();
        public int TotalRecharges => RechargeCounts.Values.Sum();
        public decimal TotalRecharged => RechargeTotals.Values.Sum();
        public int TotalServicePayments => ServicePaymentCounts.Values.Sum();
        public decimal TotalServicesPaid => ServicePaymentTotals.Values.Sum();
        public int TotalTagRecharges => TagRechargeCounts.Values.Sum();
        public decimal TotalTagRecharged => TagRechargeTotals.Values.Sum();
        public int TotalPurchases => PurchaseCounts.Values.Sum();
        public decimal TotalPurchased => PurchaseTotals.Values.Sum();

        public Account(string owner, string cardNumber, string accountNumber, string phone, string address, string email, string password)
        {
            Owner = owner;
            CardNumber = cardNumber;
            AccountNumber = accountNumber;
            Phone = phone;
            Address = address;
            Email = email;
            Password = password;
            FailedLoginAttempts = 0;
            Balance = 0m;
        }
    }

    public class PurchaseRecord
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPaid { get; set; }
        public DateTime Date { get; set; }
    }

    public class ReturnRecord
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OriginalCost { get; set; }
        public bool IsGoodCondition { get; set; }
        public decimal DamageCharge { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal ChargePercent { get; set; }
        public DateTime Date { get; set; }
    }

    public static class AccountManager
    {
        public static List<Account> Accounts { get; } = new List<Account>();
        public static Account CurrentAccount { get; set; }
        public static int TotalFailedLoginAttempts { get; set; } = 0;

        public static string GenerateCardNumber()
        {
            var rnd = new Random();
            var parts = new List<int>();
            for (int i = 0; i < 4; i++) parts.Add(rnd.Next(1000, 9999));
            return string.Join("-", parts);
        }

        public static string GenerateAccountNumber()
        {
            var rnd = new Random();
            var sb = new StringBuilder();
            for (int i = 0; i < 11; i++) sb.Append(rnd.Next(0, 10).ToString());
            return sb.ToString();
        }

        public static Account CreateAccount(string owner, string phone, string address, string email, string password)
        {
            // check duplicate email
            if (Accounts.Any(x => string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase)))
                return null;
            var card = GenerateCardNumber();
            var acctNum = GenerateAccountNumber();
            var acc = new Account(owner, card, acctNum, phone, address, email, password);
            Accounts.Add(acc);
            return acc;
        }

        public static Account FindByCard(string cardNumber)
        {
            return Accounts.FirstOrDefault(a => a.CardNumber == cardNumber);
        }

        public static Account FindByEmailAndPassword(string email, string password)
        {
            return Accounts.FirstOrDefault(a => string.Equals(a.Email, email, StringComparison.OrdinalIgnoreCase) && a.Password == password);
        }

        public static void Logout()
        {
            CurrentAccount = null;
        }
    }
}
