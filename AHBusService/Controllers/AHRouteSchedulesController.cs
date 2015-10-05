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
    public class AHRouteSchedulesController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // GET: AHRouteSchedules
        public ActionResult Index()
        {
            var routeSchedules = db.routeSchedules.Include(r => r.busRoute);
            return View(routeSchedules.ToList());
        }

        // GET: AHRouteSchedules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        // GET: AHRouteSchedules/Create
        public ActionResult Create()
        {
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName");
            return View();
        }

        // POST: AHRouteSchedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.routeSchedules.Add(routeSchedule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // GET: AHRouteSchedules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // POST: AHRouteSchedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(routeSchedule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // GET: AHRouteSchedules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        // POST: AHRouteSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            db.routeSchedules.Remove(routeSchedule);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Returns a view with a time list of all the times the selected bus route and bus will get there.
        // If routeStopId is not supplied (by finding one routeStop in busStopsController, checks for session variable and busRouteCode from the routeselector.
        // If neither of those exist we will return to bus stop index with an error.
        public ActionResult RouteStopSchedule(int? routeStopId, string busRoutes)
        {
            routeStop routeStop;
            try
            {
                if (routeStopId == null)
                {
                    if (busRoutes == null)
                    {
                        throw new Exception("Please select a bus route!");
                    }
                    else if (Session["busStopNumber"] == null)
                    {
                        throw new Exception("Please select a bus stop to view the Schedule.");
                    }

                    String busRouteId = busRoutes.ToString();
                    int busStopNumber = int.Parse(Session["busStopNumber"].ToString());

                    routeStop = db.routeStops.Where(rs => rs.busRouteCode == busRouteId && rs.busStopNumber == busStopNumber).First();
                }
                else
                {
                    routeStop = db.routeStops.Find(routeStopId);
                }

                if (routeStop == null)
                {
                    throw new Exception("Error finding route stop for that location.");
                }

                var routeSchedules = db.routeSchedules.Where(s => s.busRouteCode == routeStop.busRouteCode).OrderBy(s => s.startTime);

                if (routeSchedules.ToList().Count == 0)
                {
                    throw new Exception("There are no schedules for that route.");
                }

                double minutes = (double)routeStop.offsetMinutes;
                TimeSpan offsetMinutes = TimeSpan.FromMinutes(minutes);
                ViewBag.offsetMinutes = offsetMinutes;
                ViewBag.busStopLocation = routeStop.busStop.location;
                ViewBag.busStopNumber = routeStop.busStop.busStopNumber;

                return View(routeSchedules);
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return RedirectToAction("Index", "AHBusStops");
        }

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
