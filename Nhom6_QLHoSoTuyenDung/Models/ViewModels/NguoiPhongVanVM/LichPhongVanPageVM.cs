namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM
{
    public class LichPhongVanPageVM
    {
        public LichPhongVanVM? LichGanNhat { get; set; }
        public List<LichPhongVanVM> LichTreHen { get; set; } = new();
        public List<LichPhongVanVM> ConLai { get; set; } = new();
    }

}
