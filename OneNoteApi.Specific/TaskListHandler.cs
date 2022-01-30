using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneNoteApi.PageContent;
using OneNoteApi.PageContent.Builders;

namespace OneNoteApi.Mine
{
    /// <summary>
    /// Helper class to work with a list of bulleted tasks (task = bullet with task tag)
    /// </summary>
    public class TaskListHandler
    {
        private readonly OE _content;
        private readonly List<TaskListObject> _tasks;

        /// <summary>
        /// create a new task list handler
        /// </summary>
        /// <param name="content">the top OE of the task list</param>
        public TaskListHandler(OE content)
        {
            _content = content;
            _tasks = CreateTasks(content.Children);
        }

        private List<TaskListObject> CreateTasks(IEnumerable<OE> contentChildren)
        {
            return contentChildren.Select(x => new TaskListObject(x)).ToList();
        }

        public IReadOnlyList<TaskListObject> Tasks => _tasks;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskContent"></param>
        /// <param name="addFirst">when false task would be added last, when true it would be added first</param>
        public void AddNewTask(string taskContent, bool addFirst = false)
        {
            // build new task object
            var newTask = OeBuilder.Build()
                .WithTag(KnownTags.ToDoTagIndex)
                .WithBullet(3)
                .WithText(taskContent);

            _tasks.Insert(0, new TaskListObject(newTask));

            // add new child
            _content.AddOEChild(newTask, addFirst);
        }

        public void RemoveTask(TaskListObject task)
        {
            if (!_tasks.Contains(task))
            {
                throw new Exception("Unknown task");
            }

            task._oe.RemoveFromParent();
            _tasks.Remove(task);
        }

    }

    public class TaskListObject
    {
        internal readonly OE _oe;

        public TaskListObject(OE oe)
        {
            _oe = oe;
        }

        public bool IsCompleted
        {
            get => _oe.Tag.IsCompleted;
            set => _oe.Tag.SetState(value);
        }

        public bool HasSubTasks => _oe.Children.Any();
        

    }
}
