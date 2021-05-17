using System.Collections.Generic;
using OneNoteApi.Hierarchy;

namespace OneNoteApi.Services
{
    public class PageHierarchyService : IOneNoteService
    {
        public IEnumerable<PageHierarchyModel> GetElements(string id = null)
        {
            using var oneNoteRaw = new OneNoteRaw();
            var xml = oneNoteRaw.GetPageHierarchy(id);
            var walker = new PageHierarchyWalker(xml);
            return walker.GetElements();
        }
    }
}