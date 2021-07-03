using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OneNoteApi.Common;
using OneNoteApi.Hierarchy;
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
            // to sort page
            var toSortPage = _oneNote.PageHierarchyService.GetPages(_todayPageSectionId).FirstOrDefault(x => x.Name == "To Sort");
            var newPageId = _oneNote.PageHierarchyService.AddPageToSection(_todayPageSectionId, toSortPage);

            var newPage = _oneNote.PageHierarchyService.GetPages(_todayPageSectionId).FirstOrDefault(x => x.Id == newPageId);
            
            UpdateNewTodayPage(newPage, movedItems);
        }

        private void UpdateNewTodayPage(PageHierarchyModel newPage, List<OE> movedItems)
        {
            // get page content
            var newPageContent = _oneNote.PageContentService.GetPageContent(newPage);
            var todoSection = FindTodoSection(newPageContent);

            // add stuff to page + and rename it to be the new today
            foreach (var oe in movedItems)
            {
                todoSection.RawXml.Element(PageElementTypes.OeChildren).Add(oe.RawXml);
            }

            // 30/6/2021 - Today
            var newName = DateTime.Today.ToString("dd/MM/yyyy") + " "+ TodayPageFinder.TodayTitle;
            RenamePageAndTitle(newPageContent, newName);

            _oneNote.PageContentService.UpdatePageContent(newPageContent, true);
        }

        private List<OE> FindAndUpdateTodayPage()
        {
            var pageFinder = new TodayPageFinder(_oneNote, _todayPageSectionId);
            var today = pageFinder.GetTodayPage();

            // get page content
            var todayPage = _oneNote.PageContentService.GetPageContent(today);

            var res = UpdateTodayPage(todayPage);

            return res;
        }

        private List<OE> UpdateTodayPage(Page todayPage)
        {
            // Todo section
            var todoOe = FindTodoSection(todayPage);

            var moveToNewToday = CollectUnfinishedTasks(todoOe);

            ConvertAndUpdateAllTags(todayPage);

            RenameTodayPage(todayPage);

            // update page after changes
            _oneNote.PageContentService.UpdatePageContent(todayPage, true);

            return moveToNewToday;
        }

        private void RenameTodayPage(Page todayPage)
        {
            // change name 
            var name = todayPage.Name;
            var newName = name.Replace(TodayPageFinder.TodayTitle, string.Empty);
            RenamePageAndTitle(todayPage, newName);
        }

        private void RenamePageAndTitle(Page page, string newName)
        {
            // change name 
            page.Name = newName;

            // change title
            var titleText = page.Title.Content.Text;
            titleText.Value = newName;
        }

        private static List<OE> CollectUnfinishedTasks(OE todoOe)
        {
            var moveToNewToday = new List<OE>();

            // go over all children and collect unfinished
            foreach (var todoOeChild in todoOe.Children)
            {
                // check if completed 
                var tag = todoOeChild.Tag;
                if (tag.Exists)
                {
                    // has tag
                    if (!tag.IsCompleted)
                    {
                        // collect OE
                        moveToNewToday.Add(todoOeChild.Clone());
                    }
                }
            }

            return moveToNewToday;
        }

        private static void ConvertAndUpdateAllTags(Page todayPage)
        {
            // go over all tags in page and update them
            foreach (var xElement in todayPage.Root.Descendants(PageElementTypes.Tag))
            {
                var tag = new Tag(xElement);
                if (tag.TagType == KnownTags.ToDoTagIndex)
                {
                    tag.UpdateTagType(tag.IsCompleted ? KnownTags.HappySmilyTagIndex : KnownTags.SadSmilyTagIndex);
                    tag.Complete();
                }
            }

            // update TagDef
            todayPage.AddOrUpdateTagDefinition(KnownTags.HappySmilyTag);
            todayPage.AddOrUpdateTagDefinition(KnownTags.SadSmilyTag);
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