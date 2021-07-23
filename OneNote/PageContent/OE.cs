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

        public Tag Tag => new(this._xml.Element(PageElementTypes.Tag));

        public Text Text => new(this._xml.Element(PageElementTypes.Text));

        public XElement RawXml => _xml;

        public OE Clone()
        {
            return new(new XElement(_xml));
        }
    }
}
