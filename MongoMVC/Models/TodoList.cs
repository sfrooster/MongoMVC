using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MongoMVC.Models
{
    public class TodoList : BaseMongoEntity
    {
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        public IList<TodoListTask> Tasks { get; set; }

        public TodoList()
        {
            Name = String.Empty;
            Created = DateTime.Now;
            Tasks = new List<TodoListTask>();
        }
    }

    public class TodoListTask
    {
        public string Description { get; set; }

        public bool Complete { get; set; }
    }

    public class Error
    {
        public string Location { get; set; }
        public string Description { get; set; }
    }
}