using System;
using System.Collections.Generic;
using System.Text;
using OneNoteApi.PageContent;

namespace OneNoteApi.Mine
{
    public class KnownTags
    {
        public const int ToDoTagIndex = 0;
        public const int HappySmilyTagIndex = 1;
        public const int SadSmilyTagIndex = 2;

        static KnownTags()
        {
            //<one:TagDef index="1" type="3" symbol="25" fontColor="#00FF00" highlightColor="none" name="Did" />
            // < one:TagDef index = "2" type = "2" symbol = "113" fontColor = "#FF0000" highlightColor = "none" name = "didn't do" />
            HappySmilyTag = new TagDef(1, 3, 25, "#00FF00", "none", "Did");
            SadSmilyTag = new TagDef(2, 2, 113, "#FF0000", "none", "didn't do");
        }

        public static TagDef HappySmilyTag { get; }
        public static TagDef SadSmilyTag { get; }
    }
}
