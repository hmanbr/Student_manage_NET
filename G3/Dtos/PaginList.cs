namespace G3.Dtos
{
    public class PaginList<T> : List<T>
    {
        public int pageIndex { get; private set; }
        public int totalPages{ get; private set; }

        public PaginList(List<T> items, int count, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex;
            totalPages = (int)Math.Ceiling(count/(double)pageSize);
            this.AddRange(items);
        }
        public bool HasPreviewPage
        {
            get
            {
                return pageIndex > 1;
            }
        }
        public bool HasNextPage
        {
            get
            {
                return(pageIndex< totalPages);
            }
        }
        public static async Task<PaginList<T>> Create(IQueryable<T> source, int pageIndex, int pagesSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex -1)*pagesSize).Take(pagesSize).ToListAsync();
            return new PaginList<T> (items, count, pageIndex, pagesSize);
        }
    }
   
}
