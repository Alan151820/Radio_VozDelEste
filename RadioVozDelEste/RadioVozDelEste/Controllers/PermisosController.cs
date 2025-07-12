using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RadioVozDelEste.Models
{
    public class PermisosController : Controller
    {
        private Radio_VozDelEsteEntities db = new Radio_VozDelEsteEntities();

        // GET: Permisos
        public ActionResult Index()
        {
            return View(db.Permisos.ToList());
        }

        // GET: Permisos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permisos permisos = db.Permisos.Find(id);
            if (permisos == null)
            {
                return HttpNotFound();
            }
            return View(permisos);
        }

    
        // GET: Permisos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permisos permisos = db.Permisos.Find(id);
            if (permisos == null)
            {
                return HttpNotFound();
            }
            return View(permisos);
        }

        // POST: Permisos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PermisoID,Descripcion")] Permisos permisos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permisos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(permisos);
        }

        // GET: Permisos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permisos permisos = db.Permisos.Find(id);
            if (permisos == null)
            {
                return HttpNotFound();
            }
            return View(permisos);
        }

        // POST: Permisos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Permisos permisos = db.Permisos.Find(id);
            db.Permisos.Remove(permisos);
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
        public ActionResult EditarPermisos(int id)
        {
            var usuario = db.Usuarios.Include(u => u.Permisos).FirstOrDefault(u => u.UsuarioID == id);
            if (usuario == null)
                return HttpNotFound();

            var todosPermisos = db.Permisos.ToList();

            ViewBag.TodosPermisos = todosPermisos;

            return View(usuario);
        }
        [HttpPost]
        public ActionResult EditarPermisos(int UsuarioID, int[] PermisosSeleccionados)
        {
            var usuario = db.Usuarios.Include(u => u.Permisos).FirstOrDefault(u => u.UsuarioID == UsuarioID);
            if (usuario == null)
                return HttpNotFound();

            PermisosSeleccionados = PermisosSeleccionados ?? new int[0];

            var permisosAEliminar = usuario.Permisos
                .Where(p => !PermisosSeleccionados.Contains(p.PermisoID))
                .ToList();

            foreach (var permiso in permisosAEliminar)
            {
                usuario.Permisos.Remove(permiso);
            }

            foreach (var permisoId in PermisosSeleccionados)
            {
                if (!usuario.Permisos.Any(p => p.PermisoID == permisoId))
                {
                    var permiso = db.Permisos.Find(permisoId);
                    if (permiso != null)
                    {
                        usuario.Permisos.Add(permiso);
                    }
                }
            }

            db.SaveChanges();
            TempData["Mensaje"] = "Los permisos fueron actualizados correctamente.";
            return RedirectToAction("Index", "Usuarios");
        }

    }
}
