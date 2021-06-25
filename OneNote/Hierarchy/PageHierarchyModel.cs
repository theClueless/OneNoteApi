using System.Collections.Generic;
using System.Collections.Immutable;
using OneNoteApi.Common;
using OneNoteApi.Hierarchy;

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

    public class ContainerHierarchyModel : PageHierarchyModel
    {
        public virtual IImmutableList<ContainerHierarchyModel> Children { get; }

        public ContainerHierarchyModel(string name, string id, string createdOn, string lastModifiedTime, int? pageLevel, HierarchyType hierarchyType, IEnumerable<ContainerHierarchyModel> children) : base(name, id, createdOn, lastModifiedTime, pageLevel, hierarchyType)
        {
            Children = children.ToImmutableList();
        }

        public ContainerHierarchyModel(string name, string id, string createdOn, string lastModifiedTime, int? pageLevel,
            HierarchyType hierarchyType, IImmutableList<ContainerHierarchyModel> children) : base(name, id, createdOn,
            lastModifiedTime, pageLevel, hierarchyType)
        {
            Children = children;
        }
    }

    public class NotebookModel : ContainerHierarchyModel
    {
        public NotebookModel(string name, string id, string createdOn, string lastModifiedTime, int? pageLevel, IEnumerable<SectionModel> children) :
            this(name, id, createdOn, lastModifiedTime, pageLevel, HierarchyType.Notebook, children.ToImmutableList())
        {
        }

        public NotebookModel(string name, string id, string createdOn, string lastModifiedTime, int? pageLevel,
            HierarchyType hierarchyType, IImmutableList<SectionModel> children) :
            base(name, id, createdOn, lastModifiedTime, pageLevel, hierarchyType, children)
        {
            Sections = children;
        }

        /// <summary>
        /// A casted version of children
        /// </summary>
        public IImmutableList<SectionModel> Sections { get; }
    }


    public class SectionModel : ContainerHierarchyModel
    {
        public SectionModel(string name, string id, string createdOn, string lastModifiedTime, int? pageLevel, IEnumerable<PageModel> children) : base(name, id, createdOn, lastModifiedTime, pageLevel, HierarchyType.Section, children)
        {
        }
    }

    public class PageModel : ContainerHierarchyModel
    {
        public PageModel(string name, string id, string createdOn, string lastModifiedTime, int? pageLevel, IEnumerable<PageModel> children) : base(name, id, createdOn, lastModifiedTime, pageLevel, HierarchyType.Page, children)
        {
        }


    }

}
