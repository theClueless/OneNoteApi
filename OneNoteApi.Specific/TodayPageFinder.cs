using System.Linq;
using OneNoteApi.Hierarchy;

namespace OneNoteApi.Mine
{
    public class TodayPageFinder
    {
        private readonly IOneNote _oneNote;
        private readonly string _sectionId;

        public TodayPageFinder(IOneNote oneNote, string sectionId)
        {
            _oneNote = oneNote;
            _sectionId = sectionId;
        }

        public PageHierarchyModel GetTodayPage()
        {
            return _oneNote.PageHierarchyService.GetPages(_sectionId).First(x => x.Name.Contains("- Today"));
        }
    }
}
