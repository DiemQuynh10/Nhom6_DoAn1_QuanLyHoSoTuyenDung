using Microsoft.AspNetCore.Mvc;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    public class BaseController : Controller
    {
        protected bool ChiChoAdminVaHR()
        {
            var role = HttpContext.Session.GetString("VaiTro");
            return role == "admin" || role == "hr";
        }

        protected bool LaNguoiPhongVan()
        {
            var role = HttpContext.Session.GetString("VaiTro");
            return role == "interviewer";
        }
    }
}
