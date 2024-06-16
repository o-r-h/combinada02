using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;


namespace Veritrade2018.Helpers
{
    public static class Extensiones
    {

        public static T GetValue<T>(this DataRow row, string columnName)
        {

            int index = -1;
            if (row != null) index = row.Table.Columns.IndexOf(columnName);

            return (index < 0 || index > row.ItemArray.Count() || row[index] == DBNull.Value)
                      ? default(T)
                      //: (T)row[index];
                      : (T)Convert.ChangeType(row[index], typeof(T));
        }

        public static string GetUntilOrEmpty(this string text, string stopAt = "-")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }

        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
            {
                var obj = propertyDescriptor.GetValue(anonymousObject);
                expando.Add(propertyDescriptor.Name, obj);
            }

            return (ExpandoObject)expando;
        }


        //GetEnumDisplayName
        public static string GetDn(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .Name;
        }

        public static IEnumerable<int> To(this int from, int to)
        {
            if (from < to)
            {
                while (from <= to)
                {
                    yield return from++;
                }
            }
            else
            {
                while (from >= to)
                {
                    yield return from--;
                }
            }
        }

        public static dynamic Cast(object obj , Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        }

        public static ExpandoObject DeepCopy(this ExpandoObject original)
        {
            var clone = new ExpandoObject();

            var _original = (IDictionary<string, object>)original;
            var _clone = (IDictionary<string, object>)clone;

            foreach (var kvp in _original)
                _clone.Add(kvp.Key, kvp.Value is ExpandoObject ? DeepCopy((ExpandoObject)kvp.Value) : kvp.Value);

            return clone;
        }


        #region  Helper for DataTable

        public static List<T> DataTableToList<T>(this DataTable dataTable) where T : new()
        {
            var dataList = new List<T>();

            //Define what attributes to be read from the class
            const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

            //Read Attribute Names and Types
            var objFieldNames = typeof(T).GetProperties(flags).Cast<System.Reflection.PropertyInfo>().
                Select(item => new
                {
                    Name = item.Name,
                    Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
                }).ToList();

            //Read Datatable column names and types
            var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
                Select(item => new
                {
                    Name = item.ColumnName,
                    Type = item.DataType
                }).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var classObj = new T();

                foreach (var dtField in dtlFieldNames)
                {
                    System.Reflection.PropertyInfo propertyInfos = classObj.GetType().GetProperty(dtField.Name);

                    var field = objFieldNames.Find(x => x.Name == dtField.Name);

                    if (field != null)
                    {

                        if (propertyInfos.PropertyType == typeof(DateTime))
                        {
                            propertyInfos.SetValue
                            (classObj, convertToDateTime(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(Nullable<DateTime>))
                        {
                            propertyInfos.SetValue
                            (classObj, convertToDateTime(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(int))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToInt(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(long))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToLong(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(decimal))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToDecimal(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(String))
                        {
                            if (dataRow[dtField.Name].GetType() == typeof(DateTime))
                            {
                                propertyInfos.SetValue
                                (classObj, ConvertToDateString(dataRow[dtField.Name]), null);
                            }
                            else
                            {
                                propertyInfos.SetValue
                                (classObj, ConvertToString(dataRow[dtField.Name]), null);
                            }
                        }
                        else
                        {

                            propertyInfos.SetValue
                                (classObj, Convert.ChangeType(dataRow[dtField.Name], propertyInfos.PropertyType), null);

                        }
                    }
                }
                dataList.Add(classObj);
            }
            return dataList;
        }

        private static string ConvertToDateString(object date)
        {
            if (date == null)
                return string.Empty;

            return date == null ? string.Empty : Convert.ToDateTime(date).ConvertDate();
        }

        private static string ConvertToString(object value)
        {
            return Convert.ToString(ReturnEmptyIfNull(value));
        }

        private static int ConvertToInt(object value)
        {
            return Convert.ToInt32(ReturnZeroIfNull(value));
        }

        private static long ConvertToLong(object value)
        {
            return Convert.ToInt64(ReturnZeroIfNull(value));
        }

        private static decimal ConvertToDecimal(object value)
        {
            return Convert.ToDecimal(ReturnZeroIfNull(value));
        }

        private static DateTime convertToDateTime(object date)
        {
            return Convert.ToDateTime(ReturnDateTimeMinIfNull(date));
        }

        public static string ConvertDate(this DateTime datetTime, bool excludeHoursAndMinutes = false)
        {
            if (datetTime != DateTime.MinValue)
            {
                if (excludeHoursAndMinutes)
                    return datetTime.ToString("yyyy-MM-dd");
                return datetTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            return null;
        }
        public static object ReturnEmptyIfNull(this object value)
        {
            if (value == DBNull.Value)
                return string.Empty;
            if (value == null)
                return string.Empty;
            return value;
        }
        public static object ReturnZeroIfNull(this object value)
        {
            if (value == DBNull.Value)
                return 0;
            if (value == null)
                return 0;
            return value;
        }
        public static object ReturnDateTimeMinIfNull(this object value)
        {
            if (value == DBNull.Value)
                return DateTime.MinValue;
            if (value == null)
                return DateTime.MinValue;
            return value;
        }


        #endregion

        public static string Truncate(this string text, int maxLength, string suffix = "...", bool withTooltip = false )
        {
            text = text.Trim();
            var  _suffix = suffix.RemoveHTMLTags().Trim();
            string str = text;
            if (maxLength > 0)
            {
                int length = maxLength - _suffix.Length;
                if (length <= 0)
                {
                    return str;
                }
                if ((text != null) && (text.Length > maxLength))
                {
                    return (text.Substring(0, length).TrimEnd(new char[0]) + (!withTooltip? suffix: text.AppendMoreLess() ) );
                }
            }
            return str;
        }
        public static string Truncate2(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : string.Concat(value.Substring(0, maxLength), value.AppendMoreLess());
        }

        public static string AppendMoreLess(this string text)
        {
            return "<a javascript:void(0) class='more_less' data-toggle='popover' data-content='" + text +
                                     "'>[...]</a>";
        }


        public static string RemoveHTMLTags(this string content)
        {
            var cleaned = string.Empty;
            try
            {
                StringBuilder textOnly = new StringBuilder();
                using (var reader = XmlNodeReader.Create(new System.IO.StringReader("<xml>" + content + "</xml>")))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Text)
                            textOnly.Append(reader.ReadContentAsString());
                    }
                }
                cleaned = textOnly.ToString();
            }
            catch
            {
                //A tag is probably not closed. fallback to regex string clean.
                string textOnly = string.Empty;
                Regex tagRemove = new Regex(@"<[^>]*(>|$)");
                Regex compressSpaces = new Regex(@"[\s\r\n]+");
                textOnly = tagRemove.Replace(content, string.Empty);
                textOnly = compressSpaces.Replace(textOnly, " ");
                cleaned = textOnly;
            }

            return cleaned;
        }

        private static readonly StringComparer ToDictionaryDefaultComparer =
            StringComparer.OrdinalIgnoreCase;

        /// <summary>
        /// Converts an object's properties that can be read
        /// to an IDictionary.
        /// </summary>
        public static IDictionary<string, object> ConvertToDictionary(
            this object @this,
            StringComparer comparer = null)
        {
            // The following is adapted from: 
            // https://stackoverflow.com/a/15698713/569302
            var dictionary = new Dictionary<string, object>(
                comparer ?? ToDictionaryDefaultComparer);
            foreach (var propertyInfo in @this.GetType().GetProperties())
            {
                if (propertyInfo.CanRead &&
                    propertyInfo.GetIndexParameters().Length == 0)
                {
                    dictionary[propertyInfo.Name] =
                        propertyInfo.GetValue(@this, null);
                }
            }

            return dictionary;
        }

        //public static Dictionary<string, VarGeneral> GetVariables(this System.Web.HttpContext context, string IdGrupo = null, string IdParent = null)
        //{

        //    var dic = context.Items["vars"] as Dictionary<string, VarGeneral>;
        //    if (dic == null)
        //    {
        //        var ret = new Dictionary<string, VarGeneral>();
        //        VarGeneral.GetAllAsList().ForEach(i => ret.Add(i.IdVariable, i));
        //        context.Items["vars"] = dic = ret;
        //    }

        //    if (!string.IsNullOrEmpty(IdGrupo))
        //    {
        //        dic = dic.Where(m => m.Value.IdGrupo == IdGrupo).Select(x => new { key = x.Key, value = x.Value }).ToList().ToDictionary();
        //    }


        //    return dic;
        //}


        public static Dictionary<string, string> GetQueryValues(string url)
        {
            //Create a Uri from the Url
            Uri uri = new Uri(url);

            //Parse the Query part of the url
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);

            //Generate a dictionary with the keys and values
            Dictionary<string, string> queryValues = new Dictionary<string, string>();
            foreach (string key in nameValueCollection.Keys)
            {
                queryValues.Add(key, nameValueCollection[key]);
            }

            return queryValues;
        }

        public static string ToQueryString(this IDictionary<string, string> dict)
        {

            if (dict.Count == 0) return string.Empty;

            var buffer = new StringBuilder();
            int count = 0;
            bool end = false;

            foreach (var key in dict.Keys)
            {
                if (count == dict.Count - 1) end = true;

                if (end)
                    buffer.AppendFormat("{0}={1}", key, dict[key]);
                else
                    buffer.AppendFormat("{0}={1}&", key, dict[key]);

                count++;
            }

            return buffer.ToString();
        }

        public static void SetCookie(string key, string value, TimeSpan expires)
        {
            HttpCookie encodedCookie = /*HttpSecureCookie.Encode(*/ new HttpCookie(key, value)/*)*/;

            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[key];
                //cookieOld.Expires = DateTime.Now.Add(expires);
                cookieOld.Value = encodedCookie.Value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                encodedCookie.Expires = DateTime.Now.Add(expires);
                HttpContext.Current.Response.Cookies.Add(encodedCookie);
            }
        }
        public static string GetCookie(string key)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

            //if (cookie != null)
            //{
            //    // For security purpose, we need to encrypt the value.
            //    HttpCookie decodedCookie = HttpSecureCookie.Decode(cookie);
            //    value = decodedCookie.Value;
            //}
            if (cookie != null) value = cookie.Value.ToString();
            return value;
        }

        public static int ConvertToInt(string stringAsInt)
        {
            int myInt;
            return int.TryParse(stringAsInt, out myInt) ? myInt : 0;
        }

        public static bool IsPropertyExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }
    }
}