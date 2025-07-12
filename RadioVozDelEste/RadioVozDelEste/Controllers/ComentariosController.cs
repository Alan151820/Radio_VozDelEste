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
    public class ComentariosController : Controller
    {
        private Radio_VozDelEsteEntities db = new Radio_VozDelEsteEntities();

        public ActionResult Index()
        {
            int? userId = Session["UserId"] as int?;

            Clientes clienteActual = null;
            if (userId != null)
            {
                clienteActual = db.Clientes.FirstOrDefault(c => c.UsuarioID == userId);
            }

            ViewBag.ClienteActual = clienteActual; 
            ViewBag.Programas = db.Programas.ToList();

            var comentarios = db.Comentarios
                .Include(c => c.Clientes)
                .Include(c => c.Programas)
                .OrderByDescending(c => c.Fecha)
                .ToList();

            return View(comentarios);
        }


        // GET: Comentarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comentarios comentarios = db.Comentarios.Find(id);
            if (comentarios == null)
            {
                return HttpNotFound();
            }
            return View(comentarios);
        }

        // GET: Comentarios/Create
        public ActionResult Create()
        {
            ViewBag.ClienteID = new SelectList(db.Clientes, "CI", "Nombre");
            ViewBag.ProgramaID = new SelectList(db.Programas, "ProgramaID", "Nombre");
            return View();
        }

        // POST: Comentarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComentarioID,Comentario,ClienteID,ProgramaID")] Comentarios comentarios)
        {
            if (ModelState.IsValid)
            {
                comentarios.Fecha = DateTime.Now; // Setear fecha actual

                db.Comentarios.Add(comentarios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClienteID = new SelectList(db.Clientes, "CI", "Nombre", comentarios.ClienteID);
            ViewBag.ProgramaID = new SelectList(db.Programas, "ProgramaID", "Nombre", comentarios.ProgramaID);
            return View(comentarios);
        }

        // GET: Comentarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comentarios comentarios = db.Comentarios.Find(id);
            if (comentarios == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClienteID = new SelectList(db.Clientes, "CI", "Nombre", comentarios.ClienteID);
            ViewBag.ProgramaID = new SelectList(db.Programas, "ProgramaID", "Nombre", comentarios.ProgramaID);
            return View(comentarios);
        }

        // POST: Comentarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComentarioID,Comentario,Fecha,ClienteID,ProgramaID")] Comentarios comentarios)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comentarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClienteID = new SelectList(db.Clientes, "CI", "Nombre", comentarios.ClienteID);
            ViewBag.ProgramaID = new SelectList(db.Programas, "ProgramaID", "Nombre", comentarios.ProgramaID);
            return View(comentarios);
        }

        // GET: Comentarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comentarios comentarios = db.Comentarios.Find(id);
            if (comentarios == null)
            {
                return HttpNotFound();
            }
            return View(comentarios);
        }

        // POST: Comentarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comentarios comentarios = db.Comentarios.Find(id);
            db.Comentarios.Remove(comentarios);
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
