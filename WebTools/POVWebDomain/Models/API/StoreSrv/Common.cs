using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.Common
{
    public class Condition
    {
        public string Field { get; set; }
        public string Method { get; set; }
        public string Type { get; set; }
        public string KeywordStart { get; set; }
        public string KeywordEnd { get; set; }
    }

    public class AdvanceSearchRequest
    {
       public List<Condition> Conditions { get; set; }

        private string GetCondiStrText(Condition condition)
        {
            string condStr = "";
            string field = condition.Field, keywordStart = condition.KeywordStart, keywordEnd = condition.KeywordEnd;

            if(condition.Type == "date")
            {
                keywordStart = keywordStart.Replace("-", "");
                keywordEnd = keywordEnd.Replace("-", "");
            }

            switch (condition.Method)
            {
                case "equal":
                    condStr = String.Format("AND LOWER({0}) = LOWER('{1}')", field, keywordStart);
                    break;
                case "contains":
                    condStr = String.Format("AND LOWER({0}) Like '%' + LOWER('{1}') + '%'", field, keywordStart);
                    break;
                case "more than":
                    condStr = String.Format("AND LOWER({0}) > + LOWER('{1}') + '%'", field, keywordStart);
                    break;
                case "less than":
                    condStr = String.Format("AND LOWER({0}) < + LOWER('{1}') + '%'", field, keywordStart);
                    break;
                case "between":
                    condStr = String.Format("AND LOWER({0}) >= + LOWER('{1}') AND LOWER({0}) <= + LOWER('{2}')", field, keywordStart, keywordEnd);
                    break;
            }
            return condStr;
        }

        private string GetCondiStrNum(Condition condition)
        {
            string condStr = "";
            string field = condition.Field, keywordStart = condition.KeywordStart, keywordEnd = condition.KeywordEnd;
            switch (condition.Method)
            {
                case "equal":
                    condStr = String.Format(" AND {0} = {1}", field, keywordStart);
                    break;
                case "more than":
                    condStr = String.Format(" AND {0} > {1}", field, keywordStart);
                    break;
                case "less than":
                    condStr = String.Format(" AND {0} < {1}", field, keywordStart);
                    break;
                case "between":
                    condStr = String.Format(" AND {0} >= {1} AND {0} <= {2}", field, keywordStart, keywordEnd);
                    break;
            }
            return condStr;
        }

        private string GetCondiStrBit(Condition condition)
        {
            string condStr = "";
            string field = condition.Field, keywordStart = condition.KeywordStart, keywordEnd = condition.KeywordEnd, Type = condition.Type;
            int keyword = keywordStart == "是" ? 1 : 0;
            if (Type == "text")
            {
                if (keywordStart == "是" | keywordStart == "否" | keywordEnd == "是" | keywordEnd == "否")
                {
                    switch (condition.Method)
                    {
                        case "equal":
                            condStr = String.Format("AND LOWER({0}) = LOWER('{1}')", field, keyword);
                            break;
                        case "contains":
                            condStr = String.Format("AND LOWER({0}) Like '%' + LOWER('{1}') + '%'", field, keyword);
                            break;
                    }
                } else
                {
                    GetCondiStrText(condition);
                }
                
            } else if (Type == "Number")
            {
                GetCondiStrNum(condition);
            }
            
            return condStr;
        }
        public string GenerateSqlString()
        {
            string whereStr = "AND 1=1";
           
            Conditions.ForEach(condition =>
            {
                whereStr += condition.Type=="number"? GetCondiStrNum(condition): GetCondiStrText(condition); 
            });

            return whereStr;
        }

        public string GenerateSqlbit()
        {
            string whereStr = string.Empty;
            Conditions.ForEach(condition =>
            {
                if (condition.Type == "text" && condition.KeywordStart == "是" | condition.KeywordStart == "否" | condition.KeywordEnd == "是" | condition.KeywordEnd == "否")
                {
                    whereStr += GetCondiStrBit(condition);
                }
                else
                {
                    whereStr += condition.Type == "number" ? GetCondiStrNum(condition) : GetCondiStrText(condition);
                }
            });
            return whereStr;
        }
    }

    public class SearchTerm
    {
        public List<string> Field { get; set;}

        public string GenerateSqlString()
        {
            string whereStr = "and ";
            if(Field.Count > 0)
            {
                foreach (var field in Field.Select((value, i) => (value, i )))
                {
                    if(field.i == 0)
                    {
                        whereStr += $"({field.value} like '%' + @Q + '%'";
                    }
                    else
                    {
                        whereStr += $"OR {field.value} like '%' + @Q + '%'";
                    }
                }
                whereStr += ")";
            }
            return whereStr;
        }

        public string GenerateColumnString()
        {
            string columnStr = string.Join(", ", Field);
            
            return columnStr;
        }
    }

    public class Option<T>
    {
        public T Value { get; set; }
        public string Label { get; set; }

        public Option() { }

        public Option(T value, string label)
        {
            this.Value = value;
            this.Label = label;
        }
    }

    public class CheckboxOption<T>
    {
        public T ID { get; set; }
        public string Name { get; set; }

        public CheckboxOption() { }

        public CheckboxOption(T id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
    }

    public class MultipleLayerOption<T> : Option<T>
    {
        public List<MultipleLayerOption<T>> Children { get; set; }

        public MultipleLayerOption()
        {
            Children = new List<MultipleLayerOption<T>>();
        }

        public MultipleLayerOption(T value, string label) : base(value, label)
        {
            Children = new List<MultipleLayerOption<T>>();
        }
    }

    public class ImportExcelDataRequest
    {
        [Required(ErrorMessage = "請先選擇轉換類別")]
        public string Type { get; set; }
        public string ChangePerson { get; set; }
        public IFormFile File { get; set; }
    }

    public class CommonAction
    {
        public DynamicParameters GenerateParams(object request)
        {
            var paramList = new DynamicParameters();
            foreach (PropertyInfo propertyInfo in request.GetType().GetProperties())
            {
                Type t = propertyInfo.PropertyType;
                string paramName = $"@{propertyInfo.Name}";

                if(t == typeof(Option<string>))
                {
                    Option<string> opt = (Option<string>) propertyInfo.GetValue(request, null);
                    var paramValue = opt?.Value?.ToString() ?? "";
                    paramList.Add(paramName, paramValue);
                }
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var value = propertyInfo.GetValue(request, null);
                    paramList.Add(paramName, value);
                }

                else if (t == typeof(bool))
                {
                    var value = propertyInfo.GetValue(request, null);
                    paramList.Add(paramName, value);
                }
                else if(!t.IsGenericType)
                {
                    var paramValue = propertyInfo.GetValue(request, null)?.ToString() ?? "";
                    paramList.Add(paramName, paramValue);
                }
                else if (propertyInfo.GetValue(request, null) != null && typeof(IEnumerable<object>).IsAssignableFrom(t))
                {
                    DataTable RET1 = new();
                    RET1.Columns.Add("Str1");
                    foreach (var i in (IEnumerable<object>)propertyInfo.GetValue(request, null))
                    {
                        if(i.GetType() == typeof(Option<string>))
                        {
                            var opt = (Option<string>) i;
                            RET1.Rows.Add(opt?.Value?.ToString());
                        }
                        else
                        {
                            RET1.Rows.Add(i.ToString());
                        }
                    }
                    paramList.Add(paramName, RET1.AsTableValuedParameter($"[POVWeb].[udtStr]"));
                }
            }
            return paramList;
        }

        public DynamicParameters GenerateGenericParams(object request)
        {
            var paramList = new DynamicParameters();
            foreach (PropertyInfo propertyInfo in request.GetType().GetProperties())
            {
                Type t = propertyInfo.PropertyType;
                string paramName = $"@{propertyInfo.Name}";

                if (t.IsGenericType && propertyInfo.GetValue(request, null) != null)
                {
                    DataTable RET1 = new();
                    RET1.Columns.Add("Str1");
                    foreach (var i in (IEnumerable<object>)propertyInfo.GetValue(request, null))
                    {
                        if (i.GetType() == typeof(Option<string>))
                        {
                            var opt = (Option<string>)i;
                            RET1.Rows.Add(opt?.Value?.ToString());
                        }
                        else
                        {
                            RET1.Rows.Add(i.ToString());
                        }
                    }
                    paramList.Add(paramName, RET1.AsTableValuedParameter($"[POVWeb].[udtStr]"));
                }
            }
            return paramList;
        }
    }

    public class ImportRequest
    {
        public List<IFormFile> Files { get; set; }
        public string ChangePerson { get; set; }
    }

    public class ImportFirstRequest: ImportRequest
    {
        public string Type { get; set; }
    }
    

    public class DynamicObjectCloner
    {
        public static void CopyDynamicProperties(dynamic source, dynamic target)
        {
            if (source is IDictionary<string, object> sourceDict)
            {
                foreach (var kvp in sourceDict)
                {
                    ((IDictionary<string, object>)target).Add(kvp.Key, kvp.Value);
                }
            }
            else
            {
                foreach (var kvp in source)
                {
                    ((IDictionary<string, object>)target).Add(kvp.Key, kvp.Value);
                }
            }
           
        }
    }

    public class CustomException : Exception
    {
        public int ErrorCode { get; }

        public CustomException(string message, int errorCode, Exception innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    public static class ParamGenerator
    {
        public static DynamicParameters GenerateParams(object request)
        {
            var paramList = new DynamicParameters();
            foreach (PropertyInfo propertyInfo in request.GetType().GetProperties())
            {
                Type t = propertyInfo.PropertyType;
                string paramName = $"@{propertyInfo.Name}";
                if (!t.IsGenericType)
                {
                    if (t == typeof(string))
                    {
                        var paramValue = propertyInfo.GetValue(request, null)?.ToString() ?? "";
                        paramList.Add(paramName, paramValue);
                    }
                    else if (t == typeof(int) || t == typeof(decimal) || t == typeof(float) || t == typeof(double))
                    {
                        var paramValue = propertyInfo.GetValue(request, null) ?? 0;
                        paramList.Add(paramName, paramValue);
                    }
                    else if (t == typeof(bool))
                    {
                        string boolStr = (bool)propertyInfo.GetValue(request, null) == false ? "0" : "1";
                        paramList.Add(paramName, boolStr);
                    }
                    else if (t == typeof(DateTime))
                    {
                        DateTime paramValue = (DateTime)propertyInfo.GetValue(request, null);
                        var value = paramValue.ToString("yy-MM-dd HH:mm:ss.fff");
                        paramList.Add(paramName, value);
                    }
                }
                else if (propertyInfo.GetValue(request, null) != null)
                {
                    DataTable RET1 = new();

                    var value = propertyInfo.GetValue(request, null);

                    // 确保它是 `IEnumerable<object>` 类型，并且不为 null
                    var list = value as IEnumerable<object>;
                    if (list == null || !list.Any())
                    {
                        // ⚠ 如果列表为空，创建一个空的 DataTable（但结构要匹配）
                        RET1.Columns.Add("Str1");
                        RET1.Columns.Add("Str2");
                        RET1.Columns.Add("Str3");
                        RET1.Columns.Add("Str4");
                        RET1.Columns.Add("Str5"); // 必须和 SQL UDT 结构匹配
                        paramList.Add(paramName, RET1.AsTableValuedParameter("[POVWeb].[udtStr5]"));
                    } 
                    else
                    {
                        foreach (var i in (IEnumerable<object>)propertyInfo.GetValue(request, null))
                        {
                            if (i.GetType().IsPrimitive || i is string)
                            {
                                if (RET1.Columns.Count == 0)
                                {
                                    RET1.Columns.Add("Str1");
                                }
                                RET1.Rows.Add(i.ToString());
                                paramList.Add(paramName, RET1.AsTableValuedParameter($"[POVWeb].[udtStr]"));
                            }
                            else
                            {
                                // 物件型別，動態添加列
                                if (RET1.Columns.Count == 0)
                                {
                                    // 根據最大5個屬性創建列
                                    RET1.Columns.Add("Str1");
                                    RET1.Columns.Add("Str2");
                                    RET1.Columns.Add("Str3");
                                    RET1.Columns.Add("Str4");
                                    RET1.Columns.Add("Str5");
                                }

                                var properties = i.GetType().GetProperties();
                                var rowValues = new object[RET1.Columns.Count];

                                // 根據物件屬性填充行

                                for (int count = 0; count < properties.Length && count < 5; count++)
                                {
                                    rowValues[count] = properties[count].GetValue(i)?.ToString() ?? "";
                                }

                                RET1.Rows.Add(rowValues);
                            }
                        }
                        if (RET1.Columns.Count == 1)
                        {
                            paramList.Add(paramName, RET1.AsTableValuedParameter($"[POVWeb].[udtStr]"));
                        }
                        else
                        {
                            paramList.Add(paramName, RET1.AsTableValuedParameter($"[POVWeb].[udtStr5]"));
                        }
                    }
                }
            }
            return paramList;
        }
    }
}
