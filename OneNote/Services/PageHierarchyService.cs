using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OneNoteApi.Common;
using OneNoteApi.Hierarchy;

namespace OneNoteApi.Services
{
    public interface IPageHierarchyService : IOneNoteService
    {
        /// <summary>
        /// Gets the specified section and its child page hierarchy when null return all notebooks
        /// </summary>
        /// <param name="id">the id of the notebook\section to get the hierarchy from or null for all notebooks</param>
        /// <returns>A Section element with Page children</returns>
        IEnumerable<PageHierarchyModel> GetPages(string id = null);

        /// <summary>
        /// add a new page to section
        /// </summary>
        /// <param name="sectionId">the section Id</param>
        /// <param name="pageBefore">an optional parameter, when specified will add the page after that page, otherwise will add at the end of the section</param>
        string AddPageToSection(string sectionId, PageHierarchyModel pageBefore = null);
    }

    public class PageHierarchyService : IPageHierarchyService
    {
        private readonly IOneNoteRawFactory _factory;

        public PageHierarchyService(IOneNoteRawFactory factory)
        {
            _factory = factory;
        }


        /// <inheritdoc />
        public IEnumerable<PageHierarchyModel> GetPages(string id = null)
        {
            using var oneNoteRaw = _factory.GetNew();
            var xml = oneNoteRaw.GetPageHierarchy(id);
            var walker = new PageHierarchyWalker(xml);
            return walker.GetElements();
        }

        /// <inheritdoc />
        public string AddPageToSection(string sectionId, PageHierarchyModel pageBefore = null)
        {
            using var oneNoteRaw = _factory.GetNew();
            var id = oneNoteRaw.CreateNewPage(sectionId);

            if (pageBefore == null)
            { // we are done
                return id;
            }

            var xml = oneNoteRaw.GetPageHierarchy(sectionId);
            // need to find last page and add the new page there
            PageHierarchyModel previousPage = null;
            var walker = new PageHierarchyWalker(xml);
            foreach (var page in walker.GetElements())
            {
                if (page.Id == pageBefore.Id)
                { // we found it!
                    previousPage = page;
                    break;
                }
            }

            if (previousPage == null)
            {
                throw new Exception("failed to find the page before");
            }

            var theNewPage = xml.Elements().Last();
            theNewPage.Remove();
            previousPage.XML.AddAfterSelf(theNewPage);


            oneNoteRaw.UpdateHierarchy(xml.ToString(SaveOptions.DisableFormatting));
            return id;
        }

        //private XElement CreateNewPageForSection(string sectionId, string name)
        //{

        //    var pageId = new IdGenerator().GeneratePageId(sectionId);

        //    var xml = new XElement(HierarchyElementTypes.Page,
        //        new XAttribute(KnownAttributes.IDAttribute, pageId),
        //        new XAttribute(KnownAttributes.NameAttribute, name),
        //        new XAttribute(KnownAttributes.PageLevelAttirbute, "1"),
        //        new XAttribute(KnownAttributes.CreatedOnAttribute, DateTime.UtcNow),
        //        new XAttribute(KnownAttributes.LastModifiedOnAttribute, DateTime.UtcNow)
        //        );
        //    return xml;
        //}
    }

    //public class IdGenerator
    //{
    //    public string GeneratePageId(string sectionId)
    //    {
    //        // one note pattern => violates pattern constraint of
    //        // '\{[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}\}\{[0-9]+\}\{[A-Z][0-9]+\}'.
    //        var id = "E" + DateTime.Now.Ticks + Guid.NewGuid().GetHashCode();
    //        var sectionHeaderId = sectionId.Remove(sectionId.LastIndexOf('{'));
    //        // Page:    {2672206C-8D20-4A9B-A381-246FCC6C9622}{1}{E19490274089480943022320162594490596383860851}
    //        // Section: {2672206C-8D20-4A9B-A381-246FCC6C9622}{1}{B0}

    //        return $"{sectionHeaderId}{{{id}}}";
    //    }
    //}
}