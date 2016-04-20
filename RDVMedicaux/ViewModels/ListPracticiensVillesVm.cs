using Newtonsoft.Json;
using RDVMedicaux.Model;
using RDVMedicaux.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDVMedicaux.ViewModels
{
    public class ListPracticiensVillesVm : BaseVm
    {

        /// <summary>
        /// Obtient ou définit 
        /// </summary>
        [JsonProperty("idPracticien")]
        public int IdPracticien { get; set; }

        /// <summary>
        /// Obtient ou définit le nom
        /// </summary>
        [JsonProperty("nom")]
        public string Nom { get; set; }

        /// <summary>
        /// Obtient ou définit le prénom
        /// </summary>
        [JsonProperty("prenom")]
        public string Prenom { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("cp")]
        public int CpVille { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("nomVille")]
        public string NomVille { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("adresse")]
        public string Adresse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("distance")]
        public double Distance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("sectTarifaire")]
        public string SectTarifaire { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("specialite")]
        public string Specialite { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("acceptCarteVitale")]
        public bool AcceptCarteVitale { get; set; }

        public static ListPracticiensVillesVm Load(Practicien practicien)
        {
            ListPracticiensVillesVm listVm = new ListPracticiensVillesVm();

            listVm.IdPracticien = practicien.Id.Value;
            listVm.Nom = practicien.Nom;
            listVm.Prenom = practicien.Prenom;
            listVm.NomVille = practicien.Ville;
            listVm.SectTarifaire = practicien.SecteurTarifaire;

            if (practicien.Specialite1.Equals("10"))
            {
                listVm.Specialite = "Dermatologue";
            }
            else if (practicien.Specialite1.Equals("21"))
            {
                listVm.Specialite = "Ophtalmologue";
            }
            else
            {
                listVm.Specialite = "ORL";
            }

            listVm.AcceptCarteVitale = practicien.AcceptCarteVitale;
            listVm.Distance = practicien.Distance;
            listVm.Adresse = practicien.Adresse;
            listVm.CpVille = practicien.CP;

            return listVm;
        }

        public static Practicien Get(ListPracticiensVillesVm listVm)
        {
            Practicien practicien = new Practicien();
            // TODO
            return practicien;
        }

    }
}