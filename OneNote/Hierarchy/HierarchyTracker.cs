using System.Collections.Generic;
using System.Linq;
using System.Text;
using OneNoteApi.Common;

namespace OneNoteApi.Hierarchy
{
    /// <summary>
    /// Helper class that help add Hierarchy to PageHierarchyModel collection when feed in order
    /// </summary>
    public class HierarchyTracker
    {
        private readonly Stack<string> _stack = new();
        private readonly StringBuilder _builder = new(500);

        /// <summary>
        /// Get the next element and return it's hierarchy
        /// </summary>
        /// <param name="model">the model to get the hierarchy from</param>
        /// <returns>the hierarchy of the model</returns>
        public string GetAndUpdate(PageHierarchyModel model)
        {
            var current = _stack.Count;
            var modelCount = GetCountFromModel(model);
            if (modelCount < current)
            { // note\sec\page1\page2
                // note\sec\page3
                // need to remove
                for (int i = 0; i < current - modelCount; i++)
                {
                    _stack.Pop();
                }
            }

            var res = BuildCurrent();

            // update 
            _stack.Push(model.Name);

            return res;
        }

        private string BuildCurrent()
        {
            _builder.Clear();
            bool first = true;
            foreach (var elem in _stack.Reverse())
            {
                if (!first)
                {
                    _builder.Append('\\');
                }
                else
                {
                    first = false;
                }

                _builder.Append(elem);
            }

            return _builder.ToString();
        }

        private static int GetCountFromModel(PageHierarchyModel model)
        {
            return model.HierarchyType switch
            {
                HierarchyType.Notebook => 0,
                HierarchyType.Section => 1,
                HierarchyType.Page => model.PageLevel.GetValueOrDefault() + 1,
                _ => 0
            };
        }
    }
}