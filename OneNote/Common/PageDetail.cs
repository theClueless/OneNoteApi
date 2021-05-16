using Microsoft.Office.Interop.OneNote;

namespace OneNoteApi.Common
{
    public enum PageDetail
    {
        All = PageInfo.piAll,
        Basic = PageInfo.piBasic,
        BinaryData = PageInfo.piBinaryData,
        BinaryDataFileType = PageInfo.piBinaryDataFileType,
        BinaryDataSelection = PageInfo.piBinaryDataSelection,
        FileType = PageInfo.piFileType,
        Selection = PageInfo.piSelection,
        SelectionFileType = PageInfo.piSelectionFileType
    }
}