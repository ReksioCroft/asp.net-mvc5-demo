using DevHacks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DevHacks.Controllers
{
    public class ArticleController : Controller
    {


        private Models.AppContext db = new Models.AppContext();

        public ActionResult Index()
        {

            Article article = new Article();
            article.Categ = GetAllCategories();


            var articles = from art in db.Articles
                            orderby art.ArticleDate descending
                            select art;

            ViewBag.Articles = articles;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View(article);
        }

        public ActionResult Index2(Article afisArticles)
        {
            if (afisArticles.CategoryId == 0)
                return RedirectToAction("Index");

            Article article = new Article ();
            article.Categ = GetAllCategories();


            var articles = from art in db.Articles
                            where art.CategoryId == afisArticles.CategoryId
                            orderby art.ArticleDate descending
                            select art;

            ViewBag.Articles = articles;


            return View(article);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();
            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;
            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            // returnam lista de categorii
            return selectList;

        }

        public ActionResult New()
        {
            Article article = new Article();
            article.Categ = GetAllCategories();
            return View(article);
        }

        [HttpPost]
        public ActionResult New(Article article)
        {
            article.ArticleDate = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Articles.Add(article);
                    db.SaveChanges();
                    TempData["message"] = "Aticolul a fost adaugata";
                    return Redirect("/Article/Index/");
                }
                else
                {
                    article.Categ = GetAllCategories();
                    return View(article);
                }
            }
            catch (Exception e)
            {
                return View(article);
            }
        }
        public ActionResult Show(int id)
        {
            var article = db.Articles.Find(id);
            ViewBag.Article = article;
            return View();
        }

        public ActionResult Edit(int id)
        {
            var article = db.Articles.Find(id);
            article.Categ = GetAllCategories();
            return View(article);
        }

        [HttpPut]
        public ActionResult Edit(int id, Article requestArticle)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Article article = db.Articles.Find(id);
                    if (TryValidateModel(article))
                    {
                        article.ArticleName = requestArticle.ArticleName;
                        article.ArticleContent = requestArticle.ArticleContent;
                        article.Category = requestArticle.Category;
                        article.CategoryId = requestArticle.CategoryId;
                        db.SaveChanges();
                        TempData["message"] = "Articolul a fost modificat";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    requestArticle.Categ = GetAllCategories();
                    return View(requestArticle);
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
                Article article = db.Articles.Find(id);
                db.Articles.Remove(article);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost sters";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}