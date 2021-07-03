﻿using System.Xml.Linq;
using OneNoteApi.Common;

namespace OneNoteApi.PageContent
{
    public readonly struct Title
    {
        public const string LanguageAttributeName = "lang";

        private readonly XElement _xml;
        public Title(XElement xml)
        {
            _xml = xml;
        }

        public bool Exists => _xml != null;

        public OE Content => new OE(_xml.Element(PageElementTypes.Oe));

        public string Language => _xml.Attribute(LanguageAttributeName)?.Value;
    }

    public struct Text
    {
        private readonly XElement _xml;
        public Text(XElement xml)
        {
            _xml = xml;
        }

        public bool Exists => _xml != null;

        /// <summary>
        /// Value of the text
        /// </summary>
        public string Value
        {
            get => _xml.Value;
            set => _xml.Value = value;
        }
    }
}