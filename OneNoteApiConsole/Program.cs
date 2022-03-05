using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OneNoteApi;
using OneNoteApi.Common;
using OneNoteApi.Hierarchy;
using OneNoteApi.Mine;
using OneNoteApi.Mine.SectionCrawler;
using OneNoteApi.PageContent;

namespace OneNoteApiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TryCrawler();
            var one = new OneNote();
            var sw = Stopwatch.StartNew();
            // get all projects pages
            CrawlerPrep(one, sw);


            //// var toSort = one.PageHierarchyService.GetPages().FirstOrDefault(x => x.Name == "To Sort"); // {1362B548-DA66-0234-1438-D61F6A4096C3}{1}{E17810630182128276241987605806990932121981}
            //var tosortHandler = new ToSortPageHandler(one); //@"{1362B548-DA66-0234-1438-D61F6A4096C3}{1}{E17810630182128276241987605806990932121981}");
            //var page = one.PageContentService.GetPageContent();
            //page.Outlines.First().Children.First().
            //// tosortHandler.AddNewTask("this is another new task");
            //tosortHandler.ArchiveCompletedTasks();

            //var content = one.PageContentService.GetPageContent(toSort);
            //var xml = content.Root;
            //     one.PageHierarchyService.AddPageToSection("{7DAC32A7-AD8B-FA3F-36C8-FD4777D1896F}{1}{B0}", page);
            //int i = 0;

            //var page = one.PageHierarchyService.GetPages().FirstOrDefault(x => x.HierarchyType == HierarchyType.Page);
            //var content = one.PageContentService.GetPageContent(page);

            //foreach (var pageHierarchyModel in one.PageHierarchyService.GetPages())
            //{
            //    i++;
            //    Console.WriteLine($"{pageHierarchyModel.Name}:{pageHierarchyModel.HierarchyType}:{pageHierarchyModel.PageLevel.GetValueOrDefault()}");
            //}

            //Console.WriteLine(i);

            PlayWithTodayUpdater();
        }

        private static void CrawlerPrep(OneNote one, Stopwatch sw)
        {
            var pages = one.PageHierarchyService.GetPages(@"{82AC11DF-DCB7-069E-1E51-00FB2B5D48FF}{1}{B0}").ToList();


            Dictionary<PageHierarchyModel, List<OE>> pageToTasks = new();

            // get all tasks
            foreach (var page in pages)
            {
                var content = one.PageContentService.GetPageContent(page);
                var tasks = GetOeTasksFromPage(content);
                if (tasks.Count > 0)
                {
                }

                pageToTasks[page] = tasks;
            }

            sw.Stop();

            Console.WriteLine($"Got Pages: {pages.Count}");
            Console.WriteLine($"Took: {sw.Elapsed.TotalSeconds}");

            foreach (var (p, task) in pageToTasks)
            {
                if (task.Any())
                {
                    Console.WriteLine($"{p.Name}: {task.Count} tasks");
                    foreach (var oe in task)
                    {
                        Console.WriteLine($"    {oe.Text.Value}");
                    }
                }
            }

            // recheck - 
            Dictionary<string, PageHierarchyModel> idToPages = pages.ToDictionary(x => x.Id);
            var pages2 = one.PageHierarchyService.GetPages(@"{82AC11DF-DCB7-069E-1E51-00FB2B5D48FF}{1}{B0}").ToList();
            var changed = pages2.Where(x => idToPages[x.Id].LastModifiedTime != x.LastModifiedTime).ToList();
            foreach (var page in changed)
            {
                var content = one.PageContentService.GetPageContent(page);
                var tasks = GetOeTasksFromPage(content);
                if (tasks.Count > 0)
                {
                }

                pageToTasks[page] = tasks;
            }
        }

        private static void TryCrawler()
        {
            SectionTaskCrawler crawler = new(new OneNote(), @"{82AC11DF-DCB7-069E-1E51-00FB2B5D48FF}{1}{B0}");
            crawler.Update();
            Console.WriteLine($"First update: {crawler.AvgTime}" );
            int i = 0;
            Stopwatch sw = Stopwatch.StartNew();
            var oe = crawler.State.PagesToTasks.First(x => x.Value.Any()).Value.First();
            while (true)
            {
                sw.Restart();
                crawler.Update();
                sw.Stop();
                Console.WriteLine($"Current: {sw.ElapsedMilliseconds}, Avg Time: {crawler.AvgTime}");
                Thread.Sleep(1000);
                
            }
            
        }

        static List<OE> GetOeTasksFromPage(Page page)
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

        static void GetOeTasksFromPage(OE oe, List<OE> aggregatedList)
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





        static void PlayWithTodayUpdater()
        {
            //var todayPageSection = "{7DAC32A7-AD8B-FA3F-36C8-FD4777D1896F}{1}{B0}"; // quick notes
            //var todayPageSection = "{2672206C-8D20-4A9B-A381-246FCC6C9622}{1}{B0}"; // team section
            //TodayPageCreator creator = new TodayPageCreator(new OneNote(), todayPageSection);
            //creator.Create();
            
        }
    }
}
