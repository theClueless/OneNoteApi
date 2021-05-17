using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneNoteApi.Services;

namespace OneNoteApi
{
    /// <summary>
    /// Just a wrapper entry for the different services of the API
    /// </summary>
    public class OneNote
    {
        private readonly Lazy<IPageHierarchyService> _pageHierarchyService = new();
        private readonly Lazy<IPageNavigatorService> _pageNavigatorService = new();

        public IPageHierarchyService PageHierarchyService => _pageHierarchyService.Value;
        public IPageNavigatorService PageNavigatorService => _pageNavigatorService.Value;
    }
}
