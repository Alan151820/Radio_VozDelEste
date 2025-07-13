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
    public class UsuariosController : Controller
    {
        private Radio_VozDelEsteEntities db = new Radio_VozDelEsteEntities();

        // GET: Usuarios
        public ActionResult Index()
        {
            var usuarios = db.Usuarios.Include(u => u.Roles);
            return View(usuarios.ToList());
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            ViewBag.RolID = new SelectList(db.Roles, "RolID", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nombre,Email,Contraseña")] Usuarios usuarios, string CI,string apellido)
        {
            if (ModelState.IsValid)
            {
                usuarios.RolID = 2;
                usuarios.Imagen = "/Images/Usuarios/AvatarDefault1.png";
                db.Usuarios.Add(usuarios);

                // Crear cliente con CI y vinculado al nuevo Usuario
                Clientes cliente = new Clientes
                {
                    UsuarioID = usuarios.UsuarioID,
                    CI = CI,
                    Apellido = apellido,
                    Nombre = usuarios.Nombre,
                    Email = usuarios.Email
                };

                db.Clientes.Add(cliente);
                db.SaveChanges();
                db.SaveChanges();

                return RedirectToAction("Login", "Usuarios");
            }
            return View(usuarios);
        }


        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            ViewBag.RolID = new SelectList(db.Roles, "RolID", "Nombre", usuarios.RolID);
            return View(usuarios);


        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UsuarioID,Nombre,Email,Contraseña")] Usuarios usuarios, HttpPostedFileBase ImagenPerfil)
        {
            if (ModelState.IsValid)
            {
                var usuarioOriginal = db.Usuarios.Find(usuarios.UsuarioID);
                if (usuarioOriginal == null)
                {
                    return HttpNotFound();
                }

                usuarioOriginal.Nombre = usuarios.Nombre;
                usuarioOriginal.Email = usuarios.Email;
                usuarioOriginal.Contraseña = usuarios.Contraseña;


                if (ImagenPerfil != null && ImagenPerfil.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImagenPerfil.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/Usuarios/"), fileName);
                    ImagenPerfil.SaveAs(path);
                    usuarioOriginal.Imagen = "/Images/Usuarios/" + fileName;

                    Session["PhotoUrl"] = usuarioOriginal.Imagen;
                }

                db.SaveChanges();

                return RedirectToAction("MainPage", "Noticias");
            }

            return View(usuarios);
        }


        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var cliente = db.Clientes.FirstOrDefault(c => c.UsuarioID == id);
            if (cliente != null)
                db.Clientes.Remove(cliente);

            var usuario = db.Usuarios.Find(id);
            if (usuario != null)
                db.Usuarios.Remove(usuario);

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
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string contraseña)
        {
            var usuario = db.Usuarios
                .Include("Permisos") // Incluye permisos asociados
                .FirstOrDefault(x => x.Email == email && x.Contraseña == contraseña);

            if (usuario != null)
            {
                Session["UserId"] = usuario.UsuarioID;
                Session["UserName"] = usuario.Nombre;
                Session["PhotoUrl"] = usuario.Imagen;
                Session["Rol"] = usuario.Roles.Nombre.Trim();

                var permisos = usuario.Permisos
                    .Select(p => p.Controlador + "." + p.Accion)
                    .ToList();

                Session["Permisos"] = permisos;

                return RedirectToAction("MainPage", "Noticias"); // Redirige a tu página principal
            }
            else
            {
                ViewBag.Error = "Email o contraseña incorrectos";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
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
        // GET
        // GET: Mostrar formulario con dropdownlists
        public ActionResult AsignarRol()
        {
            ViewBag.Usuarios = new SelectList(db.Usuarios.ToList(), "UsuarioID", "Nombre");
            ViewBag.Roles = new SelectList(db.Roles.ToList(), "RolID", "Nombre");
            return View();
        }

        // POST: Recibir selección y actualizar rol
        [HttpPost]
        public ActionResult AsignarRol(int UsuarioID, int RolID)
        {
            var usuario = db.Usuarios.Find(UsuarioID);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
            }
            else
            {
                var rol = db.Roles.Find(RolID);
                if (rol == null)
                {
                    ModelState.AddModelError("", "Rol no encontrado.");
                }
                else
                {
                    usuario.RolID = rol.RolID;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }



    }
}
