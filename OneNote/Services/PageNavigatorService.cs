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
        public void NavigateTo(PageHierarchyModel model)
        {
            NavigateTo(model.Id);
        }

        public void NavigateTo(string pageId, string elementId = null, bool shouldOpenNewWindow = false)
        {
            using var oneNoteRaw = new OneNoteRaw();
            oneNoteRaw.NavigateTo(pageId, elementId, shouldOpenNewWindow);
        }
    }
}