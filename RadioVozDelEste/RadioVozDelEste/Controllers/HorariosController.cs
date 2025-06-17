using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RadioVozDelEste.Models;

namespace RadioVozDelEste.Controllers
{
    public class HorariosController : Controller
    {
        private Radio_VozDelEsteEntities db = new Radio_VozDelEsteEntities();

        // GET: Horarios
        public ActionResult Index()
        {
            var GrillaSemanal = db.Horarios
                .Include(h => h.Programas.Conductores)
                .Include(h => h.Dias)
                .OrderBy(h => h.DiaID)
                .ThenBy(h => h.HoraInicio)
                .ToList();

            var gruposPorDia = GrillaSemanal.GroupBy(h => h.Dias.Nombre);
            return View(gruposPorDia);

        }
        // GET: Horarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var horario = db.Horarios
                            .Include(h => h.Dias)
                            .Include(h => h.Programas)
                            .FirstOrDefault(h => h.HorarioID == id);

            if (horario == null)
            {
                return HttpNotFound();
            }
            return View(horario);
        }



        // GET: Horarios/Create
        public ActionResult Create()
        {
            ViewBag.DiaID = new SelectList(db.Dias, "DiaID", "Nombre");
            ViewBag.ProgramaID = new SelectList(db.Programas, "ProgramaID", "Nombre");
            return View();
        }

        // POST: Horarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HorarioID,ProgramaID,DiaID,HoraInicio,HoraFin")] Horarios horarios)
        {
            if (ModelState.IsValid)
            {
                db.Horarios.Add(horarios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DiaID = new SelectList(db.Dias, "DiaID", "Nombre", horarios.DiaID);
            ViewBag.ProgramaID = new SelectList(db.Programas, "ProgramaID", "Nombre", horarios.ProgramaID);
            return View(horarios);
        }

        // GET: Horarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Horarios horarios = db.Horarios.Find(id);
            if (horarios == null)
            {
                return HttpNotFound();
            }
            ViewBag.DiaID = new SelectList(db.Dias, "DiaID", "Nombre", horarios.DiaID);
            ViewBag.ProgramaID = new SelectList(db.Programas, "ProgramaID", "Nombre", horarios.ProgramaID);
            return View(horarios);
        }

        // POST: Horarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HorarioID,ProgramaID,DiaID,HoraInicio,HoraFin")] Horarios horarios)
        {
            if (ModelState.IsValid)
            {
                db.Entry(horarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DiaID = new SelectList(db.Dias, "DiaID", "Nombre", horarios.DiaID);
            ViewBag.ProgramaID = new SelectList(db.Programas, "ProgramaID", "Nombre", horarios.ProgramaID);
            return View(horarios);
        }

        // GET: Horarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Horarios horarios = db.Horarios.Find(id);
            if (horarios == null)
            {
                return HttpNotFound();
            }
            return View(horarios);
        }

        // POST: Horarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Horarios horarios = db.Horarios.Find(id);
            db.Horarios.Remove(horarios);
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
