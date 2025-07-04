using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nhom6_QLHoSoTuyenDung.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhongBans",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ten_phong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mo_ta = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongBans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PhongPhongVans",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ten_phong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dia_diem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mo_ta = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongPhongVans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "NhanViens",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ho_ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ma_nv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ngay_sinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    chuc_vu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phong_ban_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sdt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ngay_vao_cty = table.Column<DateTime>(type: "datetime2", nullable: true),
                    kinh_nghiem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mo_ta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    muc_luong = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanViens", x => x.id);
                    table.ForeignKey(
                        name: "FK_NhanViens_PhongBans_phong_ban_id",
                        column: x => x.phong_ban_id,
                        principalTable: "PhongBans",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ViTriTuyenDungs",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ten_vi_tri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    so_luong_can_tuyen = table.Column<int>(type: "int", nullable: true),
                    trang_thai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phong_ban_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ky_nang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViTriTuyenDungs", x => x.id);
                    table.ForeignKey(
                        name: "FK_ViTriTuyenDungs_PhongBans_phong_ban_id",
                        column: x => x.phong_ban_id,
                        principalTable: "PhongBans",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    nhan_vien_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ten_dang_nhap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mat_khau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vai_tro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phong_ban_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ho_ten = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sdt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungs", x => x.nhan_vien_id);
                    table.ForeignKey(
                        name: "FK_NguoiDungs_NhanViens_nhan_vien_id",
                        column: x => x.nhan_vien_id,
                        principalTable: "NhanViens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NguoiDungs_PhongBans_phong_ban_id",
                        column: x => x.phong_ban_id,
                        principalTable: "PhongBans",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ThongKeTuyenDungs",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    vi_tri_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    so_luong_ung_vien = table.Column<int>(type: "int", nullable: true),
                    so_luong_dat = table.Column<int>(type: "int", nullable: true),
                    so_luong_truot = table.Column<int>(type: "int", nullable: true),
                    thoi_gian_thong_ke = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongKeTuyenDungs", x => x.id);
                    table.ForeignKey(
                        name: "FK_ThongKeTuyenDungs_ViTriTuyenDungs_vi_tri_id",
                        column: x => x.vi_tri_id,
                        principalTable: "ViTriTuyenDungs",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "UngViens",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ho_ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ngay_sinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sdt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vi_tri_ung_tuyen_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    link_cv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    kinh_nghiem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    thanh_tich = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mo_ta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    trang_thai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ngay_nop = table.Column<DateTime>(type: "datetime2", nullable: true),
                    nguon_ung_tuyen = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UngViens", x => x.id);
                    table.ForeignKey(
                        name: "FK_UngViens_ViTriTuyenDungs_vi_tri_ung_tuyen_id",
                        column: x => x.vi_tri_ung_tuyen_id,
                        principalTable: "ViTriTuyenDungs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoLuuTrus",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ung_vien_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    vi_tri_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ly_do_luu_tru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ngay_luu = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoLuuTrus", x => x.id);
                    table.ForeignKey(
                        name: "FK_HoSoLuuTrus_UngViens_ung_vien_id",
                        column: x => x.ung_vien_id,
                        principalTable: "UngViens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoSoLuuTrus_ViTriTuyenDungs_vi_tri_id",
                        column: x => x.vi_tri_id,
                        principalTable: "ViTriTuyenDungs",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "LichPhongVans",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    phong_phong_van_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ung_vien_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    vi_tri_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    thoi_gian = table.Column<DateTime>(type: "datetime2", nullable: true),
                    trang_thai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ghi_chu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhongPhongVanId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UngVienMaUngVien = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ViTriTuyenDungMaViTri = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichPhongVans", x => x.id);
                    table.ForeignKey(
                        name: "FK_LichPhongVans_PhongPhongVans_PhongPhongVanId1",
                        column: x => x.PhongPhongVanId1,
                        principalTable: "PhongPhongVans",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_LichPhongVans_PhongPhongVans_phong_phong_van_id",
                        column: x => x.phong_phong_van_id,
                        principalTable: "PhongPhongVans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichPhongVans_UngViens_UngVienMaUngVien",
                        column: x => x.UngVienMaUngVien,
                        principalTable: "UngViens",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_LichPhongVans_UngViens_ung_vien_id",
                        column: x => x.ung_vien_id,
                        principalTable: "UngViens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichPhongVans_ViTriTuyenDungs_ViTriTuyenDungMaViTri",
                        column: x => x.ViTriTuyenDungMaViTri,
                        principalTable: "ViTriTuyenDungs",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_LichPhongVans_ViTriTuyenDungs_vi_tri_id",
                        column: x => x.vi_tri_id,
                        principalTable: "ViTriTuyenDungs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DanhGiaPhongVans",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    lich_phong_van_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    nhan_vien_danh_gia_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    diem_danh_gia = table.Column<int>(type: "int", nullable: true),
                    nhan_xet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    de_xuat = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGiaPhongVans", x => x.id);
                    table.ForeignKey(
                        name: "FK_DanhGiaPhongVans_LichPhongVans_lich_phong_van_id",
                        column: x => x.lich_phong_van_id,
                        principalTable: "LichPhongVans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhGiaPhongVans_NhanViens_nhan_vien_danh_gia_id",
                        column: x => x.nhan_vien_danh_gia_id,
                        principalTable: "NhanViens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhanVienThamGiaPhongVans",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    lich_phong_van_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    nhan_vien_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    vai_tro = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienThamGiaPhongVans", x => x.id);
                    table.ForeignKey(
                        name: "FK_NhanVienThamGiaPhongVans_LichPhongVans_lich_phong_van_id",
                        column: x => x.lich_phong_van_id,
                        principalTable: "LichPhongVans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NhanVienThamGiaPhongVans_NhanViens_nhan_vien_id",
                        column: x => x.nhan_vien_id,
                        principalTable: "NhanViens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaPhongVans_lich_phong_van_id",
                table: "DanhGiaPhongVans",
                column: "lich_phong_van_id");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaPhongVans_nhan_vien_danh_gia_id",
                table: "DanhGiaPhongVans",
                column: "nhan_vien_danh_gia_id");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoLuuTrus_ung_vien_id",
                table: "HoSoLuuTrus",
                column: "ung_vien_id");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoLuuTrus_vi_tri_id",
                table: "HoSoLuuTrus",
                column: "vi_tri_id");

            migrationBuilder.CreateIndex(
                name: "IX_LichPhongVans_phong_phong_van_id",
                table: "LichPhongVans",
                column: "phong_phong_van_id");

            migrationBuilder.CreateIndex(
                name: "IX_LichPhongVans_PhongPhongVanId1",
                table: "LichPhongVans",
                column: "PhongPhongVanId1");

            migrationBuilder.CreateIndex(
                name: "IX_LichPhongVans_ung_vien_id",
                table: "LichPhongVans",
                column: "ung_vien_id");

            migrationBuilder.CreateIndex(
                name: "IX_LichPhongVans_UngVienMaUngVien",
                table: "LichPhongVans",
                column: "UngVienMaUngVien");

            migrationBuilder.CreateIndex(
                name: "IX_LichPhongVans_vi_tri_id",
                table: "LichPhongVans",
                column: "vi_tri_id");

            migrationBuilder.CreateIndex(
                name: "IX_LichPhongVans_ViTriTuyenDungMaViTri",
                table: "LichPhongVans",
                column: "ViTriTuyenDungMaViTri");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_phong_ban_id",
                table: "NguoiDungs",
                column: "phong_ban_id");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_phong_ban_id",
                table: "NhanViens",
                column: "phong_ban_id");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVienThamGiaPhongVans_lich_phong_van_id",
                table: "NhanVienThamGiaPhongVans",
                column: "lich_phong_van_id");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVienThamGiaPhongVans_nhan_vien_id",
                table: "NhanVienThamGiaPhongVans",
                column: "nhan_vien_id");

            migrationBuilder.CreateIndex(
                name: "IX_ThongKeTuyenDungs_vi_tri_id",
                table: "ThongKeTuyenDungs",
                column: "vi_tri_id");

            migrationBuilder.CreateIndex(
                name: "IX_UngViens_vi_tri_ung_tuyen_id",
                table: "UngViens",
                column: "vi_tri_ung_tuyen_id");

            migrationBuilder.CreateIndex(
                name: "IX_ViTriTuyenDungs_phong_ban_id",
                table: "ViTriTuyenDungs",
                column: "phong_ban_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhGiaPhongVans");

            migrationBuilder.DropTable(
                name: "HoSoLuuTrus");

            migrationBuilder.DropTable(
                name: "NguoiDungs");

            migrationBuilder.DropTable(
                name: "NhanVienThamGiaPhongVans");

            migrationBuilder.DropTable(
                name: "ThongKeTuyenDungs");

            migrationBuilder.DropTable(
                name: "LichPhongVans");

            migrationBuilder.DropTable(
                name: "NhanViens");

            migrationBuilder.DropTable(
                name: "PhongPhongVans");

            migrationBuilder.DropTable(
                name: "UngViens");

            migrationBuilder.DropTable(
                name: "ViTriTuyenDungs");

            migrationBuilder.DropTable(
                name: "PhongBans");
        }
    }
}
