using OneNoteApi.Common;

namespace OneNoteApi.Hierarchy
{
    public class PageHierarchyModel
    {
        public PageHierarchyModel(string name, string id, string createdOn, string lastModifiedTime, int? pageLevel, HierarchyType hierarchyType)
        {
            Name = name;
            Id = id;
            CreatedOn = createdOn;
            LastModifiedTime = lastModifiedTime;
            PageLevel = pageLevel;
            HierarchyType = hierarchyType;
        }

        public string Name { get; }

        public string Id { get; }

        public string CreatedOn { get; }

        public string LastModifiedTime { get; }

        public int? PageLevel { get; }

        public HierarchyType HierarchyType { get; }
    }
}