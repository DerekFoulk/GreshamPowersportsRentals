using System;
using System.Linq;
using BlazorApp.Shared.Exceptions;

namespace BlazorApp.Shared.Extensions
{
    public static class ObjectExtensions
    {
        public static T GetAttributeFrom<T>(this object obj, string propertyName) where T : Attribute
        {
            var attrType = typeof(T);
            var property = obj.GetType().GetProperty(propertyName);

            if (property?.GetCustomAttributes(attrType, false).Single() is not T attr)
                throw new AttributeNotFoundException();
            
            return attr;
        }
    }
}
