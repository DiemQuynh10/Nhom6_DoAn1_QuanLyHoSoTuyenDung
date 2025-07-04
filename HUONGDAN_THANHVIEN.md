# 📘 HƯỚNG DẪN LÀM VIỆC VỚI GITHUB DÀNH CHO CÁC THÀNH VIÊN NHÓM 6

## ✅ 1. Clone Project (nếu chưa làm)

git clone https://github.com/DiemQuynh10/Nhom6_DoAn1_QuanLyHoSoTuyenDung.git
cd Nhom6_DoAn1_QuanLyHoSoTuyenDung


## ✅ 2. Nếu đã clone từ trước → cập nhật các nhánh mới

---
git fetch origin
---
→ Kiểm tra có các nhánh `feature/minh`, `feature/quynh`, `feature/van`, `dev`, `main`.



## ✅ 3. Chuyển sang đúng nhánh của bạn để làm việc

| Thành viên               | Nhánh làm việc         |
|--------------------------|------------------------|
| Đỗ Công Minh             | `feature/minh`         |
| Đinh Thị Diễm Quỳnh      | `feature/quynh`        |
| Trần Thị Thanh Vân       | `feature/van`          |

🔸 Ví dụ: Quỳnh làm thì chạy:

---
git checkout feature/quynh
---

## ✅ 4. Sau khi sửa code → đẩy lên GitHub

---
git add .
git commit -m "Mô tả ngắn về phần bạn đã làm"
git push origin feature/tenban
---

📌 Thay `tenban` bằng `quynh`, `van`, hoặc `minh`.


## ✅ 5. Tạo Pull Request (PR)

1. Lên GitHub repo
2. Chọn tab **Pull Requests → New Pull Request**
3. Chọn:
   - **base:** `dev`
   - **compare:** `feature/tenban`
4. Nhấn **Create Pull Request**

Trưởng nhóm sẽ kiểm tra và hợp nhất vào nhánh `dev`.

---

## 🚨 Lưu ý

- ❌ Không làm việc trực tiếp trên `main`
- ✅ Luôn `checkout` đúng nhánh trước khi code
- ✅ Nếu repo có thay đổi, hãy cập nhật:
---
git pull origin dev
git merge dev
---


**🎯 Chúc các bạn làm việc hiệu quả!**
