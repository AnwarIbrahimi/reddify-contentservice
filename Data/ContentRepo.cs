using ContentService.Models;

namespace ContentService.Data
{
    public class ContentRepo : IContentRepo
    {
        private readonly AppDbContext _context;

        public ContentRepo(AppDbContext context)
        {
            _context = context;
        }

        public void CreateContent(Content cont)
        {
            if (cont == null)
            {
                throw new ArgumentNullException(nameof(cont));
            }
            _context.Contents.Add(cont);
        }

        public IEnumerable<Content> GetAllContents()
        {
            return _context.Contents.ToList();
        }

        public Content GetContentById(int id)
        {
            return _context.Contents.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
