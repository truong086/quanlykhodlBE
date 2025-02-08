using quanlykhodl.Models;

namespace quanlykhodl.Common
{
    public static class CheckQuantity
    {
        public static bool checkLocationQuantity(Shelf area, int location, int quantity, DBContext _context)
        {
            var checkQuantityLocation = _context.locationexceptions.Where(x => x.id_shelf == area.id && x.location == location && !x.deleted).FirstOrDefault();
            var checkTotal = _context.productlocations.Where(x => x.id_shelf == area.id && x.location == location && !x.deleted && x.isaction).Sum(x => x.quantity);
            if (checkQuantityLocation != null)
            {
                if (checkQuantityLocation.max < checkTotal + quantity)
                    return false;
            }
            else
            {
                //var checkArea = _context.shelfs.Where(x => x.id == shelfs.id && !x.deleted).FirstOrDefault();
                if (area.max < checkTotal + quantity) return false;
            }

            return true;
        }
    }
}
