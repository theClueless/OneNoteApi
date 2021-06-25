using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OneNoteApi.Common;

namespace OneNoteApi.Hierarchy
{
    internal class PageHierarchyWalker
    {
        private readonly XElement _xml;

        public PageHierarchyWalker(XElement pagesDoc)
        {
            this._xml = pagesDoc;
        }

        //public SectionModel GetSection()
        //{
        //    Stack<PageHierarchyModel> models = new Stack<PageHierarchyModel>();
        //    foreach (var pageHierarchyModel in GetElements())
        //    {
        //        if (!models.Any())
        //        {
        //            models.Push(pageHierarchyModel);
        //        }
        //        else
        //        {
        //            var last = models.Peek();
        //            var currentLevel = pageHierarchyModel.PageLevel.GetValueOrDefault();
        //            if (last.PageLevel.GetValueOrDefault() <= currentLevel)
        //            {
        //                var parent = models.Pop();
        //                var children = GetChildrenFromStack(models, currentLevel);
        //                // we need to create it with it's children
        //                // section 1  -> 2
        //                    // page 1 -> 3
        //                        // page 2 ->4
        //                            // page 3 -> 5
        //                    // page 4 -> 3
        //            }




        //        }
        //    }

        //    // element
        //        // find all the children it has
        //        // create element with children
        //    // return element

        //}

        //private IEnumerable<PageHierarchyModel> GetChildrenFromStack(Stack<PageHierarchyModel> models, int currentLevel)
        //{
        //    while (models.Peek().PageLevel.GetValueOrDefault() > currentLevel)
        //    {
        //        yield return models.Pop();
        //    }
        //}

        public IEnumerable<PageHierarchyModel> GetElements()
        {
            foreach (var xElement in _xml.Descendants())
            {
                var id = OneNoteXmlHelper.GetId(xElement);
                if (id == null)
                {
                    continue;
                }

                var name = OneNoteXmlHelper.GetName(xElement);
                var pageLevel = OneNoteXmlHelper.GetPageLevel(xElement);
                HierarchyType hierarchyType = pageLevel.HasValue ?
                    HierarchyType.Page :
                    xElement.Name.LocalName == PageTypes.Section ? HierarchyType.Section : HierarchyType.Notebook;

                var createdOn = OneNoteXmlHelper.GetAttribute(xElement, KnownAttributes.CreatedOnAttribute);
                var lastModified = OneNoteXmlHelper.GetAttribute(xElement, KnownAttributes.LastModifiedOnAttribute);
                yield return new PageHierarchyModel(name, id, createdOn, lastModified, pageLevel, hierarchyType);
            }
        }
    }

    internal static class OneNoteXmlHelper
    {
        public static int? GetPageLevel(XElement element)
        {
            var value = element?.Attribute(KnownAttributes.PageLevelAttirbute)?.Value;

            if (value != null && int.TryParse(value, out int res))
            {
                return res;
            }

            return null;
        }

        public static string GetId(XElement xElement)
        {
            return xElement.Attribute(KnownAttributes.IDAttribute)?.Value;
        }

        public static string GetCreatedOn(XElement xElement)
        {
            return xElement.Attribute(KnownAttributes.CreatedOnAttribute)?.Value;
        }

        public static string GetAttribute(XElement element, string attributeName)
        {
            return element.Attribute(attributeName)?.Value;
        }

        public static string GetName(XElement element) => element.Attribute(KnownAttributes.NameAttribute)?.Value;

    }
}
