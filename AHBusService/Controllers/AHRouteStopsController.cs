/* 
 * AHRouteStopsController.cs
 * Controller for managing route stop actions
 * Revisions:
 * Andrew Hartwig, September 24, 2015
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

namespace AHBusService.Controllers
{
    public class AHRouteStopsController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // GET: AHRouteStops
        // Get on index without a routecode query string or existing cookie will redirect you to bus routes, otherwise filters by routeCode
        public ActionResult Index(int? routeCode)
        {
            if (routeCode != null)
            {
                HttpCookie routeCodeCookie = new HttpCookie("routeCode", routeCode.ToString());
                Response.Cookies.Add(routeCodeCookie);
            }
            else if (Request.Cookies["routeCode"] != null)
            {
                routeCode = int.Parse(Request.Cookies["routeCode"].Value);
            }
            else
            {
                TempData["message"] = "Please select a bus route to see its stops!";
                return RedirectToAction("Index", "AHBusRoutes");
            }

            busRoute b = db.busRoutes.Find(routeCode.ToString());

            ViewData["busRouteCode"] = b.busRouteCode;
            ViewData["busRouteName"] = b.routeName;

            var routeStops = db.routeStops.Where(r => r.busRouteCode == routeCode.ToString()).Include(r => r.busRoute).Include(r => r.busStop);
            return View(routeStops.ToList());
        }

        // GET: AHRouteStops/Details/5
        // Gets details on specific route stop.
        public ActionResult Details(int? id)
        {
            string routeCode;
            if (Request.Cookies["routeCode"] != null)
            {
                routeCode = Request.Cookies["routeCode"].Value;
            }
            else
            {
                TempData["message"] = "Please select a bus route.";
                return RedirectToAction("Index", "AHBusRoutes");
            }

            busRoute b = db.busRoutes.Find(routeCode.ToString());

            ViewData["busRouteCode"] = b.busRouteCode;
            ViewData["busRouteName"] = b.routeName;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            return View(routeStop);
        }

        // GET: AHRouteStops/Create
        // Gets create view for a new route stop.
        public ActionResult Create()
        {
            string routeCode;
            if (Request.Cookies["routeCode"] != null)
            {
                routeCode = Request.Cookies["routeCode"].Value;
            }
            else
            {
                TempData["message"] = "Please select a bus route.";
                return RedirectToAction("Index", "AHBusRoutes");
            }

            busRoute b = db.busRoutes.Find(routeCode);

            ViewData["busRouteName"] = b.routeName;
            ViewData["routeCode"] = b.busRouteCode;

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName");
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location");
            return View(new routeStop()
            {
                busRouteCode = b.busRouteCode
            });
        }

        // POST: AHRouteStops/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Posts a new routeStop to the data base.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routeStop)
        {
            if (ModelState.IsValid)
            {
                db.routeStops.Add(routeStop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        // GET: AHRouteStops/Edit/5
        // Returns edit view for editing an existing route stop. ID is required.
        public ActionResult Edit(int? id)
        {
            string routeCode;
            if (Request.Cookies["routeCode"] != null)
            {
                routeCode = Request.Cookies["routeCode"].Value;
            }
            else
            {
                TempData["message"] = "Please select a bus route.";
                return RedirectToAction("Index", "AHBusRoutes");
            }

            busRoute b = db.busRoutes.Find(routeCode);

            ViewData["busRouteName"] = b.routeName;
            ViewData["routeCode"] = b.busRouteCode;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        // POST: AHRouteStops/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Posts changes to the database of route stop.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routeStop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(routeStop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        // GET: AHRouteStops/Delete/5
        // Gets  the delete confirmation view for a route stop.
        public ActionResult Delete(int? id)
        {
            string routeCode;
            if (Request.Cookies["routeCode"] != null)
            {
                routeCode = Request.Cookies["routeCode"].Value;
            }
            else
            {
                TempData["message"] = "Please select a bus route.";
                return RedirectToAction("Index", "AHBusRoutes");
            }

            busRoute b = db.busRoutes.Find(routeCode);

            ViewData["busRouteName"] = b.routeName;
            ViewData["busRouteCode"] = b.busRouteCode;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            return View(routeStop);
        }

        // POST: AHRouteStops/Delete/5
        // Deletes a routestop from the DB
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeStop routeStop = db.routeStops.Find(id);
            db.routeStops.Remove(routeStop);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Garbage disposal.
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
