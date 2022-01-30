using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OneNoteApi.PageContent;
using OneNoteApi.PageContent.Builders;

namespace OneNoteApi.Mine
{
    public class ToSortPageHandler
    {
        private readonly IOneNote _oneNote;
        private string _toSortPageId;

        public ToSortPageHandler(IOneNote oneNote, string toSortPageId = null)
        {
            _oneNote = oneNote;
            _toSortPageId = toSortPageId;
        }

        public void AddNewTask(string taskContent)
        {
            var pageContent = GetPageContent();

            var list = GetCurrentTasksListOe(pageContent);

            TaskListHandler listHandler = new (list);
            listHandler.AddNewTask(taskContent);
            
            // update page -
            _oneNote.PageContentService.UpdatePageContent(pageContent, true);
        }

        private OE GetCurrentTasksListOe(Page pageContent)
        {
            // find outline closest to start
            var outline = FindClosestToPageStart(pageContent.Outlines);
            var listHead = outline.Children.First();
            return listHead;
        }

        private Outline FindClosestToPageStart(IEnumerable<Outline> outlines)
        {
            return outlines.OrderBy(x => x.Position.X + x.Position.Y).FirstOrDefault();
        }

        private string GetPageId()
        {
            if (_toSortPageId == null)
            {
                _toSortPageId = _oneNote.PageHierarchyService.GetPages().FirstOrDefault(x => x.Name == "To Sort")?.Id; // {1362B548-DA66-0234-1438-D61F6A4096C3}{1}{E17810630182128276241987605806990932121981}
                if (_toSortPageId == null)
                {
                    throw new Exception("Failed to find to sort page ID");
                }
            }

            return _toSortPageId;
        }

        private Page GetPageContent()
        {
            // get page content
            var pageId = GetPageId();
            var pages = _oneNote.PageHierarchyService.GetPages(pageId).ToList();
            if (pages.Count != 1)
            {
                // something bad something something
            }

            var pageModel = pages.First();
            var pageContent = _oneNote.PageContentService.GetPageContent(pageModel);
            return pageContent;
        }
    }
}
