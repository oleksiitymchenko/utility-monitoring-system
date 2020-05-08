using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL;
using DAL.Models;

namespace MonitoringSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerRegistryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ControllerRegistryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ControllerRegistry
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ControllerRegistry>>> GetControllerRegistry()
        {
            return await _context.ControllerRegistry.ToListAsync();
        }

        // GET: api/ControllerRegistry/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ControllerRegistry>> GetControllerRegistry(Guid id)
        {
            var controllerRegistry = await _context.ControllerRegistry.FindAsync(id);

            if (controllerRegistry == null)
            {
                return NotFound();
            }

            return controllerRegistry;
        }

        // PUT: api/ControllerRegistry/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutControllerRegistry(Guid id, ControllerRegistry controllerRegistry)
        {
            if (id != controllerRegistry.Id)
            {
                return BadRequest();
            }

            _context.Entry(controllerRegistry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ControllerRegistryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ControllerRegistry
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ControllerRegistry>> PostControllerRegistry(ControllerRegistry controllerRegistry)
        {
            _context.ControllerRegistry.Add(controllerRegistry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetControllerRegistry", new { id = controllerRegistry.Id }, controllerRegistry);
        }

        // DELETE: api/ControllerRegistry/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ControllerRegistry>> DeleteControllerRegistry(Guid id)
        {
            var controllerRegistry = await _context.ControllerRegistry.FindAsync(id);
            if (controllerRegistry == null)
            {
                return NotFound();
            }

            _context.ControllerRegistry.Remove(controllerRegistry);
            await _context.SaveChangesAsync();

            return controllerRegistry;
        }

        private bool ControllerRegistryExists(Guid id)
        {
            return _context.ControllerRegistry.Any(e => e.Id == id);
        }
    }
}
