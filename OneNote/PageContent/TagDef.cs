using System;
using System.Xml.Linq;
using OneNoteApi.Common;

namespace OneNoteApi.PageContent
{
    public readonly struct TagDef
    {
        public const string IndexAttributeName = "index";
        public const string TagTypeAttributeName = "type";
        public const string SymbolAttributeName = "symbol";
        public const string FontColorAttributeName = "fontColor";
        public const string HighlightColorAttributeName = "highlightColor";
        public const string NameAttributeName = "name";


        private readonly XElement _xml;
        
        // <one:TagDef index="1" tagType="3" symbol="25" fontColor="#00FF00" highlightColor="none" name="Did" />
        public TagDef(XElement xml)
        {
            _xml = xml;
        }

        public TagDef(int index, int tagType, int symbol, string fontColor, string highlightColor, string name)
            : this(new XElement(PageElementTypes.TagDef,
                new XAttribute(IndexAttributeName, index.ToString()),
                new XAttribute(TagTypeAttributeName, tagType.ToString()),
                new XAttribute(SymbolAttributeName, symbol.ToString()),
                new XAttribute(FontColorAttributeName, fontColor),
                new XAttribute(HighlightColorAttributeName, highlightColor),
                new XAttribute(NameAttributeName, name)
            ))
        { }

        public bool Exists => _xml != null;
        public XElement Xml => _xml;

        public int TagType => Convert.ToInt32(_xml.Attribute(TagTypeAttributeName).Value);
        public int Symbol => Convert.ToInt32(_xml.Attribute(SymbolAttributeName).Value);
        public int Index => Convert.ToInt32(_xml.Attribute(IndexAttributeName).Value);
        public string Name => _xml.Attribute(NameAttributeName).Value;
        public string FontColor => _xml.Attribute(FontColorAttributeName).Value;
        public string HighlightColor => _xml.Attribute(HighlightColorAttributeName).Value;
    }
}