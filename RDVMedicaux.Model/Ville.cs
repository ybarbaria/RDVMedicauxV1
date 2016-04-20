using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDVMedicaux.Model
{
    public class Ville
    {
        /// <summary>
        /// Obtient ou définit l'id de la ville
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Obtient ou définit le code postal de la ville
        /// </summary>
        public string CodePostal { get; set; }

        /// <summary>
        /// Obtient ou définit le nom de la ville
        /// </summary>s
        public string NomVille { get; set; }

        /// <summary>
        /// Obtient ou définit la latitude de la position GPS de la ville 
        /// </summary>
        public decimal LocX { get; set; }

        /// <summary>
        /// Obtient ou définit la longitude de la position GPS de la ville
        /// </summary>
        public decimal LocY { get; set; }
    }
}
