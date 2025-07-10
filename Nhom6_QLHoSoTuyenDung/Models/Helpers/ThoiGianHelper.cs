namespace Nhom6_QLHoSoTuyenDung.Models.Helpers
{
    public static class ThoiGianHelper
    {
        public static string TinhTuLuc(DateTime thoiGian)
        {
            var now = DateTime.Now;
            var span = now - thoiGian;

            if (span.TotalSeconds < 0)
                return "Sắp tới";

            if (span.TotalDays >= 1)
                return $"{(int)span.TotalDays} ngày trước";
            else if (span.TotalHours >= 1)
                return $"{(int)span.TotalHours} giờ trước";
            else if (span.TotalMinutes >= 1)
                return $"{(int)span.TotalMinutes} phút trước";
            else
                return "Vừa xong";
        }

    }
}
