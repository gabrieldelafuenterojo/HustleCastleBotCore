using System;
using System.Reflection;

namespace HustleCastleBotCore
{
    public static class PropertyExtension
    {
        public static void SetPropertyValue(this object obj, string propName, string value)
        {
            PropertyInfo p = obj.GetType().GetProperty(propName);
            obj.GetType().GetProperty(propName).SetValue(obj, Enum.Parse(p.PropertyType, value), null);
        }
    }
}
