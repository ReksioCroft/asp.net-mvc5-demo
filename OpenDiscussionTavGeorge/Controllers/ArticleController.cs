using Microsoft.AspNet.Identity;
using OpenDiscussionTavGeorge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenDiscussionTavGeorge.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private int _perPage = 3;

        private ApplicationDbContext db = ApplicationDbContext.Create();

        public ActionResult IndexAux(Article requestArticle)
        {
            return Redirect("/Article/Index/" + requestArticle.CategoryId);
        }

        public ActionResult Index(int? id)
        {
            ViewBag.indexId = id;
            Article article = new Article ();
            article.Categ = GetAllCategories();


            var articles = (id == null || id == 0) ? from art in db.Articles
                                                     orderby art.ArticleDate descending
                                                     select art : from art in db.Articles
                                                                  where art.CategoryId == id
                                                                  orderby art.ArticleDate descending
                                                                  select art;
            ViewBag.Articles = articles;

            var totalItems = articles.Count();

            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            var paginatedArticles = articles.Skip(offset).Take(this._perPage);

            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.Articles = paginatedArticles;


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
            article.UserId = User.Identity.GetUserId();

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