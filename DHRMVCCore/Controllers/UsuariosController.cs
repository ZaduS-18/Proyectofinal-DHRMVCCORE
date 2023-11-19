using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DHRMVCCore.Models;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace DHRMVCCore.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly BdDhrContext _context;

        public UsuariosController(BdDhrContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal c = HttpContext.User;
            if (c.Identity != null)
            {
                if (c.Identity.IsAuthenticated)
                    return RedirectToAction("ListarActas", "Actums");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Usuario u)
        {
            try
            {
                // Encriptar la contraseña ingresada
                string contrasenaEncriptada = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: u.Contrasena,
                    salt: new byte[128 / 8], // Asegúrate de manejar la sal correctamente
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                // Buscar el usuario en la base de datos usando EF Core
                var usuarioEncontrado = await _context.Usuarios
                    .Include(usuario => usuario.RolIdRolNavigation)
                    .Where(usuario => usuario.Correo == u.Correo && usuario.Contrasena == contrasenaEncriptada)
                    .FirstOrDefaultAsync();

                if (usuarioEncontrado != null)
                {
                    // Verificar si el usuario está habilitado
                    if (!usuarioEncontrado.Habilitado)
                    {
                        ViewBag.Error = "La cuenta de usuario no está habilitada.";
                        return View();
                    }

                    // Crear los claims del usuario
                    List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, u.Correo),
                new Claim("Admin", usuarioEncontrado.Admin ? "True" : "False"),
                new Claim("RolIdRol", usuarioEncontrado.RolIdRolNavigation?.IdRol.ToString() ?? string.Empty),
                new Claim("PrimerNombre", usuarioEncontrado.PrimerNombre ?? string.Empty),
                new Claim("PrimerApellido", usuarioEncontrado.PrimerApellido ?? string.Empty)
            };

                    // Crear la identidad y la cookie de autenticación
                    ClaimsIdentity ci = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties p = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = u.MantenerActivo,
                        ExpiresUtc = u.MantenerActivo ? DateTimeOffset.UtcNow.AddDays(1) : DateTimeOffset.UtcNow.AddSeconds(30)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(ci), p);
                    return RedirectToAction("ListarActas", "Actums");
                }
                else
                {
                    ViewBag.Error = "Credenciales Incorrectas o No registradas";
                    return View();
                }
            }
            catch (System.Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }
        }



        //GET: Usuarios
        public async Task<IActionResult> ListarUsuarios()
        {
            var bdDhrContext = _context.Usuarios.Include(u => u.RolIdRolNavigation);
            return View(await bdDhrContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.RolIdRolNavigation)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            //ViewData["ObraIdObra"] = new SelectList(_context.Obras, "IdObra", "IdObra");
            ViewData["RolIdRol"] = new SelectList(_context.Rols, "IdRol", "IdRol");
            //ViewData["ObraIdObra"] = new SelectList(_context.Obras, "IdObra", "NombreObra");
            ViewData["ObraIdObra"] = new SelectList(_context.Obras, "IdObra", "NombreObra");

            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public async Task<IActionResult> registrarseDHR([Bind("IdUsuario,PrimerNombre,SegundoNombre,PrimerApellido,SegundoApellido,Telefono,Rut,Direccion,Admin,Contrasena,Correo,FotoP,RolIdRol,Habilitado,ObraIdObra,UsuarioId,ObraIdObrasSeleccionadas")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                string encriptedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: usuario.Contrasena,
                    salt: new byte[128 / 8],
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                usuario.Contrasena = encriptedPassword;

                usuario.UsuarioId = usuario.IdUsuario;
                // Limpiar la lista existente para evitar duplicados
                usuario.ObraIdObras.Clear();
                // Añadir las obras seleccionadas al usuario
                if (usuario.ObraIdObrasSeleccionadas != null)
                {
                    foreach (var obraId in usuario.ObraIdObrasSeleccionadas)
                    {
                        var obra = await _context.Obras.FindAsync(obraId);
                        if (obra != null)
                        {
                            usuario.ObraIdObras.Add(obra);
                        }
                    }
                }

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListarUsuarios", "Usuarios");
            }
            ViewData["ObraIdObra"] = new SelectList(_context.Obras, "IdObra", "NombreObra", usuario.ObraIdObrasSeleccionadas);
            ViewData["RolIdRol"] = new SelectList(_context.Rols, "IdRol", "NombreRol", usuario.RolIdRol);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            //ViewData["ObraIdObra"] = new SelectList(_context.Obras, "IdObra", "IdObra", usuario.ObraIdObra);
            ViewData["RolIdRol"] = new SelectList(_context.Rols, "IdRol", "IdRol", usuario.RolIdRol);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("IdUsuario,PrimerNombre,SegundoNombre,PrimerApellido,SegundoApellido,Telefono,Rut,Direccion,Admin,Contrasena,Correo,FotoP,RolIdRol,Habilitado,ObraIdObra,UsuarioId")] Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["ObraIdObra"] = new SelectList(_context.Obras, "IdObra", "IdObra", usuario.ObraIdObra);
            ViewData["RolIdRol"] = new SelectList(_context.Rols, "IdRol", "IdRol", usuario.RolIdRol);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                //.Include(u => u.ObraIdObraNavigation)
                .Include(u => u.RolIdRolNavigation)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'BdDhrContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Habilitar(decimal id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Habilitado = true;
                await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            }
            return RedirectToAction(nameof(ListarUsuarios));
        }

        public async Task<IActionResult> Deshabilitar(decimal id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Habilitado = false;
                await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            }
            return RedirectToAction(nameof(ListarUsuarios));
        }

        public async Task<IActionResult> HacerAdmin(decimal id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Admin = true; // Habilita el rol de administrador
                await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            }
            return RedirectToAction(nameof(ListarUsuarios));
        }
        public async Task<IActionResult> QuitarAdmin(decimal id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Admin = false; // Deshabilita el rol de administrador
                await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            }
            return RedirectToAction(nameof(ListarUsuarios));
        }

        private bool UsuarioExists(decimal id)
        {
          return (_context.Usuarios?.Any(e => e.UsuarioId == id)).GetValueOrDefault();
        }
    }
}
