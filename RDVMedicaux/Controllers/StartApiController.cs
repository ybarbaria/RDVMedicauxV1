using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using RDVMedicaux.ViewModels.Core;

namespace RDVMedicaux.Controllers
{
    public class StartApiController : ApiController
    {
        /// <summary>
        /// Permet de retourner un message JSON contenant la configuration de l'application
        /// Cette configuration peut être stocker localement dans le navigateur (sessionStorage)
        /// </summary>
        /// <returns>Message json contenant le paramétrage</returns>
        [HttpPost]
        public MessageCoreVm Index()
        {
            // objet config
            var config = new JObject() as dynamic;

            // baseUrl : stocke le path absolu
            config.baseUrl = Url.Content("~/");

            // 3 - Nombre de résultats maximal
            config.nbMaxRow = ConfigurationManager.AppSettings.Get("sql.maxcount");

            return MessageCoreVm.SendSucces<JObject>(config);
        }
    }
}
