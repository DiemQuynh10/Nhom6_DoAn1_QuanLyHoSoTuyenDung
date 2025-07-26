document.addEventListener("DOMContentLoaded", function () {
    veTatCaBieuDo();

    const exportBtn = document.getElementById("btnExportExcel");
    if (exportBtn) {
        exportBtn.addEventListener("click", xuatBaoCaoDayDu);
    }
});

// Hàm tổng gọi tất cả biểu đồ
function veTatCaBieuDo() {
    const { tuKhoa, tuNgay, denNgay } = layBoLoc();

    veBieuDo("/ThongKe/BieuDoTrangThaiUngVien", "chartTrangThai", "bar", "Ứng viên theo trạng thái", tuKhoa, tuNgay, denNgay);
    veBieuDo("/ThongKe/BieuDoNguonUngVien", "chartNguon", "pie", "Ứng viên theo nguồn", tuKhoa, tuNgay, denNgay);
    veBieuDo("/ThongKe/BieuDoTheoViTri", "chartViTri", "bar", "Ứng viên theo vị trí", tuKhoa, tuNgay, denNgay);
    veBieuDo("/ThongKe/BieuDoTheoPhongBan", "chartPhongBan", "bar", "Ứng viên theo phòng ban", tuKhoa, tuNgay, denNgay);
    veBieuDo("/ThongKe/BieuDoDiemDanhGia", "chartDanhGia", "bar", "Phân loại điểm đánh giá ứng viên", tuKhoa, tuNgay, denNgay);
    veBieuDo("/ThongKe/BieuDoXuHuongNopHoSo", "chartXuHuongThang", "line", "Xu hướng nộp hồ sơ theo tháng", tuKhoa, tuNgay, denNgay, true);
}

// Hàm vẽ biểu đồ chung
function veBieuDo(apiUrl, canvasId, chartType, chartTitle, tuKhoa, tuNgay, denNgay, isLine = false) {
    const url = `${apiUrl}?tuKhoa=${encodeURIComponent(tuKhoa)}&tuNgay=${tuNgay}&denNgay=${denNgay}`;
    fetch(url)
        .then(res => res.json())
        .then(data => {
            const labels = data.map(x => x.ten);
            const values = data.map(x => x.soLuong);

            new Chart(document.getElementById(canvasId), {
                type: chartType,
                data: {
                    labels: labels,
                    datasets: [{
                        label: "Số lượng",
                        data: values,
                        backgroundColor: isLine ? undefined : getMauNgauNhien(values.length),
                        borderColor: isLine ? "#3e95cd" : undefined,
                        fill: !isLine,
                        tension: isLine ? 0.4 : 0
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        title: {
                            display: true,
                            text: chartTitle
                        }
                    }
                }
            });
        });
}

// Hàm lấy bộ lọc từ input
function layBoLoc() {
    const tuKhoa = document.querySelector('input[name="tuKhoa"]')?.value || "";
    const tuNgay = document.querySelector('input[name="tuNgay"]')?.value || "";
    const denNgay = document.querySelector('input[name="denNgay"]')?.value || "";
    return { tuKhoa, tuNgay, denNgay };
}

// Màu ngẫu nhiên dịu nhẹ
function getMauNgauNhien(soLuong) {
    const colors = [];
    for (let i = 0; i < soLuong; i++) {
        const r = Math.floor(Math.random() * 150) + 50;
        const g = Math.floor(Math.random() * 150) + 50;
        const b = Math.floor(Math.random() * 150) + 50;
        colors.push(`rgba(${r}, ${g}, ${b}, 0.7)`);
    }
    return colors;
}

// Gửi dữ liệu export về controller
function xuatBaoCaoDayDu() {
    const chartIds = [
        "chartTrangThai",
        "chartNguon",
        "chartViTri",
        "chartPhongBan",
        "chartDanhGia",
        "chartXuHuongThang"
    ];

    const chartImages = chartIds.map(id => {
        const canvas = document.getElementById(id);
        return canvas ? {
            id: id,
            imageBase64: canvas.toDataURL("image/png")
        } : null;
    }).filter(x => x !== null);

    const tuKhoa = document.querySelector('input[name="tuKhoa"]')?.value;
    const loaiBaoCao = document.querySelector('select[name="loaiBaoCao"]')?.value;
    const tuNgay = document.querySelector('input[name="tuNgay"]')?.value;
    const denNgay = document.querySelector('input[name="denNgay"]')?.value;

    fetch("/ThongKe/XuatBaoCaoDayDu", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            chartImages: chartImages,
            tuKhoa: tuKhoa,
            loaiBaoCao: loaiBaoCao,
            tuNgay: tuNgay || null,
            denNgay: denNgay || null
        })
    })

        .then(response => response.blob())
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url;
            a.download = `BaoCaoDayDu_${new Date().toISOString().slice(0, 10)}.xlsx`;
            a.click();
            a.remove();
        })
        .catch(err => console.error("Export failed:", err));
}
