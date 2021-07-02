using System;
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

        public IEnumerable<OE> Children =>
            _xml.Element(PageElementTypes.OeChildren)?
                .Elements(PageElementTypes.Oe)
                .Select(x => new OE(x));

        public Tag Tag => new Tag(this._xml.Element(PageElementTypes.Tag));

        public XElement RawXml => _xml;

        public OE Clone()
        {
            return new OE(new XElement(_xml));
        }
    }

    public struct Tag
    {
        public const string IndexAttributeName = "index";
        public const string CompletedAttributeName = "completed";

        private readonly XElement _xml;
        public Tag(XElement xml)
        {
            _xml = xml;
        }

        public bool Exists => _xml != null;

        public int TagType => Convert.ToInt32(IndexAttribute.Value);

        public bool IsCompleted => Convert.ToBoolean(CompletedAttribute.Value);

        public void Complete()
        {
            CompletedAttribute.Value = bool.TrueString;
        }

        public void UpdateTagType(int newindex)
        {   
            IndexAttribute.Value = newindex.ToString();
        }


        private XAttribute CompletedAttribute => _xml.Attribute(CompletedAttributeName);
        private XAttribute IndexAttribute => _xml.Attribute(IndexAttributeName);
    }
}
