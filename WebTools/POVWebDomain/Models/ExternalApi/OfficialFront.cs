using POVWebDomain.Models.API.StoreSrv.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.ExternalApi.OfficialFront
{
    public class GetCategoryRequest
    {
        public int StoreNumber { get; set; }
    }

    public class GetCategoryResponse
    {
        public int CategoryID { get; set; }
        public int? ParentID { get; set; }
        public string CategoryName { get; set; }
        public bool Active { get; set; }
        public List<GetCategoryResponse> Children { get; set; }
    }

    public static class GetCategoryResponseExtensions
    {
        public static List<MultipleLayerOption<int>> ToMultipleLayerOptions(this List<GetCategoryResponse> categories, bool activeOnly = true)
        {
            var filtered = activeOnly ? categories?.Where(c => c.Active) : categories;
            return filtered?.Select(c => c.ToMultipleLayerOption(activeOnly)).ToList() ?? new List<MultipleLayerOption<int>>();
        }

        public static MultipleLayerOption<int> ToMultipleLayerOption(this GetCategoryResponse category, bool activeOnly = true)
        {
            return new MultipleLayerOption<int>
            {
                Value = category.CategoryID,
                Label = category.CategoryName,
                Children = category.Children?.ToMultipleLayerOptions(activeOnly) ?? new List<MultipleLayerOption<int>>()
            };
        }
    }
}
