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
        public static string TinhThoiGianConLai(DateTime? thoiGian)
        {
            if (!thoiGian.HasValue)
                return "Không rõ thời gian";

            var now = DateTime.Now;
            var span = thoiGian.Value - now;

            if (span.TotalDays >= 2)
            {
                return $"Phỏng vấn lúc {thoiGian.Value:dd/MM/yyyy HH:mm}";
            }
            else if (span.TotalDays >= 1)
            {
                int gio = (int)(span.TotalHours - 24);
                return $"1 ngày {(gio > 0 ? $"{gio} giờ " : "")}nữa";
            }
            else if (span.TotalMinutes > 0)
            {
                int gio = (int)span.TotalHours;
                int phut = span.Minutes;
                return $"{gio} giờ {phut} phút nữa";
            }
            else
            {
                return "Đã quá giờ";
            }
        }
    }
}
