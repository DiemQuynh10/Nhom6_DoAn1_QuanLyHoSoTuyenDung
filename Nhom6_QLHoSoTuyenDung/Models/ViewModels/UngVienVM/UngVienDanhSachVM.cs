using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Entities; 

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.UngVienVM
{
    public class UngVienDanhSachVM
    {
        public IEnumerable<Models.Entities.UngVien> DanhSach { get; set; } = new List<Models.Entities.UngVien>();
        public BoLocViewModel BoLoc { get; set; } = new BoLocViewModel();
    }
}
