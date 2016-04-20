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
        /// Obtient ou définit l'id du practicien
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Obtient ou définit 
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        /// Obtient ou définit 
        /// </summary>s
        public string Prenom { get; set; }

        /// <summary>
        /// Obtient ou définit 
        /// </summary>
        public string Adresse { get; set; }

        /// <summary>
        /// Obtient ou définit 
        /// </summary>
        public string CP { get; set; }

        /// <summary>
        /// Obtient ou définit 
        /// </summary>
        public string Ville { get; set; }

        /// <summary>
        /// Obtient ou définit
        /// </summary>
        public string Specialite1 { get; set; }
    }
}
