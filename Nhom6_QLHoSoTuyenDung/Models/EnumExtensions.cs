using System.ComponentModel.DataAnnotations;

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
    }
}
