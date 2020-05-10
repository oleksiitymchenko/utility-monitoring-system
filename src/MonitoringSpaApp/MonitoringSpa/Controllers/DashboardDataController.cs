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
using MonitoringSpa.ViewModels;
using System.Globalization;

namespace MonitoringSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class DashboardDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardDataController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/ControllerRegistry
        [HttpGet("{timeclause}")]
        public async Task<ActionResult<DashboardData>> GetDashboardData(string timeclause)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var query = _context.TelemetryRecord
                    .Join(
                        _context.ControllerRegistry,
                        x => x.ControllerRegistryId,
                        y => y.Id,
                        (r, j) => new { telrec = r, conreg = j })
                    .Where(tr => tr.conreg.ApplicationUserId == user.Id)
                    .Select(f => f.telrec)
                    .Include(f => f.ControllerRegistry)
                    .OrderBy(f => f.CreatedDate);
                List<TelemetryRecord> list;
                var now = DateTime.UtcNow;
                var title = "Data for last week";
                var chartLabels = new List<string>();
                switch (timeclause)
                {
                    case "lastmonth":
                        {
                            title = "Data for last month";
                            list = await query.Where(f => f.CreatedDate <= now && f.CreatedDate >= now.AddMonths(-1)).ToListAsync();
                            var lol = list.GroupBy(x => x.CreatedDate.Date)
                                .Select(x => new 
                                { 
                                    CounterValue = x.Average(s => s.CounterValue),
                                    CreatedDate = x.Key,
                                })
                                .ToList();
                            chartLabels = lol.Select(f => f.CreatedDate.ToString("dd/MM", CultureInfo.CreateSpecificCulture("en-US"))).ToList();
                            return new DashboardData { ChartData = lol.Select(f => f.CounterValue.ToString()).ToList(), ChartTitle = title, ChartLabels = chartLabels };
                        }
                    case "lastyear":
                        {
                            title = "Data for last year";
                            list = await query.Where(f => f.CreatedDate <= now && f.CreatedDate >= now.AddYears(-1)).ToListAsync();
                            var lol = list.GroupBy(x => x.CreatedDate.Month)
                                .Select(x => new
                                {
                                    CounterValue = x.Average(s => s.CounterValue),
                                    CreatedDate = x.First().CreatedDate,
                                })
                                .ToList();

                            chartLabels = lol.Select(f => f.CreatedDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-US"))).ToList();
                            return new DashboardData { ChartData = lol.Select(f => f.CounterValue.ToString()).ToList(), ChartTitle = title, ChartLabels = chartLabels };
                        }
                    default:
                        {
                            list = await query.Where(f => f.CreatedDate <= now && f.CreatedDate >= now.AddDays(-7)).ToListAsync();
                            chartLabels = list.Select(f => f.CreatedDate.ToString("ddd d MMM", CultureInfo.CreateSpecificCulture("en-US"))).ToList();
                            break;
                        }
                }

                return new DashboardData { ChartData = list.Select(f => f.CounterValue.ToString()).ToList(), ChartTitle = title, ChartLabels = chartLabels };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
