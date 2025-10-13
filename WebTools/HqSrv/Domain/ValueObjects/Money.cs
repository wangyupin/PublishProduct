using System;

namespace HqSrv.Domain.ValueObjects
{
    /// <summary>
    /// 金錢值物件 - 確保價格處理的一致性
    /// </summary>
    public class Money : IEquatable<Money>
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        private Money() { }

        public static Money Create(decimal amount, string currency = "TWD")
        {
            if (amount < 0)
                throw new ArgumentException("金額不能為負數", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("幣別不能為空", nameof(currency));

            return new Money
            {
                Amount = amount,
                Currency = currency
            };
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"不同幣別無法相加: {Currency} + {other.Currency}");

            return Create(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"不同幣別無法相減: {Currency} - {other.Currency}");

            return Create(Amount - other.Amount, Currency);
        }

        public bool IsGreaterThan(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"不同幣別無法比較: {Currency} vs {other.Currency}");

            return Amount > other.Amount;
        }

        // 值物件相等性
        public bool Equals(Money other)
        {
            if (other is null) return false;
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Money);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }

        public override string ToString()
        {
            return $"{Amount:N2} {Currency}";
        }
    }
}
