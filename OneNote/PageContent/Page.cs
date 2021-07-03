using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using OneNoteApi.Common;
using OneNoteApi.Hierarchy;

namespace OneNoteApi.PageContent
{
    public class Page
    {
        public Page(XElement content)
        {
            Root = content ?? throw new ArgumentNullException(nameof(content));
            PageId = Root.Attribute(KnownAttributes.IDAttribute)?.Value;
        }

        public string PageId { get; }

        /// <summary>
        /// Gets the root element of the page
        /// </summary>
        public XElement Root { get; }

        public string Name
        {
            get => Root.Attribute(KnownAttributes.NameAttribute).Value;
            set => Root.Attribute(KnownAttributes.NameAttribute).Value = value;
        }

        public Title Title => new Title(Root.Element(PageElementTypes.Title));

        /// <summary>
        /// try to add a new tag definition and return it's index
        /// </summary>
        /// <param name="definition">the tag to define on the page</param>
        /// <returns>the index of the new\current tag</returns>
        public void AddOrUpdateTagDefinition(TagDef definition)
        {
            // go over tag def
            foreach (var xElement in Root
                .Elements(PageElementTypes.TagDef))
            {
                // verify for each if it is the same like the current tag definition
                if (definition.Xml == xElement)
                {
                    return;
                }
            }

            // didn't find the tag definition , add it
            var element = Root.Element(PageElementTypes.TagDef);
            if (element != null)
            {
                element.AddAfterSelf(definition.Xml);
            }
            else
            {
                Root.AddFirst(definition.Xml);
            }
        }
    }
}
