using System;
using OneNoteApi;

namespace OneNoteApiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var one = new OneNote();
            foreach (var pageHierarchyModel in one.PageHierarchyService.GetPages())
            {
                Console.WriteLine($"{pageHierarchyModel.Name}:{pageHierarchyModel.HierarchyType}:{pageHierarchyModel.PageLevel.GetValueOrDefault()}");
            }
        }
    }
}
