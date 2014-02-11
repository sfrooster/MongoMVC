using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

using MongoMVC.Models;

namespace MongoMVC.Controllers
{
    public class TodoListController : Controller
    {
        private MongoEntity<TodoList> meTDL
        {
            get
            {
                return new MongoEntity<TodoList>();
            }
        }

        public ActionResult Index()
        {
            var recent = meTDL.Collection.AsQueryable()
                .Take(10)
                .OrderByDescending(tdl => tdl.Created);

            return View(recent.ToList<TodoList>());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new TodoList());
        }

        [HttpPost]
        public ActionResult Create(TodoList tdl)
        {
            ObjectId? newId = null;

            if (ModelState.IsValid)
            {
                newId = meTDL.Insert(tdl);

                if (newId.HasValue)
                {
                    return RedirectToAction("Details", new { id = newId.Value });
                }
                else
                {
                    return RedirectToAction("Error", new Error { Location = "Create TDL", Description = "???" });
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var oid = new ObjectId(id);
            var entity = meTDL.Collection.AsQueryable().Where(e => e.Id == oid).FirstOrDefault();
            return View(entity);
        }

        [HttpPost]
        public ActionResult Edit(TodoList tdl)
        {
            WriteConcernResult result = null;
   
            if (ModelState.IsValid)
            {
                MongoEntity<TodoList> mtdl = new MongoEntity<TodoList>();
                var update = new UpdateBuilder<TodoList>();
                update.Set(t=>t.Name, tdl.Name);

                result = mtdl.Update(tdl.Id, update);

                if (result.Ok && result.DocumentsAffected == 1)
                {
                    return RedirectToAction("Details", new { id = tdl.Id });
                }
                else
                {
                    return RedirectToAction("Error", new Error { Location = "Update TDL", Description = "???" });
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            var oid = new ObjectId(id);
            var entity = meTDL.Collection.AsQueryable().Where(e => e.Id == oid).FirstOrDefault();
            return View(entity);
        }
    }
}