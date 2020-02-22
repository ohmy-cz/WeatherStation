using Microsoft.AspNetCore.Mvc;
using net.jancerveny.weatherstation.BusinessLayer;
using System;

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

        public IActionResult GetReadings()
        {
            var result = _dr.GetReadings();
            return Ok(result);
        }

        public IActionResult GetDataSources()
        {
            var result = _ds.GetAll();
            return Ok(result);
        }
    }
}
