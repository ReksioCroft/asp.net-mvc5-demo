using DevHacks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DevHacks.Controllers
{
    public class QuestionController : Controller
    {


        private Models.AppContext db = new Models.AppContext();
       
        public ActionResult Index()
        {

            Question question = new Question();
            question.Categ = GetAllCategories();


            var questions = from qs in db.Questions
                            orderby qs.QuestionDate descending
                            select qs;
            
            ViewBag.Questions = questions;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View(question);
        }

        public ActionResult Index2(Question afisQuestions)
        {
            if(afisQuestions.CategoryId==0)
                return RedirectToAction("Index");

            Question question = new Question();
            question.Categ = GetAllCategories();


            var questions = from qs in db.Questions
                            where qs.CategoryId==afisQuestions.CategoryId
                            orderby qs.QuestionDate descending
                            select qs;

            ViewBag.Questions = questions;

            
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
            question.Categ = GetAllCategories();
            return View(question);
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
                        question.QuestionTitle = requestQuestion.QuestionTitle;
                        question.QuestionContent = requestQuestion.QuestionContent;
                        question.Category = requestQuestion.Category;
                        question.CategoryId = requestQuestion.CategoryId;
                        db.SaveChanges();
                        TempData["message"] = "Intrebarea a fost modificat";
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
                db.Questions.Remove(question);
                db.SaveChanges();
                TempData["message"] = "Intrebarea a fost stearsa";
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
                db.Comments.Remove(comment);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost sters";
                return Redirect("/Question/Show/" + comment.QuestionId );
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
                return Redirect("/Question/Show/" + comment.QuestionId );
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
            return View(comment);
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
                        comment.CommentContent = requestComment.CommentContent;
                        db.SaveChanges();
                        TempData["message"] = "Comentariul a fost modificat";
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