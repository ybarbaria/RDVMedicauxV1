using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDVMedicaux.Controllers
{
    public class AnnuaireController : Controller
    {
        /// <summary>
        /// GET: Annuaire
        /// </summary>
        /// <returns>Vue View/Annuaire/Index.cshtml</returns>
        public ActionResult Index()
        {
            return View();
        }


    }
}