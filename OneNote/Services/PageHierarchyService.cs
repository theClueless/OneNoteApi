using System.Collections.Generic;
using OneNoteApi.Hierarchy;

namespace OneNoteApi.Services
{
    public interface IPageHierarchyService : IOneNoteService
    {
        IEnumerable<PageHierarchyModel> GetPages(string id = null);
    }

    public class PageHierarchyService : IPageHierarchyService
    {
        public IEnumerable<PageHierarchyModel> GetPages(string id = null)
        {
            using var oneNoteRaw = new OneNoteRaw();
            var xml = oneNoteRaw.GetPageHierarchy(id);
            var walker = new PageHierarchyWalker(xml);
            return walker.GetElements();
        }
    }
}