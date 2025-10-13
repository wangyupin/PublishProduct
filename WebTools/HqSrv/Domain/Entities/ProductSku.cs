using System;

namespace HqSrv.Domain.Entities
{
    /// <summary>
    /// 商品 SKU 實體
    /// </summary>
    public class ProductSku
    {
        private ProductSku() { }

        public string OuterId { get; private set; }
        public string OriginalOuterId { get; private set; }
        public string Name { get; private set; }
        public int Qty { get; private set; }
        public int OriginalQty { get; private set; }
        public int OnceQty { get; private set; }
        public decimal SuggestPrice { get; private set; }
        public decimal Price { get; private set; }
        public decimal Cost { get; private set; }
        public int SafetyStockQty { get; private set; }
        public bool IsShow { get; private set; } = true;

        public static ProductSku Create(
            string outerId,
            string name,
            int qty,
            int onceQty,
            decimal price,
            decimal cost)
        {
            if (string.IsNullOrWhiteSpace(outerId))
                throw new ArgumentException("SKU 外部編號不能為空");

            if (qty < 0) throw new ArgumentException("庫存數量不能為負數");
            if (onceQty <= 0) throw new ArgumentException("單次購買數量必須大於0");
            if (price < 0) throw new ArgumentException("價格不能為負數");
            if (cost < 0) throw new ArgumentException("成本不能為負數");

            return new ProductSku
            {
                OuterId = outerId,
                OriginalOuterId = outerId,
                Name = name,
                Qty = qty,
                OriginalQty = qty,
                OnceQty = onceQty,
                Price = price,
                Cost = cost
            };
        }

        public void UpdatePricing(decimal suggestPrice, decimal price, decimal cost)
        {
            if (price < 0) throw new ArgumentException("價格不能為負數");
            if (cost < 0) throw new ArgumentException("成本不能為負數");

            SuggestPrice = suggestPrice;
            Price = price;
            Cost = cost;
        }

        public void UpdateInventory(int qty, int safetyStockQty)
        {
            if (qty < 0) throw new ArgumentException("庫存數量不能為負數");
            if (safetyStockQty < 0) throw new ArgumentException("安全庫存不能為負數");

            Qty = qty;
            SafetyStockQty = safetyStockQty;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(OuterId) &&
                   Qty >= 0 &&
                   OnceQty > 0 &&
                   Price >= 0 &&
                   Cost >= 0;
        }
    }
}
