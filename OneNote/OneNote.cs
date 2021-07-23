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
        private readonly Lazy<IPageHierarchyService> _pageHierarchyService = new(() => new PageHierarchyService(new OneNoteRawFactory()), LazyThreadSafetyMode.PublicationOnly);
        private readonly Lazy<IPageNavigatorService> _pageNavigatorService = new(() => new PageNavigatorService(new OneNoteRawFactory()), LazyThreadSafetyMode.PublicationOnly);
        private readonly Lazy<IPageContentService> _pageContentService = new(() => new PageContentService(new OneNoteRawFactory()),  LazyThreadSafetyMode.PublicationOnly);

        public IPageHierarchyService PageHierarchyService => _pageHierarchyService.Value;
        public IPageNavigatorService PageNavigatorService => _pageNavigatorService.Value;

        public IPageContentService PageContentService => _pageContentService.Value;
    }

    public interface IOneNote
    {
        public IPageHierarchyService PageHierarchyService {get;}
        public IPageNavigatorService PageNavigatorService { get; }
        public IPageContentService PageContentService { get; }

    }
}
