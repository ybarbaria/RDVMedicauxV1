using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using RDVMedicaux.ViewModels.Core;
using RDVMedicaux.Model;

namespace RDVMedicaux.ViewModels
{
    /// <summary>
    /// Vue modèle du filtre pour les activités
    /// </summary>
    public class AnnuaireFilterVm : BaseVm
    {
        static AnnuaireFilterVm()
        {
            MapCriteriaVm = new Dictionary<Practicien.Criteria, Func<AnnuaireFilterVm, object>>()
            {
                { Practicien.Criteria.Lieu, (vm) => vm.Lieu },
                { Practicien.Criteria.SortByName, (vm) => vm.SortByName },
                { Practicien.Criteria.SortBySecteur1, (vm) => vm.SortBySecteur1 },
                { Practicien.Criteria.Specialite, (vm) => vm.Specialite },
                 { Practicien.Criteria.LocLat, (vm) => vm.LocLat },
                 { Practicien.Criteria.LocLong, (vm) => vm.LocLong }
            };
        }

        /// <summary>
        /// Obtient le dictionnaire des critères de recherche
        /// </summary>
        public static Dictionary<Practicien.Criteria, Func<AnnuaireFilterVm, object>> MapCriteriaVm { get; private set; }

        /// <summary>
        /// Obtient ou définit la ville saisie
        /// </summary>
        [JsonProperty("lieu")]
        public string Lieu { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si on applique un tri par nom
        /// </summary>
        [JsonProperty("sortByName")]
        public bool? SortByName { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si on applique un tri par secteur 1s
        /// </summary>
        [JsonProperty("sortBySector1")]
        public bool? SortBySecteur1 { get; set; }

        /// <summary>
        /// Obtient ou définit la specialité selectionnée
        /// </summary>
        [JsonProperty("selectedSpec")]
        public string Specialite { get; set; }

        /// <summary>
        /// Obtient ou définit la latitude de la position gps de la ville
        /// </summary>
        [JsonProperty("locLat")]
        public double? LocLat { get; set; }

        /// <summary>
        /// Obtient ou définit la longitude de la position gps de la ville
        /// </summary>
        [JsonProperty("locLong")]
        public double? LocLong { get; set; }
    }
}