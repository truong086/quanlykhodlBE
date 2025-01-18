using quanlykhodl.ViewModel;

namespace quanlykhodl.Common
{
    public static class TinhPhanTram
    {
        public static decimal tinhPhanTramData(decimal sotien, decimal phantram)
        {
            if (sotien < 0 || phantram < 0)
                throw new Exception(Status.DATANULL);

            decimal tinh = 1 - (phantram / 100);
            decimal total = sotien * tinh;

            return total;
        }
    }
}
