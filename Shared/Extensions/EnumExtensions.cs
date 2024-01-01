using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace BlazorApp.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var type = value.GetType();
            var members = type.GetMember(value.ToString());
            var member = members.First();
            var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();
            var displayName = displayAttribute?.GetName();
            
            return displayName ?? value.ToString();

        }
    }
}
