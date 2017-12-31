using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataSystem;
using WebSystem.Code;

namespace WebSystem.Controllers
{
    public class QRController : Controller
    {
        // GET: QR
        public ActionResult Index()
        {

            return View(DataList.Current);
        }

        public ActionResult Get(string Name)
        {
            if (DataList.Current[Name] == null) Name = DataList.Current[0].Name;
            DataList.Current[Name].Students.ForEach(p => p.StudentToQR());
            return View("Get",DataList.Current[Name]);
        }
    }
}