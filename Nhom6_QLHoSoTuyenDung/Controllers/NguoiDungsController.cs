using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models;
using Microsoft.Extensions.Options;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    public class NguoiDungsController : Controller
    {
        private readonly AppDbContext _context;
        private static Dictionary<string, string> _maXacNhanDict = new();
        private readonly EmailSettings _emailSettings;

        public NguoiDungsController(AppDbContext context, IOptions<EmailSettings> emailOptions)
        {
            _context = context;
            _emailSettings = emailOptions.Value;
        }
        // GET: NguoiDungs
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.NguoiDungs.Include(n => n.NhanVien).Include(n => n.PhongBan);
            return View(await appDbContext.ToListAsync());
        }
        [HttpGet]
        public IActionResult DangNhap()
        {
            if (HttpContext.Session.GetString("VaiTro") != null)
                return RedirectToAction("Index", "Home");

            return View(new DangNhapVM());
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(DangNhapVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int soLanSai = HttpContext.Session.GetInt32("SoLanSai") ?? 0;
            if (soLanSai >= 5)
            {
                ModelState.AddModelError("", "Bạn đã nhập sai quá nhiều lần. Vui lòng thử lại sau.");
                return View(model);
            }

            var user = _context.NguoiDungs.FirstOrDefault(u =>
                u.TenDangNhap == model.TenDangNhap &&
                u.MatKhau == model.MatKhau);

            if (user == null)
            {
                HttpContext.Session.SetInt32("SoLanSai", soLanSai + 1);
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            // ✅ Ghi session
            HttpContext.Session.SetInt32("SoLanSai", 0);
            HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
            HttpContext.Session.SetString("VaiTro", user.VaiTro);
            HttpContext.Session.SetString("HoTen", user.HoTen);

            // ✅ Flush Session để đảm bảo được ghi trước khi redirect
            await HttpContext.Session.CommitAsync();  // 🆕 yêu cầu Microsoft.AspNetCore.Session >= 2.1+

            // ✅ Không dùng RedirectToAction nếu muốn chắc chắn giữ session → dùng Redirect với URL tuyệt đối
            var url = Url.Action(
                user.VaiTro switch
                {
                    "Admin" or "HR" => "Index",
                    "Interviewer" => "Index",
                    _ => "DangNhap"
                },
                user.VaiTro switch
                {
                    "Admin" or "HR" => "UngViens",
                    "Interviewer" => "LichPhongVans",
                    _ => "NguoiDungs"
                });

            return Redirect(url!);
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa session nếu có
            return RedirectToAction("DangNhap", "NguoiDungs"); // Chuyển về trang đăng nhập
        }

        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QuenMatKhau(QuenMatKhauVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.TenDangNhap == model.TenDangNhap && u.Email == model.Email);

            if (user == null)
            {
                TempData["Error"] = "Thông tin không khớp. Vui lòng kiểm tra lại.";
                return View(model);
            }

            string ma = new Random().Next(100000, 999999).ToString();

            // ✅ Lưu vào session
            HttpContext.Session.SetString("MaXacNhan", ma);
            HttpContext.Session.SetString("ThoiGianMa", DateTime.Now.ToString("O"));

            try
            {
                var message = new MailMessage(_emailSettings.Mail, user.Email)
                {
                    Subject = "Mã xác nhận khôi phục mật khẩu",
                    Body = $"Mã xác nhận của bạn là: {ma}"
                };

                using var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.Mail, _emailSettings.Password),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(message);
                TempData["Success"] = "Đã gửi mã xác nhận đến email của bạn.";

                // ✅ KHÔNG redirect nữa → tránh mất session
                return View("XacNhanMa", new XacNhanMaVM { TenDangNhap = model.TenDangNhap });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi gửi email: " + ex.Message;
                return View(model);
            }
        }



        [HttpGet]
        public IActionResult XacNhanMa(string tenDangNhap)
        {
            return View("XacNhanMa", new XacNhanMaVM { TenDangNhap = tenDangNhap });
        }

        [HttpPost]
        public IActionResult XacNhanMa(XacNhanMaVM model)
        {
            var ma = HttpContext.Session.GetString("MaXacNhan");
            var thoiGianStr = HttpContext.Session.GetString("ThoiGianMa");

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(thoiGianStr))
            {
                TempData["Error"] = "Không tìm thấy mã xác nhận. Có thể phiên làm việc đã hết hạn.";
                return View(model);
            }

            DateTime thoiGianTao = DateTime.Parse(thoiGianStr);
            if ((DateTime.Now - thoiGianTao).TotalMinutes > 2)
            {
                TempData["Error"] = "Mã xác nhận đã hết hạn. Vui lòng quay lại gửi lại mã mới.";
                return View(model);
            }

            if (model.MaXacNhan != ma)
            {
                TempData["Error"] = "Mã xác nhận không đúng.";
                return View(model);
            }

            // Thành công → xóa session
            HttpContext.Session.Remove("MaXacNhan");
            HttpContext.Session.Remove("ThoiGianMa");

            return RedirectToAction("DatLaiMatKhau", new { tenDangNhap = model.TenDangNhap });
        }


        [HttpPost]
        public async Task<IActionResult> GuiLaiMa(string tenDangNhap)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == tenDangNhap);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("QuenMatKhau");
            }

            string ma = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("MaXacNhan", ma);
            HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
            HttpContext.Session.SetString("ThoiGianMa", DateTime.Now.ToString("O"));

            try
            {
                var message = new MailMessage(_emailSettings.Mail, user.Email)
                {
                    Subject = "Mã xác nhận mới",
                    Body = $"Mã xác nhận mới của bạn là: {ma}"
                };

                using var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.Mail, _emailSettings.Password),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(message);
                TempData["Success"] = "Đã gửi lại mã xác nhận.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi gửi lại mã: " + ex.Message;
            }

            return RedirectToAction("XacNhanMa", new { tenDangNhap });
        }

        [HttpGet]
        public IActionResult DatLaiMatKhau(string tenDangNhap)
        {
            return View(new DatLaiMatKhauVM { TenDangNhap = tenDangNhap });
        }

        [HttpPost]
        public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.TenDangNhap == model.TenDangNhap);

            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản.";
                return View(model);
            }

            // TODO: nếu cần mã hoá thì dùng thư viện băm (Hash password)
            user.MatKhau = model.MatKhauMoi;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại.";
            return RedirectToAction("DangNhap");
        }

        // GET: NguoiDungs/Create
        public IActionResult Create()
        {
            ViewData["NhanVienId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien");
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id");
            return View();
        }

        // POST: NguoiDungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NhanVienId,TenDangNhap,MatKhau,VaiTro,PhongBanId,HoTen,Email,SoDienThoai,NgayTao")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nguoiDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NhanVienId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien", nguoiDung.NhanVienId);
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id", nguoiDung.PhongBanId);
            return View(nguoiDung);
        }

        // GET: NguoiDungs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            ViewData["NhanVienId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien", nguoiDung.NhanVienId);
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id", nguoiDung.PhongBanId);
            return View(nguoiDung);
        }

        // POST: NguoiDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NhanVienId,TenDangNhap,MatKhau,VaiTro,PhongBanId,HoTen,Email,SoDienThoai,NgayTao")] NguoiDung nguoiDung)
        {
            if (id != nguoiDung.NhanVienId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguoiDung);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiDungExists(nguoiDung.NhanVienId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["NhanVienId"] = new SelectList(_context.Set<NhanVien>(), "MaNhanVien", "MaNhanVien", nguoiDung.NhanVienId);
            ViewData["PhongBanId"] = new SelectList(_context.Set<PhongBan>(), "Id", "Id", nguoiDung.PhongBanId);
            return View(nguoiDung);
        }

        // GET: NguoiDungs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.NhanVien)
                .Include(n => n.PhongBan)
                .FirstOrDefaultAsync(m => m.NhanVienId == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // POST: NguoiDungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung != null)
            {
                _context.NguoiDungs.Remove(nguoiDung);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiDungExists(string id)
        {
            return _context.NguoiDungs.Any(e => e.NhanVienId == id);
        }
    }
}
