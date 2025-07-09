using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models;
namespace Nhom6_QLHoSoTuyenDung
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext") ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian sống của session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;              // bắt buộc lưu cookie
            });
            builder.Services.AddHttpContextAccessor(); 
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                PhongBanSeedData.Seed(context);
                NhanVienSeedData.Seed(context);
                NguoiDungSeedData.Seed(context);
                PhongPhongVanSeedData.Seed(context);
                ViTriTuyenDungSeedData.Seed(context);
                UngVienSeedData.Seed(context);
                LichPhongVanSeedData.Seed(context);
                NhanVienThamGiaPhongVanSeedData.Seed(context);
                DanhGiaPhongVanSeedData.Seed(context); 
                HoSoLuuTruSeedData.Seed(context);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();
            app.UseCookiePolicy(); // ✅ Thêm dòng này

            app.UseSession();
            app.UseAuthentication(); // nếu dùng Identity
            app.UseAuthorization();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=NguoiDungs}/{action=DangNhap}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
