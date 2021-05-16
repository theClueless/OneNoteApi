using Microsoft.Office.Interop.OneNote;

namespace OneNoteApi.Common
{
    public enum Scope
    {
        Children = HierarchyScope.hsChildren,
        Notebooks = HierarchyScope.hsNotebooks,
        Pages = HierarchyScope.hsPages,
        Sections = HierarchyScope.hsSections,
        Self = HierarchyScope.hsSelf
    }

    public enum HierarchyType
    {
        Notebook,
        Section,
        Page
    }
}