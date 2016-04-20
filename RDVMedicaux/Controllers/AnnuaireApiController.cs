using RDVMedicaux.BLL;
using RDVMedicaux.Model;
using RDVMedicaux.ViewModels;
using RDVMedicaux.ViewModels.Core;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace RDVMedicaux.Controllers
{
    public class AnnuaireApiController : ApiController
    {
        public MessageCoreVm Filter(AnnuaireFilterVm msgVm)
        {
            // Lorsque un utilisateur clique sur l'onglet mes activité ou lorsqu'il accède à cette page via l'url la view est vide

            List<Practicien> listPracticien = new List<Practicien>();

            PracticienBll practicienBll = new PracticienBll();

            // Lecture des critères de filtre depuis la vm
            Dictionary<Practicien.Criteria, object> dicoFilter = msgVm.GetDicoFilter(AnnuaireFilterVm.MapCriteriaVm);

            // Chargement des activités 
            listPracticien = practicienBll.Filter(dicoFilter);

            // Transformation en Vm
            List<ListPracticiensVillesVm> listVm = listPracticien.Select((a) => ListPracticiensVillesVm.Load(a)).ToList();

            // Message json
            return MessageCoreVm.SendSucces<List<ListPracticiensVillesVm>>(listVm);
        }
    }
}
