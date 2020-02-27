using Microsoft.AspNetCore.Mvc;
using net.jancerveny.weatherstation.BusinessLayer;
using net.jancerveny.weatherstation.Common.Interfaces;
using net.jancerveny.weatherstation.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.jancerveny.weatherstation.Client.Web.Controllers
{
    public class DataController : Controller
    {
        private readonly DataReadingsService _dr;
        private readonly DataSourcesService _ds;

        public DataController(DataReadingsService dr, DataSourcesService ds)
        {
            if (dr == null) throw new ArgumentNullException(nameof(dr));
            if (ds == null) throw new ArgumentNullException(nameof(ds));
            _dr = dr;
            _ds = ds;
        }

        public async Task<IActionResult> GetReadingsAsync(DateTime? since = null)
        {
            var result = await  _dr.GetReadingsAsync(since);
            return Ok(result);
        }

        public async Task<IActionResult> GetDataSources()
        {
            var result = await _ds.GetAllAsync();
            return Ok(result);
        }

        public async Task<IActionResult> GetAggregations()
        {
            var result = await _dr.GetAggregationsAsync(ChartConfiguration.Last7Days.Labels[0].Start);
            return Ok(result);
        }

        public IActionResult GetChartsConfig()
        {
            return Ok(new List<IChartConfiguration> { 
                ChartConfiguration.RealTime,
                ChartConfiguration.Last7Days
            });
        }
    }
}
