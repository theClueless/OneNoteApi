using System;
using OneNoteApi;

namespace OneNoteApiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var one = new OneNote();
            int i = 0;
            
            foreach (var pageHierarchyModel in one.PageHierarchyService.GetPages())
            {
                i++;
                Console.WriteLine($"{pageHierarchyModel.Name}:{pageHierarchyModel.HierarchyType}:{pageHierarchyModel.PageLevel.GetValueOrDefault()}");
            }

            Console.WriteLine(i);
        }
    }
}
