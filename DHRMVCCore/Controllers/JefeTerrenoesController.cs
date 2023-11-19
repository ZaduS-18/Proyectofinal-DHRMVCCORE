﻿using System;
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
    public class JefeTerrenoesController : Controller
    {
        private readonly BdDhrContext _context;

        public JefeTerrenoesController(BdDhrContext context)
        {
            _context = context;
        }

        // GET: JefeTerrenoes
        public async Task<IActionResult> Index()
        {
            var bdDhrContext = _context.JefeTerrenos.Include(j => j.ActaIdActaNavigation);
            return View(await bdDhrContext.ToListAsync());
        }

        // GET: JefeTerrenoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JefeTerrenos == null)
            {
                return NotFound();
            }

            var jefeTerreno = await _context.JefeTerrenos
                .Include(j => j.ActaIdActaNavigation)
                                .ThenInclude(a => a.EspecialidadIdEspecialidadNavigation)
                .Include(j => j.ActaIdActaNavigation.RazonSocialIdRazonSocialNavigation)
                .Include(j => j.ActaIdActaNavigation.ObraIdObraNavigation)
                .FirstOrDefaultAsync(m => m.IdJefeterreno == id);
            if (jefeTerreno == null)
            {
                return NotFound();
            }

            return View(jefeTerreno);
        }

        // GET: JefeTerrenoes/Create
        public IActionResult Create()
        {
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa");
            return View();
        }

        // POST: JefeTerrenoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdJefeterreno,Nombre,CumPlaSem,AsiReuPla,SupPerObra,CumProgGen,Observacion,Firma,ActaIdActa")] JefeTerreno jefeTerreno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jefeTerreno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", jefeTerreno.ActaIdActa);
            return View(jefeTerreno);
        }

        // GET: JefeTerrenoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JefeTerrenos == null)
            {
                return NotFound();
            }

            // Incluye la relación de navegación ActaIdActaNavigation
            var jefeTerreno = await _context.JefeTerrenos
                .Include(j => j.ActaIdActaNavigation)
                .ThenInclude(a => a.EspecialidadIdEspecialidadNavigation)
                .Include(j => j.ActaIdActaNavigation.RazonSocialIdRazonSocialNavigation)
                .Include(j => j.ActaIdActaNavigation.ObraIdObraNavigation)
                .FirstOrDefaultAsync(m => m.IdJefeterreno == id);


            if (jefeTerreno == null)
            {
                return NotFound();
            }

            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", jefeTerreno.ActaIdActa);
            return View(jefeTerreno);
        }
        public IActionResult RedirectToListarActums()
        {
            return RedirectToAction("ListarActas", "Actums");
        }

        // POST: JefeTerrenoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdJefeterreno,Nombre,CumPlaSem,AsiReuPla,SupPerObra,CumProgGen,Observacion,Firma,ActaIdActa")] JefeTerreno jefeTerreno)
        {
            if (id != jefeTerreno.IdJefeterreno)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jefeTerreno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JefeTerrenoExists(jefeTerreno.IdJefeterreno))
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
            ViewData["ActaIdActa"] = new SelectList(_context.Acta, "IdActa", "IdActa", jefeTerreno.ActaIdActa);
            return View(jefeTerreno);
        }

        // GET: JefeTerrenoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JefeTerrenos == null)
            {
                return NotFound();
            }

            var jefeTerreno = await _context.JefeTerrenos
                .Include(j => j.ActaIdActaNavigation)
                .FirstOrDefaultAsync(m => m.IdJefeterreno == id);
            if (jefeTerreno == null)
            {
                return NotFound();
            }

            return View(jefeTerreno);
        }

        // POST: JefeTerrenoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JefeTerrenos == null)
            {
                return Problem("Entity set 'BdDhrContext.JefeTerrenos'  is null.");
            }
            var jefeTerreno = await _context.JefeTerrenos.FindAsync(id);
            if (jefeTerreno != null)
            {
                _context.JefeTerrenos.Remove(jefeTerreno);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JefeTerrenoExists(int id)
        {
          return (_context.JefeTerrenos?.Any(e => e.IdJefeterreno == id)).GetValueOrDefault();
        }
    }
}
