using System.Web.Mvc;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Agrimanagr/Test/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Agrimanagr/Test/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Agrimanagr/Test/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Agrimanagr/Test/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Agrimanagr/Test/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Agrimanagr/Test/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Agrimanagr/Test/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Agrimanagr/Test/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
