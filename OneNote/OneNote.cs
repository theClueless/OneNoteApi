using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OneNoteApi.Services;

namespace OneNoteApi
{
    /// <summary>
    /// Just a wrapper entry for the different services of the API
    /// </summary>
    public class OneNote : IOneNote
    {
        private readonly Lazy<IPageHierarchyService> _pageHierarchyService = new Lazy<IPageHierarchyService>(() => new PageHierarchyService(), LazyThreadSafetyMode.PublicationOnly);
        private readonly Lazy<IPageNavigatorService> _pageNavigatorService = new Lazy<IPageNavigatorService>(() => new PageNavigatorService(), LazyThreadSafetyMode.PublicationOnly);

        public IPageHierarchyService PageHierarchyService => _pageHierarchyService.Value;
        public IPageNavigatorService PageNavigatorService => _pageNavigatorService.Value;
    }

    public interface IOneNote
    {
        public IPageHierarchyService PageHierarchyService {get;}
        public IPageNavigatorService PageNavigatorService { get; }
    }
}
