using OneNoteApi.Common;

namespace OneNoteApi.Hierarchy
{
    public class PageHierarchyModel
    {
        public string Name { get; init; }

        public string Id { get; init; }

        public string CreatedOn { get; init; }

        public string LastModifiedTime { get; init; }

        public int? PageLevel { get; init; }

        public HierarchyType HierarchyType { get; init; }
        //<one:Page ID="{6DD529C0-762E-018C-0461-B3B60247D129}{1}{E19534990630611588600120138076622292878245531}"
        //name="Someone" dateTime="2019-11-07T06:38:56.000Z" lastModifiedTime="2019-11-07T06:38:59.000Z" pageLevel="1" />
        //<one:Page ID = "{6DD529C0-762E-018C-0461-B3B60247D129}{1}{E19540860559378138484120135987021111103018271}"
        //              name="SomethingLevelTwo" dateTime="2019-07-25T18:39:30.000Z" lastModifiedTime="2019-11-23T21:57:17.000Z" pageLevel="2" />

        //private List<OneNoteEntry> ConvertDocToEntries(XElement doc)
        //{
        //    var res = new List<OneNoteEntry>();
        //    var idSet = new Dictionary<string, string>();
        //    var perLevelDictionary = new Dictionary<int, string>();

        //    foreach (var xElement in doc.Descendants())
        //    {

        //        var id = OneNoteXmlHelper.GetId(xElement);
        //        if (id == null)
        //        {
        //            continue;
        //        }

        //        var name = OneNoteXmlHelper.GetName(xElement);

        //        string hierarchy;
        //        var pageLevel = OneNoteXmlHelper.GetPageLevel(xElement);
        //        if (pageLevel != null)
        //        {
        //            var val = pageLevel.Value;
        //            hierarchy =
        //                val == 1
        //                    ? OneNoteXmlHelper.GetFullNameHierarchy(xElement)
        //                    : $"{perLevelDictionary[pageLevel.Value - 1]}";
        //            var nextLevelHierarchy = $"{hierarchy}\\{name}";
        //            perLevelDictionary[pageLevel.Value] = nextLevelHierarchy;
        //        }
        //        else
        //        {
        //            hierarchy = OneNoteXmlHelper.GetFullNameHierarchy(xElement);
        //        }

        //        if (!idSet.ContainsKey(hierarchy))
        //        {
        //            idSet[hierarchy] = hierarchy;
        //        }
        //        else
        //        {
        //            hierarchy = idSet[hierarchy];
        //        }

        //        var entry = new OneNoteEntry
        //        {
        //            Name = name,
        //            Id = id,
        //            Hierarchy = hierarchy,
        //            PageLevel = pageLevel.GetValueOrDefault()
        //        };
        //        res.Add(entry);
        //    }

        //    return res;
        //}
    }
}