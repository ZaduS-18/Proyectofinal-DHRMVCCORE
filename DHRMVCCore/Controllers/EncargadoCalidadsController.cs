using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DHRMVCCore.Models;
using Microsoft.AspNetCore.Authorization;

namespace DHRMVCCore.Controllers
{
    [Authorize]
    public class EncargadoCalidadsController : Controller
    {
        private readonly BdDhrContext _context;

        public EncargadoCalidadsController(BdDhrContext context)
        {
            _context = context;
        }

        // GET: EncargadoCalidads
        public async Task<IActionResult> Index()
        {
            var bdDhrContext = _context.EncargadoCalidads.Include(e => e.ActaIdActaNavigation);
            return View(await bdDhrContext.ToListAsync());
        }

        // GET: EncargadoCalidads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EncargadoCalidads == null)
            {
                return NotFound();
            }

            var encargadoCalidad = await _context.EncargadoCalidads
                .Include(e => e.ActaIdActaNavigation)
                                .ThenInclude(a => a.EspecialidadIdEspecialidadNavigation)
                .Include(j => j.ActaIdActaNavigation.RazonSocialIdRazonSocialNavigation)
                .Include(j => j.ActaIdActaNavigation.ObraIdObraNavigation)
                .FirstOrDefaultAsync(m => m.IdEncargadocali == id);
            if (encargadoCalidad == null)
            {
                return NotFound();
            }

            return View(encargadoCalidad);
        }

        // GET: EncargadoCalidads/Create
        public IActionResult Create()
        {
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa");
            return View();
        }

        // POST: EncargadoCalidads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEncargadocali,Nombre,SupObra,CumEspTec,TrabjProtocolo,CerEnsayo,Calidad,TrabjTermOtr,Observacion,Firma,ActaIdActa")] EncargadoCalidad encargadoCalidad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(encargadoCalidad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", encargadoCalidad.ActaIdActa);
            return View(encargadoCalidad);
        }

        // GET: EncargadoCalidads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EncargadoCalidads == null)
            {
                return NotFound();
            }

            // Incluye las relaciones de navegación 
            var encargadoCalidad = await _context.EncargadoCalidads
                .Include(e => e.ActaIdActaNavigation)
                .ThenInclude(a => a.EspecialidadIdEspecialidadNavigation)
                .Include(e => e.ActaIdActaNavigation.RazonSocialIdRazonSocialNavigation)
                .Include(e => e.ActaIdActaNavigation.ObraIdObraNavigation)
                .FirstOrDefaultAsync(m => m.IdEncargadocali == id); // Suponiendo que el ID se llama IdEncargadoCalidad

            if (encargadoCalidad == null)
            {
                return NotFound();
            }

            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", encargadoCalidad.ActaIdActa);
            return View(encargadoCalidad);
        }

        public IActionResult RedirectToListarActums()
        {
            return RedirectToAction("ListarActas", "Actums");
        }
        // POST: EncargadoCalidads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("IdEncargadocali,Nombre,SupObra,CumEspTec,TrabjProtocolo,CerEnsayo,Calidad,TrabjTermOtr,Observacion,Firma,ActaIdActa")] EncargadoCalidad encargadoCalidad)
        {
            if (id != encargadoCalidad.IdEncargadocali)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(encargadoCalidad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EncargadoCalidadExists(encargadoCalidad.IdEncargadocali))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(RedirectToListarActums));
            }
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", encargadoCalidad.ActaIdActa);
            return View(encargadoCalidad);
        }

        // GET: EncargadoCalidads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EncargadoCalidads == null)
            {
                return NotFound();
            }

            var encargadoCalidad = await _context.EncargadoCalidads
                .Include(e => e.ActaIdActaNavigation)
                .FirstOrDefaultAsync(m => m.IdEncargadocali == id);
            if (encargadoCalidad == null)
            {
                return NotFound();
            }

            return View(encargadoCalidad);
        }

        // POST: EncargadoCalidads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EncargadoCalidads == null)
            {
                return Problem("Entity set 'BdDhrContext.EncargadoCalidads'  is null.");
            }
            var encargadoCalidad = await _context.EncargadoCalidads.FindAsync(id);
            if (encargadoCalidad != null)
            {
                _context.EncargadoCalidads.Remove(encargadoCalidad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EncargadoCalidadExists(int id)
        {
          return (_context.EncargadoCalidads?.Any(e => e.IdEncargadocali == id)).GetValueOrDefault();
        }
    }
}
