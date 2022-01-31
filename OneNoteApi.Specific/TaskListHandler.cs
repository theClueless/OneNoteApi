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

            AddNewTask(new TaskListObject(newTask), addFirst);
        }

        public void AddNewTask(TaskListObject task, bool addFirst = false)
        {
            if (addFirst)
            {
                _tasks.Insert(0, task);
            }
            else
            {
                _tasks.Add(task);
            }
            
            // add new child
            _content.AddOEChild(task.Oe, addFirst);
        }

        public void RemoveTask(TaskListObject task)
        {
            if (!_tasks.Contains(task))
            {
                throw new Exception("Unknown task");
            }

            task.Oe.RemoveFromParent();
            _tasks.Remove(task);
        }

    }

    public class TaskListObject
    {
        internal readonly OE Oe;

        public TaskListObject(OE oe)
        {
            Oe = oe;
        }

        public bool IsValid => Oe.Tag.Exists;

        public bool IsCompleted
        {
            get => Oe.Tag.IsCompleted;
            set => Oe.Tag.SetState(value);
        }

        public bool HasSubTasks => Oe.HasChildren;

        public string Content => Oe.Text.GetValueIfExists();
    }
}
