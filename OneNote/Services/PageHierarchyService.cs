using System.Collections.Generic;
using OneNoteApi.Hierarchy;

namespace OneNoteApi.Services
{
    public interface IPageHierarchyService : IOneNoteService
    {
        /// <summary>
        /// Gets the specified section and its child page hierarchy when null return all notebooks
        /// </summary>
        /// <param name="id">the id of the notebook\section to get the hierarchy from or null for all notebooks</param>
        /// <returns>A Section element with Page children</returns>
        IEnumerable<PageHierarchyModel> GetPages(string id = null);

        /// <summary>
        /// add a new page to section
        /// </summary>
        /// <param name="sectionId">the section Id</param>
        /// <param name="pageBefore">an optional parameter, when specified will add the page after that page, otherwise will add at the end of the section</param>
        void AddPageToSection(string sectionId, PageHierarchyModel pageBefore = null);
    }

    public class PageHierarchyService : IPageHierarchyService
    {
        private readonly IOneNoteRawFactory _factory;

        public PageHierarchyService(IOneNoteRawFactory factory)
        {
            _factory = factory;
        }


        /// <inheritdoc />
        public IEnumerable<PageHierarchyModel> GetPages(string id = null)
        {
            using var oneNoteRaw = _factory.GetNew();
            var xml = oneNoteRaw.GetPageHierarchy(id);
            var walker = new PageHierarchyWalker(xml);
            return walker.GetElements();
        }

        /// <inheritdoc />
        public void AddPageToSection(string sectionId, PageHierarchyModel pageBefore = null)
        {
            using var oneNoteRaw = _factory.GetNew();
            var xml = oneNoteRaw.GetPageHierarchy(sectionId);

        }
    }
}