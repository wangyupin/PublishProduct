using System.ComponentModel.DataAnnotations;

namespace LocalPortService.Core.ModelValid
{
    public static class MinimalApiModelValidator
    {
        public static (bool, string) Valid(object req)
        {
            // 創建 ValidationContext 實例
            var context = new ValidationContext(req);
            var results = new List<ValidationResult>();

            // 驗證模型
            bool isValid = ValidateObjectRecursive(req, results);
            var errors = string.Join(", ", results.Select(vr => vr.ErrorMessage));

            return (isValid, errors);

        }

        public static bool ValidateObjectRecursive(object obj, List<ValidationResult> results)
        {
            if (obj == null) return false;

            var context = new ValidationContext(obj);
            bool isValid = Validator.TryValidateObject(obj, context, results, true);

            // 遍历所有属性
            var properties = obj.GetType().GetProperties()
                .Where(prop => prop.CanRead
                && prop.GetIndexParameters().Length == 0
                && prop.GetValue(obj) != null);

            foreach (var property in properties)
            {
                object value = property.GetValue(obj);
                if (value != null && !property.PropertyType.IsPrimitive && !(value is string))
                {
                    bool childIsValid = ValidateObjectRecursive(value, results);
                    isValid = isValid && childIsValid;  // 递归结果影响最终 isValid
                }
            }

            return isValid;
        }

        public static (bool, string) SaleValid(object req)
        {
            // 創建 ValidationContext 實例
            var context = new ValidationContext(req);
            var results = new List<ValidationResult>();

            // 驗證模型
            bool isValid = Validator.TryValidateObject(req, context, results, validateAllProperties: true);
            var errors = string.Join(", ", results.Select(vr => vr.ErrorMessage));

            return (isValid, errors);
        }
    }
}
