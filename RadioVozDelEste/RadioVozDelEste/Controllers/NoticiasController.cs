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
    public class NoticiasController : Controller
    {
        private Radio_VozDelEsteEntities db = new Radio_VozDelEsteEntities();

        // GET: Noticias
        public ActionResult MainPage()
        {
            var noticias = db.Noticias
         .Include(n => n.Categorias)
         .Include(n => n.Programas)
         .ToList()
         .GroupBy(n => n.Categorias.Nombre)
         .ToList();

            return View(noticias);
        }
        public ActionResult PorCategoria(string categoria)
        {
            var noticiasFiltradas = db.Noticias
                .Include(n => n.Categorias)
                .Include(n => n.Programas)
                .Where(n => n.Categorias.Nombre == categoria)
                .OrderByDescending(n => n.FechaPublicacion)
                .ToList();

            ViewBag.CategoriaSeleccionada = categoria;
            return View(noticiasFiltradas);
        }
        public ActionResult PorPrograma(string programa)
        {
            var noticiasPorPrograma = db.Noticias
                .Include(n => n.Categorias)
                .Include(n => n.Programas)
                .Where(n => n.Programas.Nombre == programa)
                .OrderByDescending(n => n.FechaPublicacion)
                .ToList();

            ViewBag.CategoriaSeleccionada = programa;
            return View(noticiasPorPrograma);
        }

        // GET: Noticias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Noticias noticias = db.Noticias.Find(id);
            if (noticias == null)
            {
                return HttpNotFound();
            }
            return View(noticias);
        }

        // GET: Noticias/Create
        public ActionResult Create()
        {
            ViewBag.Categorias = db.Categorias.ToList();
            ViewBag.Programas = db.Programas.ToList();
            return View();
        }

        // POST: Noticias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NoticiaID,Titulo,Contenido,FechaPublicacion,Imagen,CategoriaID,ProgramaID")] Noticias noticias)
        {
            if (ModelState.IsValid)
            {
                db.Noticias.Add(noticias);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categorias = db.Categorias.ToList();
            ViewBag.Programas = db.Programas.ToList();

            return View(noticias);
        }

        // GET: Noticias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Noticias noticias = db.Noticias.Find(id);
            if (noticias == null)
            {
                return HttpNotFound();
            }
            return View(noticias);
        }

        // POST: Noticias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NoticiaID,Titulo,Contenido,FechaPublicacion,Imagen")] Noticias noticias)
        {
            if (ModelState.IsValid)
            {
                db.Entry(noticias).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(noticias);
        }

        // GET: Noticias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Noticias noticias = db.Noticias.Find(id);
            if (noticias == null)
            {
                return HttpNotFound();
            }
            return View(noticias);
        }

        // POST: Noticias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Noticias noticias = db.Noticias.Find(id);
            db.Noticias.Remove(noticias);
            db.SaveChanges();
            return RedirectToAction("MainPage","Noticias");
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
