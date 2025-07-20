using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nhom6_QLHoSoTuyenDung.Migrations
{
    /// <inheritdoc />
    public partial class themThuocTinhTrongDanhGiaPV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThongKeTuyenDungs");

            migrationBuilder.AlterColumn<string>(
                name: "sdt",
                table: "UngViens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "link_cv",
                table: "UngViens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "UngViens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<float>(
                name: "diem_danh_gia",
                table: "DanhGiaPhongVans",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "giai_quyet_van_de",
                table: "DanhGiaPhongVans",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "giao_tiep",
                table: "DanhGiaPhongVans",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ky_nang_chuyen_mon",
                table: "DanhGiaPhongVans",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ngay_danh_gia",
                table: "DanhGiaPhongVans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "thai_do_lam_viec",
                table: "DanhGiaPhongVans",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "tinh_than_hoc_hoi",
                table: "DanhGiaPhongVans",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "giai_quyet_van_de",
                table: "DanhGiaPhongVans");

            migrationBuilder.DropColumn(
                name: "giao_tiep",
                table: "DanhGiaPhongVans");

            migrationBuilder.DropColumn(
                name: "ky_nang_chuyen_mon",
                table: "DanhGiaPhongVans");

            migrationBuilder.DropColumn(
                name: "ngay_danh_gia",
                table: "DanhGiaPhongVans");

            migrationBuilder.DropColumn(
                name: "thai_do_lam_viec",
                table: "DanhGiaPhongVans");

            migrationBuilder.DropColumn(
                name: "tinh_than_hoc_hoi",
                table: "DanhGiaPhongVans");

            migrationBuilder.AlterColumn<string>(
                name: "sdt",
                table: "UngViens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "link_cv",
                table: "UngViens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "UngViens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "diem_danh_gia",
                table: "DanhGiaPhongVans",
                type: "int",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ThongKeTuyenDungs",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    vi_tri_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    so_luong_dat = table.Column<int>(type: "int", nullable: true),
                    so_luong_truot = table.Column<int>(type: "int", nullable: true),
                    so_luong_ung_vien = table.Column<int>(type: "int", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_ThongKeTuyenDungs_vi_tri_id",
                table: "ThongKeTuyenDungs",
                column: "vi_tri_id");
        }
    }
}
