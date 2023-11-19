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
    public class OficinaTecnicasController : Controller
    {
        private readonly BdDhrContext _context;

        public OficinaTecnicasController(BdDhrContext context)
        {
            _context = context;
        }

        // GET: OficinaTecnicas
        public async Task<IActionResult> Index()
        {
            var bdDhrContext = _context.OficinaTecnicas.Include(o => o.ActaIdActaNavigation);
            return View(await bdDhrContext.ToListAsync());
        }

        // GET: OficinaTecnicas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OficinaTecnicas == null)
            {
                return NotFound();
            }

            var oficinaTecnica = await _context.OficinaTecnicas
                .Include(o => o.ActaIdActaNavigation)
                                .ThenInclude(a => a.EspecialidadIdEspecialidadNavigation)
                .Include(j => j.ActaIdActaNavigation.RazonSocialIdRazonSocialNavigation)
                .Include(j => j.ActaIdActaNavigation.ObraIdObraNavigation)
                .FirstOrDefaultAsync(m => m.IdOfitec == id);
            if (oficinaTecnica == null)
            {
                return NotFound();
            }

            return View(oficinaTecnica);
        }

        // GET: OficinaTecnicas/Create
        public IActionResult Create()
        {
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa");
            return View();
        }

        // POST: OficinaTecnicas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdOfitec,Nombre,EntrEstPag,BoleGar,ConFirmEmpr,Descuento,DetaMotivo,Observacion,Firma,ActaIdActa")] OficinaTecnica oficinaTecnica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(oficinaTecnica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", oficinaTecnica.ActaIdActa);
            return View(oficinaTecnica);
        }

        // GET: OficinaTecnicas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OficinaTecnicas == null)
            {
                return NotFound();
            }

            // Incluye las relaciones de navegación 
            var oficinaTecnica = await _context.OficinaTecnicas
                .Include(o => o.ActaIdActaNavigation)
                .ThenInclude(a => a.EspecialidadIdEspecialidadNavigation)
                .Include(o => o.ActaIdActaNavigation.RazonSocialIdRazonSocialNavigation)
                .Include(o => o.ActaIdActaNavigation.ObraIdObraNavigation)
                .FirstOrDefaultAsync(m => m.IdOfitec == id); // Suponiendo que el ID se llama IdOficinaTecnica

            if (oficinaTecnica == null)
            {
                return NotFound();
            }

            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", oficinaTecnica.ActaIdActa);
            return View(oficinaTecnica);
        }

        public IActionResult RedirectToListarActums()
        {
            return RedirectToAction("ListarActas", "Actums");
        }
        // POST: OficinaTecnicas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("IdOfitec,Nombre,EntrEstPag,BoleGar,ConFirmEmpr,Descuento,DetaMotivo,Observacion,Firma,ActaIdActa")] OficinaTecnica oficinaTecnica)
        {
            if (id != oficinaTecnica.IdOfitec)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(oficinaTecnica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OficinaTecnicaExists(oficinaTecnica.IdOfitec))
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
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", oficinaTecnica.ActaIdActa);
            return View(oficinaTecnica);
        }

        // GET: OficinaTecnicas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OficinaTecnicas == null)
            {
                return NotFound();
            }

            var oficinaTecnica = await _context.OficinaTecnicas
                .Include(o => o.ActaIdActaNavigation)
                .FirstOrDefaultAsync(m => m.IdOfitec == id);
            if (oficinaTecnica == null)
            {
                return NotFound();
            }

            return View(oficinaTecnica);
        }

        // POST: OficinaTecnicas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OficinaTecnicas == null)
            {
                return Problem("Entity set 'BdDhrContext.OficinaTecnicas'  is null.");
            }
            var oficinaTecnica = await _context.OficinaTecnicas.FindAsync(id);
            if (oficinaTecnica != null)
            {
                _context.OficinaTecnicas.Remove(oficinaTecnica);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OficinaTecnicaExists(int id)
        {
          return (_context.OficinaTecnicas?.Any(e => e.IdOfitec == id)).GetValueOrDefault();
        }
    }
}
