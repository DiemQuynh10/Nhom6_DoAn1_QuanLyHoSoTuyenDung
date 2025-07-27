document.addEventListener("DOMContentLoaded", function () {
    veTatCaBieuDo();
});

// ==================== TỔNG HỢP VẼ TẤT CẢ BIỂU ĐỒ ====================
function veTatCaBieuDo() {
    const { tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId } = layBoLoc();
    const loaiBaoCao = document.querySelector('select[name="loaiBaoCao"]')?.value || "";

    // Điều kiện hiển thị từng biểu đồ dựa vào lựa chọn
    const shouldShow = (loai) => loaiBaoCao === "" || loaiBaoCao === loai;

    if (shouldShow("trangthai")) {
        veBieuDo("/ThongKe/BieuDoTrangThaiUngVien", "chartTrangThai", "bar", "Ứng viên theo trạng thái", { tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId });
    }
    if (shouldShow("nguon")) {
        veBieuDo("/ThongKe/BieuDoNguonUngVien", "chartNguon", "pie", "Ứng viên theo nguồn", { tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId });
    }
    if (shouldShow("vitri")) {
        veBieuDo("/ThongKe/BieuDoTheoViTri", "chartViTri", "bar", "Ứng viên theo vị trí", { tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId });
    }
    if (shouldShow("phongban")) {
        veBieuDo("/ThongKe/BieuDoTheoPhongBan", "chartPhongBan", "bar", "Ứng viên theo phòng ban", { tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId });
    }

    // Luôn luôn hiển thị:
    veBieuDo("/ThongKe/BieuDoDiemDanhGia", "chartDanhGia", "bar", "Phân loại điểm đánh giá ứng viên", { tuKhoa, tuNgay, denNgay });
    veBieuDo("/ThongKe/BieuDoXuHuongNopHoSo", "chartXuHuongThang", "line", "Xu hướng nộp hồ sơ theo tháng", { tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId }, true);
}

// ==================== VẼ BIỂU ĐỒ CHUNG ====================
function veBieuDo(apiUrl, canvasId, chartType, chartTitle, params, isLine = false) {
    const url = taoUrl(apiUrl, params);

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

// ==================== TẠO URL VỚI PARAMS ====================
function taoUrl(apiUrl, params) {
    const query = new URLSearchParams();
    for (let key in params) {
        if (params[key]) query.append(key, params[key]);
    }
    return `${apiUrl}?${query.toString()}`;
}

// ==================== LẤY BỘ LỌC ====================
function layBoLoc() {
    return {
        tuKhoa: document.querySelector('input[name="tuKhoa"]')?.value || "",
        tuNgay: document.querySelector('input[name="tuNgay"]')?.value || "",
        denNgay: document.querySelector('input[name="denNgay"]')?.value || "",
        trangThai: document.querySelector('select[name="trangThai"]')?.value || "",
        viTriId: document.querySelector('select[name="viTriId"]')?.value || "",
        phongBanId: document.querySelector('select[name="phongBanId"]')?.value || ""
    };
}

// ==================== MÀU NGẪU NHIÊN ====================
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

document.getElementById("btnExportExcel").addEventListener("click", async () => {
    const getValue = selector => {
        const value = document.querySelector(selector)?.value;
        return value && value.trim() !== "" ? value : null;
    };

    const request = {
        tuKhoa: getValue("input[name='tuKhoa']"),
        loaiBaoCao: getValue("select[name='loaiBaoCao']"),
        trangThai: getValue("select[name='trangThai']"),
        viTriId: getValue("select[name='viTriId']"),
        phongBanId: getValue("select[name='phongBanId']"),
        tuNgay: getValue("input[name='tuNgay']"),
        denNgay: getValue("input[name='denNgay']")
    };


    const response = await fetch("/ThongKe/XuatBaoCaoDayDu", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(request)
    });

    if (response.ok) {
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = `BaoCaoUngVien_${new Date().toISOString().slice(0, 10)}.xlsx`;
        document.body.appendChild(a);
        a.click();
        a.remove();
    } else {
        const errorText = await response.text();
        alert("❌ " + errorText);
    }

});

