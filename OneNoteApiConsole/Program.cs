using System;
using OneNoteApi;

namespace OneNoteApiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var one = new OneNote())
            {
                Console.WriteLine(one.CurrentPageId);
            }
        }
    }
}
