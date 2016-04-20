using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

using RDVMedicaux.ViewModels.Core;

namespace RDVMedicaux.Filter
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// Si une exception est lancée depuis un controller
        /// </summary>
        /// <param name="filterContext">Contexte du filtre</param>
        public override void OnException(HttpActionExecutedContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext est null");
            }

            // création du message
            ErrorVm errorVm = ErrorVm.LoadException(filterContext.Exception);

            // init du message succes=false
            MessageCoreVm msg = new MessageCoreVm()
            {
                ErrorObject = errorVm,
                Success = false
            };

            // création de la réponse
            filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.OK, msg);
        }
    }
}