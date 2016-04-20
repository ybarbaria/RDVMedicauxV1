using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDVMedicaux.Model
{
    public class Practicien
    {
        /// <summary>
        /// Constante définissant le nombre de KM max 
        /// </summary>
        private const int KM = 15;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Practicien"/>
        /// </summary>
        public Practicien()
        {
        }

        #region Criteria

        /// <summary>
        /// Critère de filtre
        /// </summary>
        public enum Criteria : int
        {
            /// <summary>
            /// Lieu de recherche
            /// </summary>
            Lieu,

            /// <summary>
            /// Specialité du medecin
            /// </summary>
            Specialite,

            /// <summary>
            /// Tri par Nom
            /// </summary>
            SortByName,

            /// <summary>
            /// Tri par secteur 1
            /// </summary>
            SortBySecteur1,

            /// <summary>
            /// Position GPS Latitude
            /// </summary>
            LocLat, 

            /// <summary>
            /// Position GPS Longitude
            /// </summary>
            LocLong
        }
        #endregion

        /// <summary>
        /// Obtient ou définit l'id du practicien
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Obtient ou définit le nom
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        /// Obtient ou définit le prénom
        /// </summary>s
        public string Prenom { get; set; }

        /// <summary>
        /// Obtient ou définit l'adresse du practicien
        /// </summary>
        public string Adresse { get; set; }

        /// <summary>
        /// Obtient ou définit la ville du practicien
        /// </summary>
        public string Ville { get; set; }

        /// <summary>
        /// Obtient ou définit le code postal du practicien
        /// </summary>
        public int CP { get; set; }

        /// <summary>
        /// Obtient ou définit la specialité
        /// </summary>
        public string Specialite1 { get; set; }

        /// <summary>
        /// Obtient ou définit le secteur tarifaire
        /// </summary>
        public string SecteurTarifaire { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le practicien accepte la carte vitale
        /// </summary>
        public bool AcceptCarteVitale { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public double Distance { get; set; }
    }
}
