using System;
using System.Globalization;
using System.Reflection;

namespace HelperMethodTest
{
    public static class HelperMethod
    {
        public static T ConvertType<T>(object value, string propertyName, bool ignoreCase = false)
        {
            try
            {
                return ConvertType<T>(value, ignoreCase);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(propertyName))
                {
                    var valueStr = value as string;
                    if (valueStr != null)
                    {
                        if (valueStr.Length > 100)
                        {
                            valueStr = valueStr.Substring(0, 100) + "...";
                        }
                        throw new Exception(string.Format("Failed to convert the value {0} in {1} property to {2} type. Please make sure the payload structure and value are correct. Error:{3}", valueStr, propertyName, typeof(T).FullName), ex);
                    }
                    else
                    {
                        throw new Exception(string.Format("Failed to convert the value in {0} property to {1} type. Please make sure the payload structure and value are correct. Error:{3}", propertyName, typeof(T).FullName), ex);
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        public static T ConvertType<T>(object value, bool ignoreCase = false)
        {
            try
            {
                return ConvertNoNullableType<T>(value, ignoreCase);
            }
            catch (InvalidCastException)
            {
                // to support conversions from 'long' to 'int?'
                if (IsNullableType(typeof(T)))
                {
                    if (value == null)
                    {
                        return (T)value;
                    }
                    else if (string.IsNullOrEmpty(value?.ToString())) //Empty Cannot be converted to int?
                    {
                        return default(T);
                    }
                    Type genericArgument = typeof(T).GetGenericArguments()[0];
                    MethodInfo m2 = typeof(HelperMethod).GetMethod("ConvertNoNullableType", new Type[] { typeof(object), typeof(bool) });
                    var m2Generic = m2.MakeGenericMethod(new Type[] { genericArgument });
                    return (T)m2Generic.Invoke(null, new object[] { value, ignoreCase });
                }

                throw;
            }
        }

        public static T ConvertNoNullableType<T>(object value, bool ignoreCase = false)
        {
            try
            {
                string strValue = value as string;
                if (typeof(T).IsEnum && (strValue != null))
                {
                    return (T)Enum.Parse(typeof(T), strValue, ignoreCase);
                }

                if (value is T)
                {
                    return (T)value;
                }

                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                MethodInfo m = typeof(T).GetMethod("Parse", new Type[] { typeof(string) });
                if (m != null)
                {
                    return (T)m.Invoke(null, new object[] { value });
                }

                throw;
            }
        }

        private static bool IsNullableType(Type type)
        {
            if (type.IsGenericType)
            {
                try
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(Nullable<>))
                    {
                        return true;
                    }
                }
                catch (NotSupportedException)
                {
                    // Swallow exceptions if cannot get generic type
                }
            }
            return false;
        }
    }
}
