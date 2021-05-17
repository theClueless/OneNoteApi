using System;
using System.Collections.Generic;
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
                    xElement.Name.LocalName == "Section" ? HierarchyType.Section : HierarchyType.Notebook;

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
            var value = element?.Attribute("pageLevel")?.Value;

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
