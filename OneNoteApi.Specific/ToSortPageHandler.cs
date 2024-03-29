﻿using System;
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

        private ToSortPage GetToSortPage()
        {
            var pageContent = GetPageContent();
            ToSortPage toSortPage = new(pageContent);
            return toSortPage;
        }

        public void AddNewTask(string taskContent)
        {
            var toSortPage = GetToSortPage();
            toSortPage.ToSortTasks.AddNewTask(taskContent);

            // update page -
            _oneNote.PageContentService.UpdatePageContent(toSortPage.Page, true);
        }

        public void ArchiveCompletedTasks()
        {
            var toSortPage = GetToSortPage();
            var taskToRemove = toSortPage.ToSortTasks.Tasks.Where(x => x.IsValid && x.IsCompleted).ToList();
            foreach (var task in taskToRemove)
            {
                toSortPage.ToSortTasks.RemoveTask(task);
                toSortPage.ArcvhiedTasks.AddNewTask(task, true);
            }

            // update page -
            _oneNote.PageContentService.UpdatePageContent(toSortPage.Page, true);
        }

        public void CompleteTask(string taskContent)
        {
            var toSortPage = GetToSortPage();
            var task =  toSortPage.ToSortTasks.Tasks.FirstOrDefault(x=>x.IsValid && !x.IsCompleted && x.Content == taskContent);
            if (task == null)
            {
                return;
            }

            task.IsCompleted = true;

            // update page -
            _oneNote.PageContentService.UpdatePageContent(toSortPage.Page, true);
        }

        public IReadOnlyList<TaskListObject> GetTasks()
        {
            var toSortPage = GetToSortPage();
            return toSortPage.ToSortTasks.Tasks;
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

    internal class ToSortPage
    {
        private TaskListHandler _toSortTasks;
        private TaskListHandler _arcvhiedTasks;
        public readonly Page Page;


        public TaskListHandler ToSortTasks => _toSortTasks ??= GetToSortTasks();

        public TaskListHandler ArcvhiedTasks => _arcvhiedTasks ??= GetArcvhiedTasksTasks();

        private TaskListHandler GetArcvhiedTasksTasks()
        {
            var biggestY = Page.Outlines.OrderByDescending(x => x.Position.Y).FirstOrDefault();
            var listHead = biggestY.Children.First();
            TaskListHandler listHandler = new(listHead);
            return listHandler;
        }

        public ToSortPage(Page page)
        {
            Page = page;
        }

        private TaskListHandler GetToSortTasks()
        {
            var list = GetCurrentTasksListOe(Page);
            TaskListHandler listHandler = new(list);
            return listHandler;
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
    }
}
