using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using RDVMedicaux.AppException;

namespace RDVMedicaux.ViewModels.Core
{
    /// <summary>
    /// Vue Modèle pour transmettre le message d'erreur fonctionnelle
    /// </summary>
    public class ErrorVm
    {
        #region Properties
        /// <summary>
        /// Obtient ou définit le message pour l'utilisateur
        /// </summary>
        [JsonProperty(PropertyName = "friendlyMessage")]
        public string FriendlyMessage { get; set; }

        /// <summary>
        /// Obtient ou définit le message d'origine
        /// </summary>
        [JsonProperty(PropertyName = "originalMessage")]
        public string OriginalMessage { get; set; }

        /// <summary>
        /// Obtient ou définit la pile des appels
        /// </summary>
        [JsonProperty(PropertyName = "stackTrace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// Obtient ou définit le code erreur
        /// </summary>
        [JsonProperty(PropertyName = "errorCode")]
        public int ErrorCode { get; set; }
        #endregion

        #region LoadError
        /// <summary>
        /// Charge un exception vers la vue modèle
        /// </summary>
        /// <param name="ex">Une exception custom ou non</param>
        /// <returns>La vue modèle chargée</returns>
        public static ErrorVm LoadException(Exception ex)
        {
            // default
            ErrorVm errorVm = new ErrorVm();
            errorVm.ErrorCode = (int)CustomExceptionErrorCode.GenericServer;
            //// errorVm.FriendlyMessage = Properties.Resources.error_default;
            errorVm.OriginalMessage = ex.Message;
            errorVm.StackTrace = ex.StackTrace;

            // customise le message pour custom exception
            CustomException custException = ex as CustomException;

            // gestion CustomException ou Exception
            if (custException != null)
            {
                // conserve le code erreur
                errorVm.ErrorCode = (int)custException.ErrorCode;

                switch (custException.ErrorCode)
                {
                    case CustomExceptionErrorCode.GenericServer:
                        errorVm.FriendlyMessage = "Une erreur applicative est survenue sur le serveur. Contactez votre administrateur.";
                        break;

                    case CustomExceptionErrorCode.ConcurrentAccess:
                        errorVm.FriendlyMessage = "Erreur d'accès concurrent. Vous ne pouvez pas sauvegarder car un autre utilisateur a modifié ces données.";
                        break;

                    case CustomExceptionErrorCode.UniqueKeyConstraint:
                        errorVm.FriendlyMessage = "Erreur en base de données (contrainte d'unicité). Contactez votre administrateur.";
                        break;

                    case CustomExceptionErrorCode.DeleteForeignKey:
                        errorVm.FriendlyMessage = "Erreur en base de données (suppression Foreign Key). Contactez votre administrateur.";
                        break;

                    case CustomExceptionErrorCode.SessionTimeOut:
                        errorVm.FriendlyMessage = "Erreur timeout de session a expiré.";
                        break;

                    case CustomExceptionErrorCode.UnKnownUser:
                        errorVm.FriendlyMessage = "Utilisateur inconnu. Vérifiez que l'utilisateur possède un profil.";
                        break;

                    case CustomExceptionErrorCode.ValidationFailed:
                        errorVm.FriendlyMessage = "Une erreur applicative est survenue sur le serveur. Contactez votre administrateur.";
                        break;

                    case CustomExceptionErrorCode.AccessDenied:
                        errorVm.FriendlyMessage = "Vous n'avez pas les droits d'accès suffisants. Contactez votre administrateur.";
                        break;

                    // on ne transmet pas de message d'erreur pour le modelstatefailed, les messages sont géré sur chaque champs de la vue
                    // case CustomExceptionErrorCode.ModelStateFailed:
                }
            }

            return errorVm;
        }
        #endregion
    }
}