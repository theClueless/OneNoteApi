using OneNoteApi.Hierarchy;

namespace OneNoteApi.Services
{
    public class PageNavigatorService : IOneNoteService
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