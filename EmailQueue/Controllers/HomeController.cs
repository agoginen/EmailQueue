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

            var data = db.emailQueues.ToList();

            data.Sort();

            return View(data);
        }

        public ActionResult Mail()
        {
            ViewBag.Message = "Mails are Being sent";

            myfunctions myqueue = new myfunctions();

            var passData = new Queue<object>();

            passData = myqueue.enqueue();

            myqueue.dequeue(passData);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}