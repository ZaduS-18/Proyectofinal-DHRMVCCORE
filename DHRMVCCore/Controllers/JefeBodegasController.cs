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
    public class JefeBodegasController : Controller
    {
        private readonly BdDhrContext _context;

        public JefeBodegasController(BdDhrContext context)
        {
            _context = context;
        }

        // GET: JefeBodegas
        public async Task<IActionResult> Index()
        {
            var bdDhrContext = _context.JefeBodegas.Include(j => j.ActaIdActaNavigation);
            return View(await bdDhrContext.ToListAsync());
        }

        // GET: JefeBodegas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JefeBodegas == null)
            {
                return NotFound();
            }

            var jefeBodega = await _context.JefeBodegas
                .Include(j => j.ActaIdActaNavigation)
                                .ThenInclude(a => a.EspecialidadIdEspecialidadNavigation)
                .Include(j => j.ActaIdActaNavigation.RazonSocialIdRazonSocialNavigation)
                .Include(j => j.ActaIdActaNavigation.ObraIdObraNavigation)
                .FirstOrDefaultAsync(m => m.IdJefeBodega == id);
            if (jefeBodega == null)
            {
                return NotFound();
            }

            return View(jefeBodega);
        }

        // GET: JefeBodegas/Create
        public IActionResult Create()
        {
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa");
            return View();
        }

        // POST: JefeBodegas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdJefeBodega,Nombre,DescuentoInsumo,EntregaCargos,Observacion,Firma,ActaIdActa")] JefeBodega jefeBodega)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jefeBodega);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", jefeBodega.ActaIdActa);
            return View(jefeBodega);
        }

        // GET: JefeBodegas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JefeBodegas == null)
            {
                return NotFound();
            }

            // Incluye las relaciones de navegación 
            var jefeBodega = await _context.JefeBodegas
                .Include(j => j.ActaIdActaNavigation)
                .ThenInclude(a => a.EspecialidadIdEspecialidadNavigation)
                .Include(j => j.ActaIdActaNavigation.RazonSocialIdRazonSocialNavigation)
                .Include(j => j.ActaIdActaNavigation.ObraIdObraNavigation)
                .FirstOrDefaultAsync(m => m.IdJefeBodega == id); // Suponiendo que el ID se llama IdJefeBodega

            if (jefeBodega == null)
            {
                return NotFound();
            }

            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", jefeBodega.ActaIdActa);
            return View(jefeBodega);
        }

        public IActionResult RedirectToListarActums()
        {
            return RedirectToAction("ListarActas", "Actums");
        }
        // POST: JefeBodegas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("IdJefeBodega,Nombre,DescuentoInsumo,EntregaCargos,Observacion,Firma,ActaIdActa")] JefeBodega jefeBodega)
        {
            if (id != jefeBodega.IdJefeBodega)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jefeBodega);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JefeBodegaExists(jefeBodega.IdJefeBodega))
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
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", jefeBodega.ActaIdActa);
            return View(jefeBodega);
        }

        // GET: JefeBodegas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JefeBodegas == null)
            {
                return NotFound();
            }

            var jefeBodega = await _context.JefeBodegas
                .Include(j => j.ActaIdActaNavigation)
                .FirstOrDefaultAsync(m => m.IdJefeBodega == id);
            if (jefeBodega == null)
            {
                return NotFound();
            }

            return View(jefeBodega);
        }

        // POST: JefeBodegas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JefeBodegas == null)
            {
                return Problem("Entity set 'BdDhrContext.JefeBodegas'  is null.");
            }
            var jefeBodega = await _context.JefeBodegas.FindAsync(id);
            if (jefeBodega != null)
            {
                _context.JefeBodegas.Remove(jefeBodega);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JefeBodegaExists(int id)
        {
          return (_context.JefeBodegas?.Any(e => e.IdJefeBodega == id)).GetValueOrDefault();
        }
    }
}
