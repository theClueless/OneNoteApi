using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OneNoteApi.Common;

namespace OneNoteApi.PageContent
{
    public class Outline
    {
        private readonly XElement _xml;

        public Outline(XElement xml)
        {
            _xml = xml;
        }

        public Position Position => new Position(_xml.Element(PageElementTypes.Position));

        public IEnumerable<OE> Children =>
            _xml.Element(PageElementTypes.OeChildren)?
                .Elements(PageElementTypes.Oe)
                .Select(x => new OE(x));
    }

    public readonly struct Position
    {
        public const string XAttributeName = "x";
        public const string YAttributeName = "y";
        public const string ZAttributeName = "z";

        private readonly XElement _xml;
        public Position(XElement xml)
        {
            _xml = xml;
        }

        public bool Exists => _xml != null;

        public double X => double.Parse(_xml.Attribute(XAttributeName).Value);
        public double Y => double.Parse(_xml.Attribute(YAttributeName).Value);
        public double Z => double.Parse(_xml.Attribute(ZAttributeName).Value);
    }
}
