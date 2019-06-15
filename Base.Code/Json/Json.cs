using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;

namespace Base.Code
{
    public static class Json
    {
        /// <summary>
        /// 强制转换为标准的Json结构并返回HttpResponseMessage
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HttpResponseMessage ToJsonHttpResponse(this object obj)
        {
            string str;
            if (obj is string || obj is char)
            {
                str = obj.ToString();
            }
            else
            {
                var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                str = JsonConvert.SerializeObject(obj, timeConverter);
            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        public static object ToJson(this string Json)
        {
            JsonSerializerSettings jSetting = new JsonSerializerSettings();
            jSetting.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            return Json == null ? null : JsonConvert.DeserializeObject(Json, jSetting);
        }
        public static string ToJson(this object obj)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        public static string ToJson(this object obj, string datetimeformats)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeformats };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        public static T ToObject<T>(this string Json)
        {
            return Json == null ? default(T) : JsonConvert.DeserializeObject<T>(Json);
        }
        public static List<T> ToList<T>(this string Json)
        {
            JsonSerializerSettings jSetting = new JsonSerializerSettings();
            jSetting.DateTimeZoneHandling = DateTimeZoneHandling.Local;

            return Json == null ? null : JsonConvert.DeserializeObject<List<T>>(Json, jSetting);
        }
        public static DataTable ToTable(this string Json)
        {
            JsonSerializerSettings jSetting = new JsonSerializerSettings();
            jSetting.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            return Json == null ? null : JsonConvert.DeserializeObject<DataTable>(Json, jSetting);
        }
        public static JObject ToJObject(this string Json)
        {
            return Json == null ? JObject.Parse("{}") : JObject.Parse(Json.Replace("&nbsp;", ""));
        }
    }
}
