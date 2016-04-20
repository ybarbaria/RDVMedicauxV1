using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using RDVMedicaux.AppException;
using RDVMedicaux.ViewModels.Core;

namespace RDVMedicaux.ViewModels.Core
{
    /// <summary>
    /// Message de base pour tous les appels json
    /// </summary>
    public class MessageCoreVm
    {
        #region Properties
        /// <summary>
        /// Obtient ou définit les données à transférer
        /// </summary>
        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }

        /// <summary>
        /// Obtient ou définit le corps html d'une vue
        /// </summary>
        [JsonProperty(PropertyName = "view", NullValueHandling = NullValueHandling.Ignore)]
        public string View { get; set; }

        /// <summary>
        /// Obtient ou définit le nombre total de pages
        /// </summary>
        [JsonProperty(PropertyName = "total", NullValueHandling = NullValueHandling.Ignore)]
        public int? Total { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le traitement a réussi ou échoué
        /// </summary>
        [JsonProperty(PropertyName = "success", NullValueHandling = NullValueHandling.Ignore)]
        public bool Success { get; set; }

        /// <summary>
        /// Obtient ou définit le détail de l'erreur
        /// </summary>
        [JsonProperty(PropertyName = "errorObject", NullValueHandling = NullValueHandling.Ignore)]
        public ErrorVm ErrorObject { get; set; }

        /// <summary>
        /// Obtient ou définit une map contenant les erreurs de validations
        /// </summary>
        [JsonProperty(PropertyName = "errorValidation", NullValueHandling = NullValueHandling.Ignore)]
        public object ErrorValidation { get; set; }

        #endregion

        #region Public methods
        /// <summary>
        /// Envoyer les données dans un message de succès
        /// </summary>
        /// <returns>Vue modèle message</returns>
        public static MessageCoreVm SendSucces()
        {
            return new MessageCoreVm() { Success = true };
        }

        /// <summary>
        /// Envoyer les données dans un message de succès
        /// </summary>
        /// <typeparam name="T">Type de la vue modèle</typeparam>
        /// <param name="data">Vue modèle à transmettre</param>
        /// <returns>Vue modèle message</returns>
        public static MessageCoreVm SendSucces<T>(T data)
        {
            return new MessageCoreVm() { Success = true, Data = data };
        }

        /// <summary>
        /// Envoyer les données d'une vue partielle dans un message de succès
        /// </summary>
        /// <param name="view">Chaine HTML de la vue partielle</param>
        /// <returns>Vue modèle message</returns>
        public static MessageCoreVm SendSucces(string view)
        {
            return new MessageCoreVm() { Success = true, View = view };
        }


        /// <summary>
        /// Envoyer des données et les données d'une vue partielle dans un message de succès
        /// </summary>
        /// <typeparam name="T">Type de la vue modèle</typeparam>
        /// <param name="data">Vue modèle à transmettre</param>
        /// <param name="view">Chaine HTML de la vue partielle</param>
        /// <returns>Vue modèle message</returns>
        public static MessageCoreVm SendSucces<T>(T data, string view)
        {
            return new MessageCoreVm() { Success = true, Data = data, View = view };
        }

        /// <summary>
        /// Envoyer un message d'échec de la validation de la vue modèle après le ModelState
        /// </summary>
        /// <param name="modelState">Dictionnaire état de la validation</param>
        /// <returns>Vue modèle message en erreur avec informations de validation à afficher au client</returns>
        public static MessageCoreVm SendModelStateFailed(System.Web.Http.ModelBinding.ModelStateDictionary modelState)
        {
            return new MessageCoreVm()
            {
                Success = false,
                ErrorObject = new ErrorVm() { ErrorCode = (int)CustomExceptionErrorCode.ModelStateFailed },
                ErrorValidation = ProcessModelState(modelState)
            };
        }

        /// <summary>
        /// Envoyer un message d'échec de validation custom pour afficher une notification
        /// </summary>
        /// <param name="message">Message de validation à transmettre</param>
        /// <returns>Vue modèle message en erreur</returns>
        public static MessageCoreVm SendModelStateFailed(string message)
        {
            return new MessageCoreVm()
            {
                Success = false,
                ErrorObject = new ErrorVm() { ErrorCode = (int)CustomExceptionErrorCode.ValidationFailed, FriendlyMessage = message },
            };
        }

        /// <summary>
        /// Chargement d'un dictionnaire simple pour renvoyer les informations de validation serveur
        /// "key => [ "error1", "error2", ... "errorN" ]".
        /// Suppression des entrées sans message
        /// </summary>
        /// <param name="modelState">ModelStateDictionary à renvoyer au client</param>
        /// <returns>Informations de validation</returns>
        public static Dictionary<string, string[]> ProcessModelState(System.Web.Http.ModelBinding.ModelStateDictionary modelState)
        {
            // TODO les clés peuevtn contenir uniquement le nom de la classe sans propriété à traiter ... pour le moment on passe
            var modelStateClean = modelState.Where(ms => ms.Key.Contains('.')).ToList();

            return modelStateClean
                .ToDictionary(
                    entry => LowerFirstLetter(entry.Key.Split('.')[1]), // ex : emailVm.SelectedRole on ne conserve que selectedRole
                    entry => entry.Value.Errors
                                .Where(error => !string.IsNullOrEmpty(error.ErrorMessage) || error.Exception != null)
                                .Select(error => !string.IsNullOrEmpty(error.ErrorMessage) ? error.ErrorMessage : error.Exception.Message).ToArray())
                                .Where(entry => entry.Value.Any())
                                .ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        #endregion

        #region Private

        /// <summary>
        /// Mettre la première lettre d'un mot en minuscule
        /// </summary>
        /// <param name="s">Chaine à modifier</param>
        /// <returns>Chaine modifiée</returns>
        private static string LowerFirstLetter(string s)
        {
            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }

        #endregion
    }
}