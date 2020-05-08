using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.AccessTokenValidation;

namespace MonitoringSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ControllerRegistryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ControllerRegistryController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/ControllerRegistry
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ControllerRegistry>>> GetControllerRegistry()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var list = await _context.ControllerRegistry.Where(x => x.ApplicationUserId == user.Id).ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // GET: api/ControllerRegistry/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ControllerRegistry>> GetControllerRegistry(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            var controllerRegistry = await _context.ControllerRegistry.FirstOrDefaultAsync(x => x.Id == id && user.Id == x.ApplicationUserId);

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
            var first = await _context.ControllerRegistry.FirstOrDefaultAsync(x => x.Id == id);
            first.Name = controllerRegistry.Name;
            first.Description = controllerRegistry.Description;
            await _context.SaveChangesAsync();
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
