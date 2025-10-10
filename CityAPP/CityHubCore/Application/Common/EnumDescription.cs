using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace CityHubCore.Application.Common { 
    public static class EnumExtensions {
        public static string getDesc(this Task source) {
            FieldInfo fi = source.GetType().GetField(source.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
             typeof(DescriptionAttribute), false);
            if (attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
    }
}
