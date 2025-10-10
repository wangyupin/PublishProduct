using System.Collections.Generic;
using System.Data;

namespace CityHubCore.Infrastructure.DB {
    public class TableValueParameterModel {
        public string Lable { get; set; }
        public string Value { get; set; }
    }

    public static class TableValueParameterHelper {
        public static DataTable GetUdtStr(List<TableValueParameterModel> list) {
            #region -- check parameters
            if (list is null) return null;
            #endregion

            DataTable RET = new();

            RET.Columns.Add("str1", typeof(string));

            foreach (var item in list) {
                RET.Rows.Add(item.Value);
            }

            return RET;
        }

        public static DataTable GetUdtStrStr(List<TableValueParameterModel> list) {
            #region -- check parameters
            if (list is null) return null;
            #endregion

            DataTable RET = new();

            RET.Columns.Add("str1", typeof(string));
            RET.Columns.Add("str2", typeof(string));

            foreach (var item in list) {
                RET.Rows.Add(item.Lable, item.Value);
            }

            return RET;
        }

        /// <summary>
        /// 因現行POVSQL查詢的資料無主鍵，故需用此function轉換。已用於StoreKPI的畫面。
        /// </summary>
        /// <param name="list"></param>
        public static List<TableValueParameterModel> FormatValueLabel(List<TableValueParameterModel> list) {
            #region -- check parameters
            if (list is null) return null;
            #endregion

            foreach (var item in list) {
                var tmpStr = item.Value.Split("：");

                if (tmpStr.Length < 2) continue;

                item.Lable = tmpStr[0];
                item.Value = tmpStr[1];
            }
            return list;
        }
    }
}
