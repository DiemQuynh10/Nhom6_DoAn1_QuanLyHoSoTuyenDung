using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;

public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LichPhongVan>()
    .HasOne(l => l.PhongPhongVan)
    .WithMany(p => p.LichPhongVans)
    .HasForeignKey(l => l.PhongPhongVanId)
    .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<LichPhongVan>()
     .HasOne(l => l.UngVien)
     .WithMany()
     .HasForeignKey(l => l.UngVienId)
     .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LichPhongVan>()
            .HasOne(l => l.ViTriTuyenDung)
            .WithMany()
            .HasForeignKey(l => l.ViTriId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<UngVien> UngViens { get; set; }
    public DbSet<DanhGiaPhongVan> DanhGiaPhongVans { get; set; }
    public DbSet<HoSoLuuTru> HoSoLuuTrus { get; set; }
    public DbSet<LichPhongVan> LichPhongVans { get; set; }
    public DbSet<NguoiDung> NguoiDungs { get; set; }
    public DbSet<NhanVien> NhanViens { get; set; }
    public DbSet<NhanVienThamGiaPhongVan> NhanVienThamGiaPhongVans { get; set; }
    public DbSet<PhongBan> PhongBans { get; set; }
    public DbSet<PhongPhongVan> PhongPhongVans { get; set; }
    public DbSet<ViTriTuyenDung> ViTriTuyenDungs { get; set; }
}
