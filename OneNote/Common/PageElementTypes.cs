using System.Xml.Linq;

namespace OneNoteApi.Common
{
    public static class PageElementTypes
    {
        public const string OneNoteNamespace = @"http://schemas.microsoft.com/office/onenote/2013/onenote";
        public const string OneNoteNamespaceName = "one";
        public const string TextName = "T";
        public const string OeName = "OE";
        public const string OeChildrenName = "OEChildren";
        public const string TagName =  "Tag";


        static PageElementTypes()
        {
            var oneNoteNamespace = XNamespace.Get(OneNoteNamespace);
            Text = oneNoteNamespace + TextName;
            Oe = oneNoteNamespace + OeName;
            OeChildren = oneNoteNamespace + OeChildrenName;
            Tag = oneNoteNamespace + TagName;
        }

        public static XName Text { get; }
        public static XName Oe { get; }
        public static XName OeChildren { get; }
        public static XName Tag { get; }
    }
}