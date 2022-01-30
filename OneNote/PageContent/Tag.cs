using System;
using System.Xml.Linq;

namespace OneNoteApi.PageContent
{
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

        /// <summary>
        /// set the state of the tag, true for complete false for incomplete.
        /// </summary>
        /// <param name="newState"></param>
        public void SetState(bool newState)
        {
            CompletedAttribute.Value = newState ? "true" : "false";
        }

        public void UpdateTagType(int newIndex)
        {   
            IndexAttribute.Value = newIndex.ToString();
        }


        private XAttribute CompletedAttribute => _xml.Attribute(CompletedAttributeName);
        private XAttribute IndexAttribute => _xml.Attribute(IndexAttributeName);
    }
}