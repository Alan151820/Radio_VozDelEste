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
    if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

    var noticia = db.Noticias.Include(n => n.Programas).FirstOrDefault(n => n.NoticiaID == id);
    if (noticia == null) return HttpNotFound();

    // Obtener noticias similares: mismas categoría y distintas noticias
    var similares = db.Noticias
                      .Where(n => n.CategoriaID == noticia.CategoriaID && n.NoticiaID != noticia.NoticiaID)
                      .OrderByDescending(n => n.FechaPublicacion)
                      .Take(4)
                      .ToList();

    ViewBag.NoticiasSimilares = similares;

    return View(noticia);
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
public ActionResult Create([Bind(Include = "NoticiaID,Titulo,Contenido,FechaPublicacion,CategoriaID,ProgramaID")] Noticias noticias, HttpPostedFileBase Imagen)
{
    if (ModelState.IsValid)
    {
        if (Imagen != null && Imagen.ContentLength > 0)
        {
                    var fileName = Path.GetFileName(Imagen.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images/Noticias"), fileName);
            Imagen.SaveAs(path);

            noticias.Imagen = "Images/Noticias/" + fileName; // ✅ Ruta relativa para usar en la vista
        }

        db.Noticias.Add(noticias);
        db.SaveChanges();
        return RedirectToAction("MainPage", "Noticias");
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
        public ActionResult Edit(RadioVozDelEste.Models.Noticias noticia, HttpPostedFileBase Imagen)
        {
            if (!ModelState.IsValid)
            {
                return View(noticia);
            }

            var original = db.Noticias.Find(noticia.NoticiaID);
            if (original == null)
                return HttpNotFound();

            if (Imagen != null && Imagen.ContentLength > 0)
            {
                // Eliminar imagen anterior si existe
                if (!string.IsNullOrEmpty(original.Imagen))
                {
                    var oldPath = Server.MapPath("~/" + original.Imagen);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Guardar nueva imagen
                var fileName = Path.GetFileName(Imagen.FileName);
                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                var ruta = Server.MapPath("~/Images/Noticias");

                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                var path = Path.Combine(ruta, uniqueName);
                Imagen.SaveAs(path);

                original.Imagen = "Images/Noticias/" + uniqueName;
            }

            // Actualizar campos
            original.Titulo = noticia.Titulo;
            original.Contenido = noticia.Contenido;
            original.FechaPublicacion = noticia.FechaPublicacion;

            db.Entry(original).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("MainPage", "Noticias");
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
