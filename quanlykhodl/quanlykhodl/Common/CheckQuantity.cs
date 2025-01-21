using quanlykhodl.Models;

namespace quanlykhodl.Common
{
    public static class CheckQuantity
    {
        public static bool checkLocationQuantity(Area area, int location, int quantity, DBContext _context)
        {
            var checkQuantityLocation = _context.locationExceptions.Where(x => x.id_area == area.id && x.location == location && !x.Deleted).FirstOrDefault();
            var checkTotal = _context.productlocations.Where(x => x.id_area == area.id && x.location == location && !x.Deleted).Sum(x => x.quantity);
            if (checkQuantityLocation != null)
            {
                if (checkQuantityLocation.max < checkTotal + quantity)
                    return false;
            }
            else
            {
                var checkArea = _context.areas.Where(x => x.id == area.id && !x.Deleted).FirstOrDefault();
                if (checkArea != null)
                {
                    if (checkArea.max < checkTotal + quantity) return false;
                }
            }

            return true;
        }
    }
}
