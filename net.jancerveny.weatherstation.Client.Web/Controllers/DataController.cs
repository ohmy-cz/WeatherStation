using Microsoft.AspNetCore.Mvc;
using net.jancerveny.weatherstation.BusinessLayer;
using System;

namespace net.jancerveny.weatherstation.Client.Web.Controllers
{
    public class DataController : Controller
    {
        private readonly DataReadings _dr;
        private readonly DataSources _ds;

        public DataController(DataReadings dr, DataSources ds)
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
