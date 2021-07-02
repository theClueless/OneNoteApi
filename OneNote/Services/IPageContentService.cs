using System.Xml.Linq;
using OneNoteApi.Hierarchy;
using OneNoteApi.PageContent;

namespace OneNoteApi.Services
{
    public interface IPageContentService : IOneNoteService
    {
        Page GetPageContent(PageHierarchyModel model, PageDetail detail = PageDetail.Basic);

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
            using var api = _factory.GetNew();
            var xml = api.GetPageContent(model.Id, detail);
            return new Page(xml);
        }

        public void UpdatePageContent(Page page, bool force = false)
        {
            using var api = _factory.GetNew();
            var content = page.Root.ToString(SaveOptions.DisableFormatting);
            api.UpdatePageContent(content, force);
        }
    }
}