using System;
using System.Xml.Linq;
using OneNoteApi.Hierarchy;
using OneNoteApi.PageContent;

namespace OneNoteApi.Services
{
    public interface IPageContentService : IOneNoteService
    {
        Page GetPageContent(PageHierarchyModel model, PageDetail detail = PageDetail.Basic);

        /// <summary>
        /// less preferred method but can save calls to hierarchy service when you already cached the page ID
        /// </summary>
        /// <param name="pageId">id of the page</param>
        /// <param name="detail">the amount of detail needed</param>
        /// <returns>the page</returns>
        Page GetPageContent(string pageId, PageDetail detail = PageDetail.Basic);

        void UpdatePageContent(Page page, bool force = false);
    }

    public class PageContentService : IPageContentService
    {
        private readonly IOneNoteRawFactory _factory;

        public PageContentService(IOneNoteRawFactory factory)
        {
            _factory = factory;
        }

        public Page GetPageContent(PageHierarchyModel model, PageDetail detail = PageDetail.Basic)
        {
            return GetPageContent(model.Id, detail);
        }

        public Page GetPageContent(string pageId, PageDetail detail = PageDetail.Basic)
        {
            using var api = _factory.GetNew();
            var xml = api.GetPageContent(pageId, detail);
            return new Page(xml);
        }


        public void UpdatePageContent(Page page, bool force = false)
        {
            using var api = _factory.GetNew();
            var content = page.Root.ToString(SaveOptions.DisableFormatting);
            api.UpdatePageContent(content, force, DateTime.MinValue);
        }
    }
}