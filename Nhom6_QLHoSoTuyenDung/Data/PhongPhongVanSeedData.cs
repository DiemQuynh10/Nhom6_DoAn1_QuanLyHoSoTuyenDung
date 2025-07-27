using Nhom6_QLHoSoTuyenDung.Models.Entities;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class PhongPhongVanSeedData
    {
        public static void Seed(AppDbContext context)
        {
            if (context.PhongPhongVans.Any()) return;

            var now = new DateTime(2025, 1, 15);

            var dsPhong = new List<PhongPhongVan>
            {
                new PhongPhongVan {
                    Id = "PPV001", TenPhong = "Phòng 201",
                    DiaDiem = "Tầng 2", MoTa = "Rộng 30m2, máy chiếu, kỹ thuật, âm thanh, vị trí tầng 2"
                },
                new PhongPhongVan {
                    Id = "PPV002", TenPhong = "Phòng 202",
                    DiaDiem = "Tầng 2", MoTa = "Rộng 25m2, điều hòa, phù hợp phỏng vấn HR, vị trí tầng 2"
                },
                new PhongPhongVan {
                    Id = "PPV003", TenPhong = "Phòng 301",
                    DiaDiem = "Tầng 3", MoTa = "Phòng rộng 40m2, có bảng trình chiếu, chuyên dùng cho backend"
                },
                new PhongPhongVan {
                    Id = "PPV004", TenPhong = "Phòng 302",
                    DiaDiem = "Tầng 3", MoTa = "Phòng nhỏ, 20m2, yên tĩnh, chuyên phỏng vấn frontend"
                },
                new PhongPhongVan {
                    Id = "PPV005", TenPhong = "Phòng 401",
                    DiaDiem = "Tầng 4", MoTa = "Rộng 50m2, có âm thanh – ánh sáng tốt, dùng cho phỏng vấn nhóm hoặc vòng cuối"
                },
            };

            context.PhongPhongVans.AddRange(dsPhong);
            context.SaveChanges();
        }
    }

}
