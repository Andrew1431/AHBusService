/*
 * AHBusRoutesController.cs
 *   - Controller for manipulating the model and database of bus route table.
 *   Revisions:
 *     Andrew Hartwig, September 21, 2015.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AHBusService.Models;

namespace AHBusService.Content
{
    public class AHBusRoutesController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // GET: AHBusRoutes
        // Returns the index view with a list of Bus Routes
        public ActionResult Index()
        {
            return View(db.busRoutes.ToList());
        }

        // GET: AHBusRoutes/Details/5
        // Returns a detailed view of a specific entry based on the ID provided. If no id is provided, the site will return error status code 400.
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        // GET: AHBusRoutes/Create
        // Returns a view with empty model to create a new entry.
        public ActionResult Create()
        {
            return View();
        }

        // POST: AHBusRoutes/Create
        // Posting here will create a new bus route entry, and will take you to the details view of that bus route.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busRouteCode,routeName")] busRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                db.busRoutes.Add(busRoute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(busRoute);
        }

        // GET: AHBusRoutes/Edit/5
        // Returns an edit view populated with the model related to the provided ID.
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        // POST: AHBusRoutes/Edit/5
        // Updates the existing record in the database based on the ID provided in the route.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busRouteCode,routeName")] busRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(busRoute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(busRoute);
        }

        // GET: AHBusRoutes/Delete/5
        // Returns a confirmation view to allow the user to confirm deletion of a record.
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        // POST: AHBusRoutes/Delete/5
        // Finalizes delete of record in database, and returns you to index view.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            busRoute busRoute = db.busRoutes.Find(id);
            db.busRoutes.Remove(busRoute);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Garbage disposal method.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
