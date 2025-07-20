using Nhom6_QLHoSoTuyenDung.Models.Entities;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class DanhGiaPhongVanSeedData
    {
        public static void Seed(AppDbContext context)
        {
            if (context.DanhGiaPhongVans.Any()) return;

            var dsDanhGia = new List<DanhGiaPhongVan>();
            var rnd = new Random();
            int idCount = 1;

            var nhanXetList = new[]
            {
                "Thái độ tốt", "Chuyên môn vững", "Cần cải thiện kỹ năng mềm",
                "Tự tin, trả lời lưu loát", "Kỹ năng phù hợp vị trí", "Thiếu kinh nghiệm thực tế"
            };

            // Không còn "Cần bổ sung", thay vào "Duyệt luôn"
            var deXuatList = new[] { "Tiếp nhận", "Từ chối" };

            for (int i = 1; i <= 40; i++)
            {
                var lichId = $"LPV{i:000}";

                // Lấy nhân viên tham gia có vai trò Đánh giá
                var danhGiaNV = context.NhanVienThamGiaPhongVans
                    .Where(x => x.LichPhongVanId == lichId && x.VaiTro == "Đánh giá")
                    .Select(x => x.NhanVienId)
                    .Distinct()
                    .ToList();

                foreach (var nvId in danhGiaNV)
                {
                    var diem = rnd.Next(6, 11); // 6 đến 10 điểm

                    var danhGia = new DanhGiaPhongVan
                    {
                        Id = $"DG{idCount++:000}",
                        LichPhongVanId = lichId,
                        NhanVienDanhGiaId = nvId,
                        DiemDanhGia = diem,
                        NhanXet = nhanXetList[rnd.Next(nhanXetList.Length)],
                        DeXuat = diem >= 9 ? "Duyệt luôn" : deXuatList[rnd.Next(deXuatList.Length)]
                    };

                    dsDanhGia.Add(danhGia);
                }
            }

            context.DanhGiaPhongVans.AddRange(dsDanhGia);
            context.SaveChanges();
        }
    }
}
