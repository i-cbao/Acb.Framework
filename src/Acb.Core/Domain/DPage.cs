
namespace Acb.Core.Domain
{
    /// <summary> 通用分页对象 </summary>
    public class DPage
    {
        private const int MaxPage = 1000;
        private const int MaxSize = 500;
        public int Page { get; private set; }
        public int Size { get; private set; }

        private DPage(int page = 0, int size = 15)
        {
            if (page < 0) page = 0;
            if (page > MaxPage) page = MaxPage;
            if (size < 1) size = 1;
            if (size > MaxSize) size = MaxSize;
            Page = page;
            Size = size;
        }

        public static DPage NewPage(int page = 0, int size = 15)
        {
            return new DPage(page, size);
        }
    }
}
