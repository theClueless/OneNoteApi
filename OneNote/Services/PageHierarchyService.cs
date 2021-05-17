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
        /// <summary>
        /// Gets the specified section and its child page hierarchy when null return all notebooks
        /// </summary>
        /// <param name="id">the id of the notebook\section to get the hierarchy from or null for all notebooks</param>
        /// <returns>A Section element with Page children</returns>
        public IEnumerable<PageHierarchyModel> GetPages(string id = null)
        {
            using var oneNoteRaw = new OneNoteRaw();
            var xml = oneNoteRaw.GetPageHierarchy(id);
            var walker = new PageHierarchyWalker(xml);
            return walker.GetElements();
        }
    }
}