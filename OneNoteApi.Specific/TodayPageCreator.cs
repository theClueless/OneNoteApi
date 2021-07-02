using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OneNoteApi.Common;
using OneNoteApi.PageContent;

namespace OneNoteApi.Mine
{
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
            var movedItems = FindAndUpdateTodayPage();
            
            //Create a new today page
            CreateNewTodayPage(movedItems);
        }

        private void CreateNewTodayPage(List<OE> movedItems)
        {
            throw new System.NotImplementedException();
        }

        private List<OE> FindAndUpdateTodayPage()
        {
            TodayPageFinder pageFinder = new TodayPageFinder(_oneNote, _todayPageSectionId);
            var today = pageFinder.GetTodayPage();
            
            // get page content
            var todayPage = _oneNote.PageContentService.GetPageContent(today);
            if (!todayPage.IsValid)
            {
                // do something
            }
            
            var res = UpdateTodayPage(todayPage);

            // rename today page

            return res;
        }

        private List<OE> UpdateTodayPage(Page todayPage)
        {
            // Todo section
            var todoOe = FindTodoSection(todayPage);

            var MoveToNewToday = new List<OE>();

            // go over all children and collect unfinished
            foreach (var todoOeChild in todoOe.Children)
            {
                // check if completed 
                var tag = todoOeChild.Tag;
                if (tag.Exists)
                { // has tag
                    if (!tag.IsCompleted)
                    { // collect OE
                        MoveToNewToday.Add(todoOeChild.Clone());
                    }
                }
            }

            // go over all tags in page and update them
            foreach (var xElement in todayPage.Root.Descendants(PageElementTypes.Tag))
            {
                var tag = new Tag(xElement);
                if (tag.TagType == KnownTags.ToDoTagIndex)
                {
                    tag.UpdateTagType(tag.IsCompleted ? KnownTags.HappySmilyTagIndex : KnownTags.SadSmilyTagIndex );
                    tag.Complete();
                }
            }

            // update TagDef
            todayPage.AddOrUpdateTagDefinition(KnownTags.HappySmilyTag);
            todayPage.AddOrUpdateTagDefinition(KnownTags.SadSmilyTag);

            _oneNote.PageContentService.UpdatePageContent(todayPage,true);
            return MoveToNewToday;
        }
         private OE FindTodoSection(Page todayPage)
        {
            var todoText = todayPage.Root.Descendants(PageElementTypes.Text).FirstOrDefault(x => x.Value.Contains("To Do - "));
//            todayPage.Root.Descendants(PageElementTypes.Text).FirstOrDefault(x => x.Value == @"<![CDATA[<span
//style='font-weight:bold'>To Do - </span>]]>");
            var todo = todoText?.Parent;
            return new OE(todo);
        }
    }
}