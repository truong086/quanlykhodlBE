namespace quanlykhodl.Clouds
{
    public class ChuyenFile
    {
        public IFormFile chuyendoi(string data, string fileName)
        {
            var base64Data = data;

            // Chuyển chuỗi Base64 thành byte array
            byte[] fileBytes = Convert.FromBase64String(base64Data);

            var kiemTraBase64 = new KiemTraDinhDangBase64();
            var kiemTra = kiemTraBase64.GetFileTypeByHeader(fileBytes);
            var file = new ChuyenDoiIFormFile(fileBytes, fileName, kiemTra);
            return file;

        }
    }
}
