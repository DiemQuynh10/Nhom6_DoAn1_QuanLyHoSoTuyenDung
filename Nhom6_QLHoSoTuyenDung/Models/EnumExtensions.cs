using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Nhom6_QLHoSoTuyenDung.Models
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .Cast<DisplayAttribute>()
                .FirstOrDefault()?.Name ?? enumValue.ToString();
        }

        // 🔁 Chiều ngược lại: từ Display Name => Enum
        public static T? GetEnumFromDisplayName<T>(string displayName) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(displayName)) return null;

            foreach (var value in Enum.GetValues(typeof(T)).Cast<T>())
            {
                var name = (value as Enum).GetDisplayName();
                if (string.Equals(name, displayName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return value;
                }
            }

            return null;
        }
    }
}
