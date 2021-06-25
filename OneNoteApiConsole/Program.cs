using System;
using System.Linq;
using OneNoteApi;
using OneNoteApi.Common;
using OneNoteApi.Hierarchy;

namespace OneNoteApiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var one = new OneNote();
            int i = 0;

            var page = one.PageHierarchyService.GetPages().FirstOrDefault(x => x.HierarchyType == HierarchyType.Page);
            var content = one.PageContentService.GetPageContent(page);

            foreach (var pageHierarchyModel in one.PageHierarchyService.GetPages())
            {
                i++;
                Console.WriteLine($"{pageHierarchyModel.Name}:{pageHierarchyModel.HierarchyType}:{pageHierarchyModel.PageLevel.GetValueOrDefault()}");
            }

            Console.WriteLine(i);
        }
    }
}
