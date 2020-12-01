using DevHacks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DevHacks.Controllers
{
    public class CategoryController : Controller
    {
        private Models.AppContext db = new Models.AppContext();
        // GET: Categories
        public ActionResult Index()
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        public ActionResult Show(int id )
        {
            var categorie = db.Categories.Find(id);
            ViewBag.Categories = categorie;
            return View();
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New( Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost adaugata";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(category);
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            var categorie = db.Categories.Find(id);
            ViewBag.Categories = categorie;
            return View(categorie);
        }

        [HttpPut]
        public ActionResult Edit( int id, Category requestCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category categorie = db.Categories.Find(id);
                    if (TryValidateModel(categorie)){
                        categorie.CategoryName = requestCategory.CategoryName;
                        db.SaveChanges();
                        TempData["message"] = "Categoria a fost modificata";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestCategory);
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                var categorie = db.Categories.Find(id);
                db.Categories.Remove(categorie);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost stearsa";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}