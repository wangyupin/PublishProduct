using HqSrv.Domain.Events;
using HqSrv.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HqSrv.Domain.Entities
{
    /// <summary>
    /// 商品實體 - 核心業務邏輯
    /// </summary>
    public class Product
    {
        // 私有建構函式，確保只能透過工廠方法建立
        private Product() { }

        // 基本資訊
        public string ParentId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string MoreInfo { get; private set; }

        // 價格資訊
        public decimal SuggestPrice { get; private set; }
        public decimal Price { get; private set; }
        public decimal Cost { get; private set; }

        // 庫存管理
        public bool HasSku { get; private set; }
        public int? Qty { get; private set; }
        public int? OnceQty { get; private set; }
        public string OuterId { get; private set; }

        // SKU 清單
        public List<ProductSku> SkuList { get; private set; } = new List<ProductSku>();

        // 規格資訊
        public List<ProductSpecification> Specifications { get; private set; } = new List<ProductSpecification>();

        // 銷售時間
        public DateTime? SellingStartDateTime { get; private set; }
        public DateTime? SellingEndDateTime { get; private set; }

        // 配送資訊
        public string ApplyType { get; private set; }
        public DateTime? ExpectShippingDate { get; private set; }
        public int? ShippingPrepareDay { get; private set; }

        // 物理尺寸
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int Length { get; private set; }
        public int Weight { get; private set; }

        // 溫層設定
        public string TemperatureTypeDef { get; private set; }

        // 新增：Domain Events 支援
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// 添加領域事件
        /// </summary>
        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// 清除領域事件
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// 標記為已發布（觸發事件）
        /// </summary>
        public void MarkAsPublished(string platformId, string publishResult)
        {
            var publishEvent = ProductPublishedEvent.Create(ParentId, platformId, publishResult);
            AddDomainEvent(publishEvent);
        }

        /// <summary>
        /// 標記驗證失敗（觸發事件）
        /// </summary>
        public void MarkValidationFailed(string platformId, List<string> errors)
        {
            var validationFailedEvent = ProductValidationFailedEvent.Create(ParentId, platformId, errors);
            AddDomainEvent(validationFailedEvent);
        }

        // ============================================
        // 工廠方法 - 建立新商品
        // ============================================
        public static Product Create(
            string parentId,
            string title,
            decimal price,
            decimal cost,
            string applyType = "一般")
        {
            // 業務驗證
            if (string.IsNullOrWhiteSpace(parentId))
                throw new ArgumentException("商品父編號不能為空", nameof(parentId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("商品標題不能為空", nameof(title));

            if (price < 0)
                throw new ArgumentException("商品價格不能為負數", nameof(price));

            if (cost < 0)
                throw new ArgumentException("商品成本不能為負數", nameof(cost));

            return new Product
            {
                ParentId = parentId,
                Title = title,
                Price = price,
                Cost = cost,
                ApplyType = applyType,
                TemperatureTypeDef = "Normal"
            };
        }

        // ============================================
        // 業務方法
        // ============================================

        /// <summary>
        /// 更新基本資訊
        /// </summary>
        public void UpdateBasicInfo(string title, string description, string moreInfo)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("商品標題不能為空", nameof(title));

            Title = title;
            Description = description;
            MoreInfo = moreInfo;
        }

        /// <summary>
        /// 更新價格資訊
        /// </summary>
        public void UpdatePricing(decimal suggestPrice, decimal price, decimal cost)
        {
            if (price < 0) throw new ArgumentException("商品價格不能為負數");
            if (cost < 0) throw new ArgumentException("商品成本不能為負數");
            if (suggestPrice < 0) throw new ArgumentException("建議售價不能為負數");

            SuggestPrice = suggestPrice;
            Price = price;
            Cost = cost;
        }

        /// <summary>
        /// 設定銷售時間
        /// </summary>
        public void SetSellingPeriod(DateTime? startTime, DateTime? endTime)
        {
            if (startTime.HasValue && endTime.HasValue && startTime >= endTime)
                throw new ArgumentException("銷售開始時間必須早於結束時間");

            SellingStartDateTime = startTime;
            SellingEndDateTime = endTime;
        }

        /// <summary>
        /// 啟用 SKU 模式
        /// </summary>
        public void EnableSkuMode()
        {
            HasSku = true;
            Qty = null;
            OnceQty = null;
            OuterId = null;
        }

        /// <summary>
        /// 停用 SKU 模式
        /// </summary>
        public void DisableSkuMode(int qty, int onceQty, string outerId)
        {
            if (qty < 0) throw new ArgumentException("庫存數量不能為負數");
            if (onceQty <= 0) throw new ArgumentException("單次購買數量必須大於0");
            if (string.IsNullOrWhiteSpace(outerId)) throw new ArgumentException("外部編號不能為空");

            HasSku = false;
            Qty = qty;
            OnceQty = onceQty;
            OuterId = outerId;
            SkuList.Clear();
        }

        /// <summary>
        /// 添加 SKU
        /// </summary>
        public void AddSku(ProductSku sku)
        {
            if (!HasSku)
                throw new InvalidOperationException("必須先啟用 SKU 模式");

            if (SkuList.Any(s => s.OuterId == sku.OuterId))
                throw new ArgumentException($"SKU 外部編號 {sku.OuterId} 已存在");

            SkuList.Add(sku);
        }

        /// <summary>
        /// 驗證商品是否可以發布
        /// </summary>
        public bool CanPublish()
        {
            if (string.IsNullOrWhiteSpace(Title)) return false;
            if (Price <= 0) return false;

            if (HasSku)
            {
                return SkuList.Any() && SkuList.All(s => s.IsValid());
            }
            else
            {
                return Qty.HasValue && Qty > 0 &&
                       OnceQty.HasValue && OnceQty > 0 &&
                       !string.IsNullOrWhiteSpace(OuterId);
            }
        }

        /// <summary>
        /// 設定物理尺寸
        /// </summary>
        public void SetDimensions(int height, int width, int length, int weight)
        {
            if (height < 0 || width < 0 || length < 0 || weight < 0)
                throw new ArgumentException("尺寸和重量不能為負數");

            Height = height;
            Width = width;
            Length = length;
            Weight = weight;
        }
    }
}
