using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneNoteApi.Services;

namespace OneNoteApi
{
    public class OneNote
    {
        private readonly Lazy<PageHierarchyService> _pageHierarchyService = new();
        private readonly Lazy<PageNavigatorService> _pageNavigatorService = new();

        public PageHierarchyService PageHierarchyService => _pageHierarchyService.Value;
        public PageNavigatorService PageNavigatorService => _pageNavigatorService.Value;
    }
}
