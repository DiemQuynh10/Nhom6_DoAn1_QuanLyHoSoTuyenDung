public class XuatBaoCaoVM
{
    public List<ChartImageVM> ChartImages { get; set; } = new();
    public string? TuKhoa { get; set; }
    public string? LoaiBaoCao { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
}

public class ChartImageVM
{
    public string Id { get; set; }
    public string ImageBase64 { get; set; }
}
