using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RDVMedicaux.Dal.Base
{
    /// <summary>
    /// Super classe des accès aux données
    /// </summary>
    public class AbstractDb
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AbstractDb" />.
        /// Constructeur par défaut
        /// </summary>
        /// <param name="connection">Connexion à la base</param>
        public AbstractDb(DbConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// Obtient ou définit la connexion à la base
        /// </summary>
        protected DbConnection Connection { get; set; }
    }
}