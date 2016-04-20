using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RDVMedicaux.AppException
{
    /// <summary>
    /// Enumération des cas d'exception
    /// </summary>
    public enum CustomExceptionErrorCode : int
    {
        /// <summary>
        /// Générique erreur (défaut)...
        /// </summary>
        GenericServer = 0,

        /// <summary>
        /// Accès concurrent en base
        /// </summary>
        ConcurrentAccess = 1,

        /// <summary>
        /// Contrainte d'unicité
        /// </summary>
        UniqueKeyConstraint = 2,

        /// <summary>
        /// Contrainte de clé étrangère
        /// </summary>
        DeleteForeignKey = 3,

        /// <summary>
        /// Session timeout
        /// </summary>
        SessionTimeOut = 4,

        /// <summary>
        /// Authentification de l'utilisateur en échec
        /// </summary>
        UnKnownUser = 5,

        /// <summary>
        /// Echec de la validation du message
        /// </summary>
        ValidationFailed = 6,

        /// <summary>
        /// Validation modelstate en echec
        /// </summary>
        ModelStateFailed = 7,

        /// <summary>
        /// Echec dans les droits d'accès
        /// </summary>
        AccessDenied = 8,

        /// <summary>
        /// Aucune données trouvée dans la base
        /// </summary>
        NoDataFound = 9,
    }

    /// <summary>
    /// Applicative Exception of the projet
    /// </summary>
    [Serializable]
    public class CustomException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CustomException" />.
        /// </summary>
        public CustomException()
            : base()
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CustomException" />.
        /// </summary>
        /// <param name="message">Message d'erreur de l'exception</param>
        public CustomException(string message)
            : base(message)
        {
            this.ErrorCode = CustomExceptionErrorCode.GenericServer;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CustomException" />.
        /// </summary>
        /// <param name="message">Message d'erreur de l'exception</param>
        /// <param name="rootEx">Exception racine</param>
        public CustomException(string message, System.Exception rootEx)
            : base(message, rootEx)
        {
            this.ErrorCode = CustomExceptionErrorCode.GenericServer;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CustomException" />.
        /// </summary>
        /// <param name="message">Message d'erreur de l'exception</param>
        /// <param name="rootEx">Exception racine</param>
        /// <param name="custEnum">Code erreur de l'exception sous forme d'énumération</param>
        public CustomException(string message, System.Exception rootEx, CustomExceptionErrorCode custEnum)
            : base(message, rootEx)
        {
            this.ErrorCode = custEnum;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CustomException" />.
        /// </summary>
        /// <param name="message">Message d'erreur de l'exception</param>
        /// <param name="custEnum">Code erreur de l'exception sous forme d'énumération</param>
        public CustomException(string message, CustomExceptionErrorCode custEnum)
            : base(message)
        {
            this.ErrorCode = custEnum;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CustomException" />.
        /// </summary>
        /// <param name="custEnum">Code erreur de l'exception sous forme d'énumération</param>
        public CustomException(CustomExceptionErrorCode custEnum)
            : base()
        {
            this.ErrorCode = custEnum;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Obtient le code erreur de l'exception
        /// </summary>
        public CustomExceptionErrorCode ErrorCode { get; private set; }

        #endregion Properties

        #region ISerializable

        /// <summary>
        /// Implémentation de <see cref="ISerializable"/> pour alerte code analysis
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> à remplir de données.</param>
        /// <param name="context">Destination de cette sérialisation.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ErrorCode", this.ErrorCode);
        }

        #endregion
    }
}
