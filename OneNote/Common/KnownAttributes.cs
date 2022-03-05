using System.Data;

namespace OneNoteApi.Common
{
    public static class KnownAttributes
    {
        public const string IDAttribute = "ID";
        public const string NameAttribute = "name";
        public const string CreatedOnAttribute = "dateTime"; // TODO: change to datetime
        public const string LastModifiedOnAttribute = "lastModifiedTime"; // TODO: change to datetime
        public const string PageLevelAttirbute = "pageLevel";
    }
}
