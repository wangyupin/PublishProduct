using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace LocalPortService.Core.Helper
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public static string[] _formats = new string[] {
            "yyyyMMdd", "yyyy-MM-dd", "yyyy/MM/dd",
            "yyyyMMddHHmm", "yyyy-MM-dd HH:mm", "yyyy/MM/dd HH:mm",
            "yyyyMMddHHmmss", "yyyy-MM-dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss",
            "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.fff",
            "yyyy-MM-dd'T'HH:mm:ss.ff",
            "yyyy-MM-dd'T'HH:mm:ss.f"
        };
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var c = reader.GetString();
            DateTime.TryParseExact(reader.GetString(), _formats, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateTime);
            return dateTime;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));
        }
    }
}
