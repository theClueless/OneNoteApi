﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using OneNoteApi.Common;

namespace OneNoteApi.PageContent
{
    public class OE
    {
        private readonly XElement _xml;

        public OE(XElement xml)
        {
            _xml = xml;
        }

        public bool HasChildren => OeChildrenElement() != null;

        public IEnumerable<OE> Children =>
            OeChildrenElement()?
                .Elements(PageElementTypes.Oe)
                .Select(x => new OE(x));

        public Tag Tag => new(this._xml.Element(PageElementTypes.Tag));

        public Text Text => new(this._xml.Element(PageElementTypes.Text));

        /// <summary>
        /// Return the Id of the OE object
        /// </summary>
        public string ObjectId => this._xml.Attribute(KnownElementAttributes.ObjectIdAttribute)?.Value;

        /// <summary>
        /// Return the last modified time or null if time not found
        /// </summary>
        public DateTime? LastModifiedTime
        {
            get
            {
                var timeString = this._xml.Attribute(KnownElementAttributes.LastModifiedOnAttribute)?.Value;
                return timeString == null ? null : DateTime.Parse(timeString);
            } 
        }

        public XElement RawXml => _xml;

        public OE Clone()
        {
            return new(new XElement(_xml));
        }

        /// <summary>
        /// Remove the child XML from the parent resulting in removal during update of the page
        /// </summary>
        public void RemoveFromParent()
        {
            _xml.Remove();
        }

        private XElement OeChildrenElement() => _xml.Element(PageElementTypes.OeChildren);

        public void AddOEChild(OE newChild, bool addFirst = false)
        {
            if (addFirst)
            {
                OeChildrenElement().AddFirst(newChild.RawXml);
            }
            else
            {
                OeChildrenElement().Add(newChild.RawXml);
            }
            
        }
    }
}
