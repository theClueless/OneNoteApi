using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneNoteApi.Hierarchy;
using OneNoteApi.PageContent;

namespace OneNoteApi.Mine.SectionCrawler
{
    public class SectionTaskCrawler
    {
        private readonly IOneNote _oneNoteServices;
        private readonly string _sectionId;
        public SectionState State { get; } = new();
        public double AvgTime { get; private set; }
        private int _updateAmount = -1;

        public bool WasUpdatedOnce => _updateAmount > -1;

        public SectionTaskCrawler(IOneNote oneNoteServices, string sectionId)
        {
            _oneNoteServices = oneNoteServices;
            _sectionId = sectionId;
        }

        public void Update()
        {
            var sw = Stopwatch.StartNew();
            var pages2 = _oneNoteServices.PageHierarchyService.GetPages(_sectionId).ToList();

            var changed = pages2.Where(x =>
            {
                if (State.Pages.TryGetValue(x.Id, out var page))
                {
                    return page.LastModifiedTime != x.LastModifiedTime;
                }

                return true;
            }).ToList();
            foreach (var page in changed)
            {
                var content = _oneNoteServices.PageContentService.GetPageContent(page);
                var tasks = GetOeTasksFromPage(content);
                State.Pages[page.Id] = page;
                State.PagesToTasks[page.Id] = tasks;
            }

            sw.Stop();
            if (_updateAmount == -1)
            {
                _updateAmount++;
                return;
            }
            AvgTime = (sw.Elapsed.TotalMilliseconds + AvgTime * _updateAmount++) / _updateAmount;
        }

        private static List<OE> GetOeTasksFromPage(Page page)
        {
            List<OE> pageOe = new List<OE>();
            foreach (var pageOutline in page.Outlines)
            {
                foreach (var pageOutlineChild in pageOutline.Children)
                {
                    GetOeTasksFromPage(pageOutlineChild, pageOe);
                }
            }

            return pageOe;
        }

        private static void GetOeTasksFromPage(OE oe, List<OE> aggregatedList)
        {
            var tag = oe.Tag;
            if (tag.Exists && !tag.IsCompleted && tag.TagType == KnownTags.ToDoTagIndex)
            {
                aggregatedList.Add(oe);
                return;
            }

            if (oe.HasChildren)
            {
                foreach (var child in oe.Children)
                {
                    GetOeTasksFromPage(child, aggregatedList);
                }
            }
        }
    }

    public class SectionState
    {
        /// <summary>
        /// from id to the pages model
        /// </summary>
        public Dictionary<string, PageHierarchyModel> Pages { get; set; } = new();

        public Dictionary<string, List<OE>> PagesToTasks { get; set; } = new();
    }
}
