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
    public class QuestionController : Controller
    {
        private int _perPage = 3;

        private ApplicationDbContext db = ApplicationDbContext.Create();

        public ActionResult IndexAux(Question requestQuestion)
        {

            return Redirect("/Question/Index/" + requestQuestion.CategoryId + "/" + requestQuestion.Criterion);
        }

        public ActionResult Index(int? id, int? criterion)
        {
            if (criterion == null)
                criterion = 0;

            ViewBag.indexId = id;
            ViewBag.criterion = criterion;

            Question question = new Question();
            question.Categ = GetAllCategories();



            var questions = (id == null || id == 0) ?
                                                    (
                                                        criterion == 0 ?
                                                          from qs in db.Questions
                                                          orderby qs.QuestionDate descending
                                                          select qs :
                                                        (
                                                            criterion == 1 ?
                                                              from qs in db.Questions
                                                              orderby qs.QuestionDate ascending
                                                              select qs :
                                                            (
                                                                criterion == 2 ?
                                                                  from qs in db.Questions
                                                                  orderby qs.QuestionTitle ascending
                                                                  select qs :

                                                                      from qs in db.Questions
                                                                      orderby qs.QuestionTitle descending
                                                                      select qs
                                                                 )
                                                        )
                                                    ) :
                                                                 (
                                                        criterion == 0 ?
                                                          from qs in db.Questions
                                                          where qs.CategoryId == id
                                                          orderby qs.QuestionDate descending
                                                          select qs :
                                                        (
                                                            criterion == 1 ?
                                                              from qs in db.Questions
                                                              where qs.CategoryId == id
                                                              orderby qs.QuestionDate ascending
                                                              select qs :
                                                            (
                                                                criterion == 2 ?
                                                                  from qs in db.Questions
                                                                  where qs.CategoryId == id
                                                                  orderby qs.QuestionTitle ascending
                                                                  select qs :

                                                                      from qs in db.Questions
                                                                      where qs.CategoryId == id
                                                                      orderby qs.QuestionTitle descending
                                                                      select qs
                                                                 )
                                                        )
                                                    );


            ViewBag.Questions = questions;

            var totalItems = questions.Count();

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

            var paginatedQuestions = questions.Skip(offset).Take(this._perPage);

            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.Questions = paginatedQuestions;

            return View(question);
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
            //   ViewBag.Categories = from qs in db.Categories
            //                      select qs;
            Question question = new Question();
            question.Categ = GetAllCategories();
            return View(question);
        }

        [HttpPost]
        public ActionResult New(Question question)
        {
            question.QuestionDate = DateTime.Now;
            question.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Questions.Add(question);
                    db.SaveChanges();
                    TempData["message"] = "Intrebarea a fost adaugata";
                    return Redirect("/Question/Index/");
                }
                else
                {
                    question.Categ = GetAllCategories();
                    return View(question);
                }
            }
            catch (Exception e)
            {
                return View(question);
            }
        }
        public ActionResult Show(int id)
        {
            var question = db.Questions.Find(id);
            ViewBag.Question = question;
            var comments = from com in db.Comments
                           where com.QuestionId == question.QuestionId
                           orderby com.CommentDate descending
                           select com;
            ViewBag.Comments = comments;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        public ActionResult Edit(int id)
        {
            var question = db.Questions.Find(id);
            if (User.Identity.GetUserId() == question.UserId || User.IsInRole("Admin")||User.IsInRole("Moderator"))
            {
                question.Categ = GetAllCategories();
                return View(question);
            }
            else
            {
                TempData["message"] = "Nu aveți această permisiune. Vă redirectez...";
                return Redirect("/Question/Show/" + id);
            }
        }

        [HttpPut]
        public ActionResult Edit(int id, Question requestQuestion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Question question = db.Questions.Find(id);

                    if (TryValidateModel(question))
                    {
                        if (User.Identity.GetUserId() == question.UserId || User.IsInRole("Moderator") || User.IsInRole("Admin"))
                        {
                            if (User.Identity.GetUserId() == question.UserId)
                            {
                                question.QuestionTitle = requestQuestion.QuestionTitle;
                                question.QuestionContent = requestQuestion.QuestionContent;
                            }
                            if (User.IsInRole("Moderator") || User.IsInRole("Admin"))
                            {
                                question.Category = requestQuestion.Category;
                                question.CategoryId = requestQuestion.CategoryId;
                            }
                            db.SaveChanges();
                            TempData["message"] = "Intrebarea a fost modificat";
                        }
                        else
                        {
                            TempData["message"] = "Nu aveți această permisiune. Vă redirectez...";
                        }
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    requestQuestion.Categ = GetAllCategories();
                    return View(requestQuestion);
                }
            }
            catch (Exception e)
            {
                return View(requestQuestion);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                Question question = db.Questions.Find(id);
                if (User.Identity.GetUserId() == question.UserId || User.IsInRole("Admin") || User.IsInRole("Admin"))
                {
                    db.Questions.Remove(question);
                    db.SaveChanges();
                    TempData["message"] = "Intrebarea a fost stearsa";
                }
                else
                {
                    TempData["message"] = "Nu aveți această permisiune. Vă redirectez...";
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult NewComment(int id, Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            comment.QuestionId = id;
            comment.UserId = User.Identity.GetUserId();

            try
            {
                if (ModelState.IsValid)
                {
                    db.Comments.Add(comment);
                    db.SaveChanges();
                    TempData["message"] = "Comentariul a fost adaugata";
                    return Redirect("/Question/Show/" + id);
                }
                else
                {
                    return Redirect("/Question/Show/" + id);
                }
            }
            catch (Exception e)
            {
                return Redirect("/Question/Show/" + id);
            }
        }

        [HttpDelete]
        public ActionResult DeleteComment(int id)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                if (User.Identity.GetUserId() == comment.UserId || User.IsInRole("Admin") || User.IsInRole("Moderator"))
                {

                    db.Comments.Remove(comment);
                    db.SaveChanges();
                    TempData["message"] = "Comentariul a fost sters";
                }
                else
                {
                    TempData["message"] = "Nu aveți această permisiune. Vă redirectez...";
                }
                return Redirect("/Question/Show/" + comment.QuestionId);
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpPut]
        public ActionResult EditCommentInside(int id, Comment requestComment)   ///BUGS!!! TODO LATER...
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                if (User.Identity.GetUserId() == comment.UserId)
                {

                    //     if (ModelState.IsValid)
                    //   {                                                          //TODO
                    // if (TryValidateModel(comment))
                    //{
                    comment.CommentContent = requestComment.CommentContent;
                    db.SaveChanges();
                    TempData["message"] = "Comentariul a fost modificat";
                    //}
                    // }
                    // else
                    //{
                    // TempData["message"] = "Comentariul NU a fost modificat";

                    // }
                }
                else
                {
                    TempData["message"] = "Nu aveți această permisiune. Vă redirectez...";

                }
                return Redirect("/Question/Show/" + comment.QuestionId);
            }
            catch (Exception e)
            {
                TempData["message"] = "Comentariul NU a fost modificat";
                return RedirectToAction("Index");
            }
        }

        public ActionResult EditComment(int id)
        {
            var comment = db.Comments.Find(id);
            if (User.Identity.GetUserId() == comment.UserId)
                return View(comment);
            else
            {
                TempData["message"] = "Nu aveți această permisiune. Vă redirectez...";
                return Redirect("/Question/Show/" + comment.QuestionId);
            }
        }


        public ActionResult Search()
        {
            var questions = db.Questions.Include("Category").OrderBy(a => a.QuestionDate);
            var search = "";

            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();

                List<int> questionsIds = db.Questions.Where
                    (
                a => a.QuestionTitle.Contains(search) ||
                a.QuestionContent.Contains(search)
                    ).Select(a => a.QuestionId).ToList();

                List<int> commentIds = db.Comments.Where
                        (
                    c => c.CommentContent.Contains(search)
                        ).Select(com => com.CommentId).ToList();

                List<int> mergedIds = questionsIds.Union(commentIds).ToList();


                questions = db.Questions.Where(question => mergedIds.Contains(question.QuestionId)).OrderBy(a => a.QuestionDate);
            }



         

            var totalItems = questions.Count();

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

            var paginatedQuestions = questions.Skip(offset).Take(this._perPage);

            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.Questions = paginatedQuestions;
            ViewBag.SearchString = search;

            return View();
        }






        [HttpPut]
        public ActionResult EditComment(int id, Comment requestComment)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                if (ModelState.IsValid)
                {
                    if (TryValidateModel(comment))
                    {
                        if (User.Identity.GetUserId() == comment.UserId)
                        {
                            comment.CommentContent = requestComment.CommentContent;
                            db.SaveChanges();
                            TempData["message"] = "Comentariul a fost modificat";
                        }
                        else
                        {
                            TempData["message"] = "Nu aveți această permisiune. Vă redirectez...";
                        }
                    }
                    else
                    {
                        TempData["message"] = "Comentariul NU a fost modificat";

                    }
                      return Redirect("/Question/Show/" + comment.QuestionId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            catch (Exception e)
            {
                return View(requestComment);
            }
        }
    }
}