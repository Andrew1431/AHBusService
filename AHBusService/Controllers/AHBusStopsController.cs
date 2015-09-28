/*
 * AHBusStopsController:
 *   - The controller to manipulate the model and database entries of the bus stops table.
 *   Revision History:
 *       Andrew Hartwig, September 21, 2015
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
    public class AHBusStopsController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        private int GenerateLocationHash(string location)
        {
            int result = 0;
            foreach( char character in location )
            {
                result += (int) character;
            }
            return result;
        }

        // GET: AHBusStops
        // Returns the index view with a list of bus stops.
        public ActionResult Index(String orderBy)
        {
            if (orderBy != null)
            {
                if (orderBy.Equals("busStopNumber"))
                {
                    return View(db.busStops.OrderBy(p => p.busStopNumber).ToList());
                }
                if (orderBy.Equals("location"))
                {
                    return View(db.busStops.OrderBy(p => p.location).ToList());
                }
            }

            return View(db.busStops.ToList());
        }

        // GET: AHBusStops/Details/5
        // Returns detailed information about a specific bus stop, no ID will return a bad request http status code (400 if not specified, 404 if not found)
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // GET: AHBusStops/Create
        // Returns a blank model object in the create view to add a new record to the system.
        public ActionResult Create()
        {
            return View();
        }

        // POST: AHBusStops/Create
        // Post to this to create a busStop entry in the database. The locationHash field is not required and will be overwritten.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busStopNumber,location,locationHash,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                busStop.locationHash = GenerateLocationHash(busStop.location);
                db.busStops.Add(busStop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(busStop);
        }

        // GET: AHBusStops/Edit/5
        // Returns an edit view populated with the model of the passed ID.
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // POST: AHBusStops/Edit/5
        // Posting will update the model and apply the changes to the database. Location hash will be overwritten (auto-generated)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busStopNumber,location,locationHash,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                busStop.locationHash = GenerateLocationHash(busStop.location);
                db.Entry(busStop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(busStop);
        }

        // GET: AHBusStops/Delete/5
        // Gets the delete confirmation view to confirm deleting the entry.
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // POST: AHBusStops/Delete/5
        // Deletes the entry from the data base.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            busStop busStop = db.busStops.Find(id);
            db.busStops.Remove(busStop);
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
