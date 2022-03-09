using NLog;
using System;
using System.Collections.Generic;

namespace HelperMethodTest
{
    public static class ExtensionMethod
    {

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static T GetProperty<T>(this IDictionary<string, object> properties, string propertyKey, T defaultValue = default(T), bool throwOnPropertyNull = false, bool throwException = true, bool throwOnValueNull = true)
        {
            if (properties == null)
            {
                if (throwOnPropertyNull)
                {
                    throw new ArgumentNullException("properties");
                }
                else
                {
                    return defaultValue;
                }
            }

            if (properties.ContainsKey(propertyKey))
            {
                try
                {
                    //Get default value if property value is null
                    if (!throwOnValueNull && null == properties[propertyKey])
                    {
                        return defaultValue;
                    }
                    return HelperMethod.ConvertType<T>(properties[propertyKey], propertyName: propertyKey, ignoreCase: true);
                }
                catch (Exception ex)
                {
                    if (!throwException)
                    {
                        logger.Error(string.Format("Property name: {0}, Exception: {1}", propertyKey, ex));
                        return defaultValue;
                    }
                    logger.Error(string.Format("Property name: {0}, Exception: {1}", propertyKey, ex));
                    throw;
                }
            }
            return defaultValue;
        }
    }
}
