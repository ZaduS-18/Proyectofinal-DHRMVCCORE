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
    public class ObsObrasController : Controller
    {
        private readonly BdDhrContext _context;

        public ObsObrasController(BdDhrContext context)
        {
            _context = context;
        }

        // GET: ObsObras
        public async Task<IActionResult> Index()
        {
            var bdDhrContext = _context.ObsObras.Include(o => o.ActaIdActaNavigation);
            return View(await bdDhrContext.ToListAsync());
        }

        // GET: ObsObras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ObsObras == null)
            {
                return NotFound();
            }

            var obsObra = await _context.ObsObras
                .Include(o => o.ActaIdActaNavigation)
                                .ThenInclude(a => a.EspecialidadIdEspecialidadNavigation)
                .Include(j => j.ActaIdActaNavigation.RazonSocialIdRazonSocialNavigation)
                .Include(j => j.ActaIdActaNavigation.ObraIdObraNavigation)
                .FirstOrDefaultAsync(m => m.IdObservacion == id);
            if (obsObra == null)
            {
                return NotFound();
            }

            return View(obsObra);
        }

        // GET: ObsObras/Create
        public IActionResult Create()
        {
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa");
            return View();
        }

        // POST: ObsObras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdObservacion,Nombre,Compromisos,Firma,ActaIdActa")] ObsObra obsObra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obsObra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", obsObra.ActaIdActa);
            return View(obsObra);
        }

        // GET: ObsObras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ObsObras == null)
            {
                return NotFound();
            }

            // Incluye las relaciones de navegación 
            var obsObra = await _context.ObsObras
                .Include(o => o.ActaIdActaNavigation)
                .ThenInclude(a => a.EspecialidadIdEspecialidadNavigation)
                .Include(o => o.ActaIdActaNavigation.RazonSocialIdRazonSocialNavigation)
                .Include(o => o.ActaIdActaNavigation.ObraIdObraNavigation)
                .FirstOrDefaultAsync(m => m.IdObservacion == id); // Suponiendo que el ID se llama IdObsObra

            if (obsObra == null)
            {
                return NotFound();
            }

            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", obsObra.ActaIdActa);
            return View(obsObra);
        }
        public IActionResult RedirectToListarActums()
        {
            return RedirectToAction("ListarActas", "Actums");
        }
        // POST: ObsObras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("IdObservacion,Nombre,Compromisos,Firma,ActaIdActa")] ObsObra obsObra)
        {
            if (id != obsObra.IdObservacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(obsObra);

                    // Buscar la acta asociada a la firma
                    Actum actaAsociada = await _context.Acta.FindAsync(obsObra.ActaIdActa);

                    if (actaAsociada != null)
                    {
                        // Actualizar el atributo "EstadoActa" a 1
                        actaAsociada.EstadoActa = 1;
                        _context.Update(actaAsociada);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObsObraExists(obsObra.IdObservacion))
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

            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", obsObra.ActaIdActa);
            return View(obsObra);
        }

        // GET: ObsObras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ObsObras == null)
            {
                return NotFound();
            }

            var obsObra = await _context.ObsObras
                .Include(o => o.ActaIdActaNavigation)
                .FirstOrDefaultAsync(m => m.IdObservacion == id);
            if (obsObra == null)
            {
                return NotFound();
            }

            return View(obsObra);
        }

        // POST: ObsObras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ObsObras == null)
            {
                return Problem("Entity set 'BdDhrContext.ObsObras'  is null.");
            }
            var obsObra = await _context.ObsObras.FindAsync(id);
            if (obsObra != null)
            {
                _context.ObsObras.Remove(obsObra);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObsObraExists(int id)
        {
          return (_context.ObsObras?.Any(e => e.IdObservacion == id)).GetValueOrDefault();
        }
    }
}
