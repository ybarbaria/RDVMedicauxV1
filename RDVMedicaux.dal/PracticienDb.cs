using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDVMedicaux.Dal.Base;

namespace RDVMedicaux.Dal
{
    public class PracticienDb : AbstractDb
    {
        #region Contructor
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PracticienDb" />.
        /// </summary>
        /// <param name="conn">Connexion courante</param>
        public PracticienDb(DbConnection conn)
            : base(conn)
        {
        }
        #endregion
    }
}
