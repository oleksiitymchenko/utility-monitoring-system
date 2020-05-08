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
    public class TelemetryRecordController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TelemetryRecordController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/ControllerRegistry
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TelemetryRecord>>> GetTelemetryRecord()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var list = await _context.TelemetryRecord
                    .Join(
                        _context.ControllerRegistry, 
                        x => x.ControllerRegistryId, 
                        y => y.Id, 
                        (r, j) => new { telrec = r, conreg = j })
                    .Where(tr => tr.conreg.ApplicationUserId == user.Id)
                    .Select(f => f.telrec).ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // PUT: api/ControllerRegistry/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTelemetryRecord(Guid id, TelemetryRecord telemetryRecord)
        {
            if (id != telemetryRecord.Id)
            {
                return BadRequest();
            }
            var first = await _context.TelemetryRecord.FirstOrDefaultAsync(x => x.Id == id);
            first.CounterValue = telemetryRecord.CounterValue;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/ControllerRegistry
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TelemetryRecord>> PostTelemetryRecord(TelemetryRecord telemetryRecord)
        {
            _context.TelemetryRecord.Add(telemetryRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTelemetryRecord", new { id = telemetryRecord.Id }, telemetryRecord);
        }

        // DELETE: api/ControllerRegistry/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TelemetryRecord>> DeleteTelemetryRecord(Guid id)
        {
            var telemetryRecord = await _context.TelemetryRecord.FindAsync(id);
            if (telemetryRecord == null)
            {
                return NotFound();
            }

            _context.TelemetryRecord.Remove(telemetryRecord);
            await _context.SaveChangesAsync();

            return telemetryRecord;
        }

        private bool TelemetryRecordExists(Guid id)
        {
            return _context.ControllerRegistry.Any(e => e.Id == id);
        }
    }
}
