using System.Collections.Generic;
using System;
using System.Linq;

namespace HqSrv.Domain.ValueObjects
{
    /// <summary>
    /// 商品規格值物件
    /// </summary>
    public class ProductSpecification
    {
        public string Name { get; private set; }
        public List<string> Options { get; private set; }

        private ProductSpecification() { }

        public static ProductSpecification Create(string name, List<string> options)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("規格名稱不能為空");

            return new ProductSpecification
            {
                Name = name,
                Options = options ?? new List<string>()
            };
        }

        // 值物件相等性
        public override bool Equals(object obj)
        {
            if (obj is not ProductSpecification other) return false;
            return Name == other.Name &&
                   Options.SequenceEqual(other.Options);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, string.Join(",", Options));
        }
    }
}