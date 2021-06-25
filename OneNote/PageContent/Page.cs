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
            PageId = content.Attribute(KnownAttributes.IDAttribute)?.Value;
        }


        public bool IsValid => Root != null;


        public string PageId { get; }

        /// <summary>
        /// Gets the root element of the page
        /// </summary>
        public XElement Root { get; }
    }
}
