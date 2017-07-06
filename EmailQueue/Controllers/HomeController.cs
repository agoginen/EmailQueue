using EmailQueue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmailQueue.Controllers
{
    public class HomeController : Controller
    {
        protected DiversityTraxEntities db;

        public ActionResult Index()
        {
            db = new DiversityTraxEntities();

            var data = db.emailQueue.OrderBy(x => x.EPriority).ToList();

            return View(data);
        }

        public ActionResult Mail()
        {
            ViewBag.Message = "Mails are Being sent";

            int[] postData;

            myfunctions myqueue = new myfunctions();

            Queue<emailQueue> passData = new Queue<emailQueue>();

            passData = myqueue.enqueue();

            postData = myqueue.dequeue(passData);
            

            return View(postData);
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Fails()
        {
            return View();
        }
    }
}