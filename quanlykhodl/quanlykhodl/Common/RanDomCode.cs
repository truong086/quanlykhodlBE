using quanlykhodl.ViewModel;

namespace quanlykhodl.Common
{
    public static class RanDomCode
    {
        public static string geneAction(int length)
        {
            var random = new Random();
            string code = Status.RANDOMCODE;
            var geneCode = new string(Enumerable.Repeat(code, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return geneCode;
        }
    }
}
