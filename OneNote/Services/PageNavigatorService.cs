using OneNoteApi.Hierarchy;

namespace OneNoteApi.Services
{
    public interface IPageNavigatorService : IOneNoteService
    {
        void NavigateTo(PageHierarchyModel model);
        void NavigateTo(string pageId, string elementId = null, bool shouldOpenNewWindow = false);
    }

    public class PageNavigatorService : IPageNavigatorService
    {
        private readonly IOneNoteRawFactory _oneNoteRawFactory;

        public PageNavigatorService(IOneNoteRawFactory oneNoteRawFactory)
        {
            _oneNoteRawFactory = oneNoteRawFactory;
        }

        public void NavigateTo(PageHierarchyModel model)
        {
            NavigateTo(model.Id);
        }

        public void NavigateTo(string pageId, string elementId = null, bool shouldOpenNewWindow = false)
        {
            using var oneNoteRaw = _oneNoteRawFactory.GetNew();
            oneNoteRaw.NavigateTo(pageId, elementId, shouldOpenNewWindow);
        }
    }
}