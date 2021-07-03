using System;
using System.Linq;
using OneNoteApi;
using OneNoteApi.Common;
using OneNoteApi.Hierarchy;
using OneNoteApi.Mine;

namespace OneNoteApiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
       //     var one = new OneNote();
       //     var page = one.PageHierarchyService.GetPages("{7DAC32A7-AD8B-FA3F-36C8-FD4777D1896F}{1}{B0}").Skip(3).First();
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

        static void PlayWithTodayUpdater()
        {
            //var todayPageSection = "{7DAC32A7-AD8B-FA3F-36C8-FD4777D1896F}{1}{B0}"; // quick notes
            var todayPageSection = "{2672206C-8D20-4A9B-A381-246FCC6C9622}{1}{B0}"; // team section
            TodayPageCreator creator = new TodayPageCreator(new OneNote(), todayPageSection);
            creator.Create();
            
        }
    }
}
