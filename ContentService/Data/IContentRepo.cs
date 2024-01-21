using ContentService.Models;

namespace ContentService.Data
{
    public interface IContentRepo
    {
        bool SaveChanges();
        IEnumerable<Content> GetAllContents();
        Content GetContentById(int id);
        void CreateContent(Content cont);
        public void DeleteContentsByUserId(string Uid);
    }
}
