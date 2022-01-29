using System.Xml.Linq;

namespace OneNoteApi.Common
{
    public static class PageElementTypes
    {
        public const string OneNoteNamespace = @"http://schemas.microsoft.com/office/onenote/2013/onenote";
        public const string OneNoteNamespaceName = "one";
        internal const string TextName = "T";
        internal const string OeName = "OE";
        internal const string OeChildrenName = "OEChildren";
        internal const string TagName =  "Tag";
        internal const string TagDefName = "TagDef";
        internal const string TitleName = "Title";
        internal const string PositionName = "Position";
        internal const string OutlineName = "Outline";
        internal const string ListName = "List";
        internal const string BulletName = "Bullet";

        static PageElementTypes()
        {
            var oneNoteNamespace = XNamespace.Get(OneNoteNamespace);
            Text = oneNoteNamespace + TextName;
            Oe = oneNoteNamespace + OeName;
            OeChildren = oneNoteNamespace + OeChildrenName;
            Tag = oneNoteNamespace + TagName;
            TagDef = oneNoteNamespace + TagDefName;
            Title = oneNoteNamespace + TitleName;
            Position = oneNoteNamespace + PositionName;
            Outline = oneNoteNamespace + OutlineName;
            List = oneNoteNamespace + ListName;
            Bullet = oneNoteNamespace + BulletName;
        }

        public static XName Text { get; }
        public static XName Oe { get; }
        public static XName OeChildren { get; }
        public static XName Tag { get; }
        public static XName TagDef { get; }
        public static XName Title { get; }
        public static XName Position { get; }
        public static XName Outline { get; }
        public static XName List { get; }
        public static XName Bullet { get; }

    }
}