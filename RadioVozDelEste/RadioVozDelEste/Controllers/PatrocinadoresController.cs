using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RadioVozDelEste.Models;

namespace RadioVozDelEste.Controllers
{
    public class PatrocinadoresController : Controller
    {
        private Radio_VozDelEsteEntities db = new Radio_VozDelEsteEntities();

        // GET: Patrocinadores
        public ActionResult Index()
        {
            var patrocinadores = db.Patrocinadores.Include(p => p.Planes);
            return View(patrocinadores.ToList());
        }

        // GET: Patrocinadores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrocinadores patrocinadores = db.Patrocinadores.Find(id);
            if (patrocinadores == null)
            {
                return HttpNotFound();
            }
            return View(patrocinadores);
        }

        // GET: Patrocinadores/Create
        public ActionResult Create()
        {
            ViewBag.PlanID = new SelectList(db.Planes, "PlanID", "Nombre");
            return View();
        }

        // POST: Patrocinadores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PatrocinadorID,Nombre,Descripcion,PlanID")] Patrocinadores patrocinadores)
        {
            if (ModelState.IsValid)
            {
                db.Patrocinadores.Add(patrocinadores);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PlanID = new SelectList(db.Planes, "PlanID", "Nombre", patrocinadores.PlanID);
            return View(patrocinadores);
        }

        // GET: Patrocinadores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrocinadores patrocinadores = db.Patrocinadores.Find(id);
            if (patrocinadores == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlanID = new SelectList(db.Planes, "PlanID", "Nombre", patrocinadores.PlanID);
            return View(patrocinadores);
        }

        // POST: Patrocinadores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatrocinadorID,Nombre,Descripcion,PlanID")] Patrocinadores patrocinadores)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patrocinadores).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlanID = new SelectList(db.Planes, "PlanID", "Nombre", patrocinadores.PlanID);
            return View(patrocinadores);
        }

        // GET: Patrocinadores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrocinadores patrocinadores = db.Patrocinadores.Find(id);
            if (patrocinadores == null)
            {
                return HttpNotFound();
            }
            return View(patrocinadores);
        }

        // POST: Patrocinadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patrocinadores patrocinadores = db.Patrocinadores.Find(id);
            db.Patrocinadores.Remove(patrocinadores);
            db.SaveChanges();
            return RedirectToAction("Index");
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
