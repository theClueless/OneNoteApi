using System.Xml.Linq;

namespace OneNoteApi.Common
{
    public static class HierarchyElementTypes
    {
        internal const string PageName = "Page";
        internal const string SectionName = "Section";
        internal const string NotebooksName = "Notebooks";
        internal const string NotebookName = "Notebook";
        internal const string SectionGroupName = "SectionGroup";


        static HierarchyElementTypes()
        {
            var oneNoteNamespace = XNamespace.Get(PageElementTypes.OneNoteNamespace);
            Page = oneNoteNamespace + PageName;
            Section = oneNoteNamespace + SectionName;
            Notebooks = oneNoteNamespace + NotebooksName;
            Notebook = oneNoteNamespace + NotebookName;
            SectionGroup = oneNoteNamespace + SectionGroupName;
        }

        public static XName Page { get; }
        public static XName Section { get; }
        public static XName Notebooks { get; }
        public static XName Notebook { get; }
        public static XName SectionGroup { get; }
    }
}