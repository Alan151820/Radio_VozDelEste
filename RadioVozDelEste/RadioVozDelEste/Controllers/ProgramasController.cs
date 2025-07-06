using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RadioVozDelEste.Models;

namespace RadioVozDelEste.Controllers
{
    public class ProgramasController : Controller
    {
        private Radio_VozDelEsteEntities db = new Radio_VozDelEsteEntities();

        // GET: Programas
        public ActionResult Index()
        {
            return View(db.Programas.ToList());
        }

        // GET: Programas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Programas programas = db.Programas.Find(id);
            if (programas == null)
            {
                return HttpNotFound();
            }
            return View(programas);
        }

        // GET: Programas/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RadioVozDelEste.Models.Programas programa, HttpPostedFileBase Imagen)
        {
            if (Imagen != null && Imagen.ContentLength > 0)
            {
                // Nombre único para evitar colisiones
                var fileName = Path.GetFileName(Imagen.FileName);
                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);

                // Ruta física donde se guardará la imagen
                var path = Path.Combine(Server.MapPath("~/Images/Programas"), uniqueName);

                // Guardar archivo en disco
                Imagen.SaveAs(path);

                // Guardar ruta relativa en el modelo para mostrar después
                programa.Imagen = "/Images/Programas/" + uniqueName;
            }

            if (ModelState.IsValid)
            {
                // Guardar en base de datos (ajustá según tu contexto)
                db.Programas.Add(programa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(programa);
        }


        // GET: Programas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Programas programas = db.Programas.Find(id);
            if (programas == null)
            {
                return HttpNotFound();
            }
            return View(programas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Programas programa, HttpPostedFileBase Imagen)
        {
            var original = db.Programas.Find(programa.ProgramaID);

            if (original == null)
                return HttpNotFound();

            if (Imagen != null && Imagen.ContentLength > 0)
            {
                if (!string.IsNullOrEmpty(original.Imagen))
                {
                    var oldPath = Server.MapPath(original.Imagen);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                var fileName = Path.GetFileName(Imagen.FileName);
                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                var ruta = Server.MapPath("~/images/programas");

                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                var path = Path.Combine(ruta, uniqueName);
                Imagen.SaveAs(path);

                original.Imagen = "/images/programas/" + uniqueName;
            }

            original.Nombre = programa.Nombre;
            original.Descripcion = programa.Descripcion;

            db.Entry(original).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        // GET: Programas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Programas programas = db.Programas.Find(id);
            if (programas == null)
            {
                return HttpNotFound();
            }
            return View(programas);
        }

        // POST: Programas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Programas programas = db.Programas.Find(id);
            db.Programas.Remove(programas);
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
