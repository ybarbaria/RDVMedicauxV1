using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDVMedicaux.Dal.Base;

namespace RDVMedicaux.Dal
{
    public class VilleDb : AbstractDb
    {
        #region Contructor
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="VilleDb" />.
        /// </summary>
        /// <param name="conn">Connexion courante</param>
        public VilleDb(DbConnection conn)
            : base(conn)
        {
        }
        #endregion
    }
}
