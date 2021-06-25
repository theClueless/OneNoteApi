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

    public class TodayPageCreator
    {
        private readonly IOneNote _oneNote;
        private readonly string _todayPageSectionId;

        public TodayPageCreator(IOneNote oneNote, string todayPageSectionId)
        {
            _oneNote = oneNote;
            _todayPageSectionId = todayPageSectionId;
        }

        public void Create()
        {
            // Find the last today page - rename it.
            TodayPageFinder pageFinder = new TodayPageFinder(_oneNote, _todayPageSectionId);
            var today = pageFinder.GetTodayPage();

            

            //Create a new today page

        }
    }
}
