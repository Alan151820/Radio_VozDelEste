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
    public class ConductoresController : Controller
    {
        private Radio_VozDelEsteEntities db = new Radio_VozDelEsteEntities();

        // GET: Conductores
        public ActionResult Index()
        {
            return View(db.Conductores.ToList());
        }

        // GET: Conductores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conductores conductores = db.Conductores.Find(id);
            if (conductores == null)
            {
                return HttpNotFound();
            }
            return View(conductores);
        }

        // GET: Conductores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Conductores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ConductorID,Nombre,Descripcion")] Conductores conductores)
        {
            if (ModelState.IsValid)
            {
                db.Conductores.Add(conductores);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(conductores);
        }

        // GET: Conductores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conductores conductores = db.Conductores.Find(id);
            if (conductores == null)
            {
                return HttpNotFound();
            }
            return View(conductores);
        }

        // POST: Conductores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ConductorID,Nombre,Descripcion")] Conductores conductores)
        {
            if (ModelState.IsValid)
            {
                db.Entry(conductores).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(conductores);
        }

        // GET: Conductores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conductores conductores = db.Conductores.Find(id);
            if (conductores == null)
            {
                return HttpNotFound();
            }
            return View(conductores);
        }

        // POST: Conductores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Conductores conductores = db.Conductores.Find(id);
            db.Conductores.Remove(conductores);
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
        public ActionResult AsignarConductor()
        {
            ViewBag.Programas = new SelectList(db.Programas.ToList(), "ProgramaID", "Nombre");
            ViewBag.Conductores = new SelectList(db.Conductores.ToList(), "ConductorID", "Nombre");

            return View();
        }

        [HttpPost]
        public ActionResult AsignarConductor(int programaId, int conductorId)
        {
            var programa = db.Programas
                .Include("Conductores")
                .FirstOrDefault(x => x.ProgramaID == programaId);

            var conductor = db.Conductores.Find(conductorId);

            if (programa == null || conductor == null)
                return HttpNotFound();

            if (!programa.Conductores.Contains(conductor))
            {
                programa.Conductores.Add(conductor);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }


}

