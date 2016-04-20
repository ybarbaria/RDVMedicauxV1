using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

using RDVMedicaux.AppException;

namespace RDVMedicaux.Dal.Base
{
    /// <summary>
    /// Classe d'aide aux interactions avec la base de données
    /// </summary>
    public abstract class ConnectionHelper
    {
        #region Constants

        /// <summary>
        /// Définit le délimiteur utilisé dans les messages d'erreur levés par les procédures stockées.
        /// Le délimiteur est le caractère juste après le code erreur.
        /// </summary>
        private const string CsDelimiterInSqlException = "-";

        #endregion Constants

        #region Properties

        /// <summary>
        /// Obtient ou définit le nombre maximum de lignes à lire en base de données
        /// TODO définir une valeur dans le config
        /// </summary>
        private static int numberMaxRow = int.Parse(ConfigurationManager.AppSettings.Get("sql.maxcount"));

        #endregion

        #region Delegate

        /// <summary>
        /// Méthodes délégué pour parsing générique
        /// </summary>
        /// <typeparam name="T">Destination type</typeparam>
        /// <param name="s">String source</param>
        /// <returns>Type attendu</returns>
        public delegate T ParseMethod<T>(string s);

        #endregion Delegate

        #region Static Property

        /// <summary>
        /// Obtient le nombre maximal de ligne pour le résultat d'une requête de recherche
        /// </summary>
        public static int NumberMaxRow
        {
            get
            {
                return numberMaxRow;
            }
        }
        #endregion

        #region Static functions to manage Connection

        /// <summary>
        /// Obtient une connexion depuis la factory
        /// </summary>
        /// <param name="settings">Paramétrage de connexion</param>
        /// <returns>Instance de la connexion</returns>
        public static DbConnection GetConnection(ConnectionStringSettings settings)
        {
            DbConnection connexion = null;

            // Recovery of the connection in the factory
            connexion = DbProviderFactories.GetFactory(settings.ProviderName).CreateConnection();

            connexion.ConnectionString = settings.ConnectionString;

            // It returns the connection instance
            return connexion;
        }

        /// <summary>
        /// Obtient une connexion depuis la factory
        /// </summary>
        /// <param name="connName">Nom de la connexion</param>
        /// <returns>Instance de la connexion</returns>
        public static DbConnection GetConnection(string connName)
        {
            return ConnectionHelper.GetConnection(ConfigurationManager.ConnectionStrings[connName]);
        }

        #endregion Static functions to manage Connection

        #region Static functions to manage Exception

        /// <summary>
        /// Vérifie si une requête renvoie une valeur. Lève une Exception si ce n'est pas le cas.
        /// </summary>
        /// <param name="item">Objet rempli par la requête</param>
        /// <param name="sql">La requête SQL</param>
        /// <param name="message">Un message a afficher</param>
        public static void CheckNullValueWithException(object item, string sql, string message)
        {
            if (item == null)
            {
                throw new Exception(string.Format("Aucune valeur retournée par la requête: {0}, {1}", sql, message));
            }
        }

        #endregion Static functions to manage Exception

        #region Static functions ExecuteNonQuery

        /// <summary>
        /// Exécute une requête de type INSERT, UPDATE or DELETE
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="commandType">Requête type</param>
        /// <param name="connexion">Connexion courante</param>
        /// <returns>Nombre de lignes impactées</returns>
        public static int ExecuteNonQuery(string commandText, CommandType commandType, DbConnection connexion)
        {
            return ConnectionHelper.ExecuteNonQuery(commandText, commandType, connexion, null);
        }

        /// <summary>
        /// Exécute une requête de type INSERT, UPDATE or DELETE
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="commandType">Requête type</param>
        /// <param name="connexion">Connexion courante</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns>Nombre de lignes impactées</returns>
        public static int ExecuteNonQuery(string commandText, CommandType commandType, DbConnection connexion, List<DbParameter> parameters)
        {
            using (DbCommand command = ConnectionHelper.CreateCommand(connexion, commandType, commandText, parameters))
            {
                // Command execution
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Exécute une requête de type INSERT, UPDATE or DELETE
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="connexion">Connexion courante</param>
        /// <returns>Nombre de lignes impactées</returns>
        public static int ExecuteNonQuery(string commandText, DbConnection connexion)
        {
            return ConnectionHelper.ExecuteNonQuery(commandText, CommandType.Text, connexion, null);
        }

        /// <summary>
        /// Exécute une requête de type INSERT, UPDATE or DELETE
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="connexion">Connexion courante</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns>Nombre de lignes impactées</returns>
        public static int ExecuteNonQuery(string commandText, DbConnection connexion, List<DbParameter> parameters)
        {
            return ConnectionHelper.ExecuteNonQuery(commandText, CommandType.Text, connexion, parameters);
        }

        #endregion Static functions ExecuteNonQuery

        #region Static functions ExecuteScalar

        /// <summary>
        /// Requête de type scalaire
        /// </summary>
        /// <typeparam name="T">Type attendu en retour de la requête</typeparam>
        /// <param name="commandText">Requête texte</param>
        /// <param name="commandType">Requête type</param>
        /// <param name="connexion">Connexion courante</param>
        /// <returns>Résultat de la requête</returns>
        public static T ExecuteScalar<T>(string commandText, CommandType commandType, DbConnection connexion)
        {
            return ConnectionHelper.ExecuteScalar<T>(commandText, commandType, connexion, null);
        }

        /// <summary>
        /// Requête de type scalaire
        /// </summary>
        /// <typeparam name="T">Type attendu en retour de la requête</typeparam>
        /// <param name="commandText">Requête texte</param>
        /// <param name="commandType">Requête type</param>
        /// <param name="connexion">Connexion courante</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns>Résultat de la requête</returns>
        public static T ExecuteScalar<T>(string commandText, CommandType commandType, DbConnection connexion, List<DbParameter> parameters)
        {
            // result of type T
            T result;

            // Command creation
            using (DbCommand command = ConnectionHelper.CreateCommand(connexion, commandType, commandText, parameters))
            {
                // Query Execution
                result = (T)command.ExecuteScalar();
            }

            // It returns the result
            return result;
        }

        #endregion Static functions ExecuteScalar

        #region Static functions GetReader

        /// <summary>
        /// Obtenir le résultat d'une requête Select
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="commandType">Requête type</param>
        /// <param name="connexion">Connexion courante</param>
        /// <returns>Reader du jeu de résultat</returns>
        public static DbDataReader GetReader(string commandText, CommandType commandType, DbConnection connexion)
        {
            return ConnectionHelper.GetReader(commandText, commandType, connexion, null);
        }

        /// <summary>
        /// Obtenir le résultat d'une requête Select
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="connexion">Connexion courante</param>
        /// <returns>Reader du jeu de résultat</returns>
        public static DbDataReader GetReader(string commandText, DbConnection connexion)
        {
            return ConnectionHelper.GetReader(commandText, CommandType.Text, connexion, null);
        }

        /// <summary>
        /// Obtenir le résultat d'une requête Select
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="commandType">Requête type</param>
        /// <param name="connexion">Connexion courante</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns>Reader du jeu de résultat</returns>
        public static DbDataReader GetReader(string commandText, CommandType commandType, DbConnection connexion, List<DbParameter> parameters)
        {
            DbDataReader reader = null;

            using (DbCommand command = ConnectionHelper.CreateCommand(connexion, commandType, commandText, parameters))
            {
                reader = command.ExecuteReader(CommandBehavior.Default);
            }

            return reader;
        }

        /// <summary>
        /// Obtenir le résultat d'une requête Select
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="connexion">Connexion courante</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns>Reader du jeu de résultat</returns>
        public static DbDataReader GetReader(string commandText, DbConnection connexion, List<DbParameter> parameters)
        {
            DbDataReader reader = null;

            using (DbCommand command = ConnectionHelper.CreateCommand(connexion, CommandType.Text, commandText, parameters))
            {
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }

            return reader;
        }

        /// <summary>
        /// Obtenir le résultat d'une requête Select avec gestion de l'absence de données
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="connexion">Connexion courante</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns>Reader du jeu de résultat</returns>
        public static DbDataReader GetReaderWithNoData(string commandText, DbConnection connexion, List<DbParameter> parameters)
        {
            DbDataReader reader = null;

            using (DbCommand command = ConnectionHelper.CreateCommand(connexion, CommandType.Text, commandText, parameters))
            {
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }

            if (!reader.HasRows)
            {
                throw new CustomException(CustomExceptionErrorCode.NoDataFound);
            }

            return reader;
        }

        /// <summary> 
        /// Obtenir le résultat d'une requête Select avec gestion de l'absence de données
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="connexion">Connexion courante</param>
        /// <returns>Reader du jeu de résultat</returns>
        public static DbDataReader GetReaderWithNoData(string commandText, DbConnection connexion)
        {
            return GetReaderWithNoData(commandText, connexion, null);
        }

        #endregion Static functions GetReader

        #region Static functions GetDataTable

        /// <summary>
        /// Obtenir une datatable
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="commandType">Requête type</param>
        /// <param name="connexion">Connexion courante</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns>Datatable obtenu à partir de la requête</returns>
        public static DataTable GetDataTable(string commandText, CommandType commandType, DbConnection connexion, List<DbParameter> parameters)
        {
            DataTable dt = new DataTable();

            using (DbDataReader dataReader = GetReader(commandText, commandType, connexion, parameters))
            {
                dt.Load(dataReader, LoadOption.OverwriteChanges);
            }

            return dt;
        }

        #endregion Static functions GetDataTable

        #region Static functions to get data and tools

        /// <summary>
        /// Lit le reader et vérifie s'il faut continuer la lecture
        /// </summary>
        /// <param name="reader">le reader qui transporte les données</param>
        /// <param name="count">Le nombre d'élément couramment dans la liste remplie par le reader</param>
        /// <returns>Vrai si on peut continuer (le reader n'a pas fini et la liste d'objet n'a pas atteint la limite)</returns>
        public static bool LimitedRead(IDataReader reader, int count)
        {
            return reader.Read() && count < ConnectionHelper.numberMaxRow;
        }

        /// <summary>
        /// Transforme un objet dans le type attendu
        /// </summary>
        /// <typeparam name="T">Type attendu</typeparam>
        /// <param name="readerValue">objet source</param>
        /// <param name="p_oMethod">Méthode de conversion à utiliser</param>
        /// <returns>object convertit</returns>
        public static T To<T>(object readerValue, ParseMethod<T> p_oMethod)
        {
            if (readerValue == DBNull.Value)
            {
                return default(T);
            }

            return p_oMethod(readerValue.ToString());
        }

        /// <summary>
        /// Pour obtenir un booléen
        /// </summary>
        /// <param name="readerValue">Valeur source</param>
        /// <returns>Valeur cible</returns>
        public static bool ToBool(object readerValue)
        {
            return bool.Parse(readerValue.ToString());
        }

        /// <summary>
        /// Pour obtenir une date
        /// </summary>
        /// <param name="readerValue">Valeur source</param>
        /// <returns>Valeur cible</returns>
        public static DateTime ToDateTime(object readerValue)
        {
            if (readerValue != DBNull.Value)
            {
                return DateTime.Parse(readerValue.ToString());
            }

            return new DateTime();
        }

        /// <summary>
        /// Pour obtenir une date nullable
        /// </summary>
        /// <param name="readerValue">Valeur source</param>
        /// <returns>Valeur cible</returns>
        public static DateTime? ToDateTimeNullable(object readerValue)
        {
            if (readerValue == DBNull.Value)
            {
                return null;
            }

            return new DateTime?(DateTime.Parse(readerValue.ToString()));
        }

        /// <summary>
        /// Pour obtenir un horaire
        /// </summary>
        /// <param name="readerValue">Valeur source</param>
        /// <returns>Valeur cible</returns>
        public static TimeSpan ToTimeSpan(object readerValue)
        {
            if (readerValue != DBNull.Value)
            {
                return TimeSpan.Parse(readerValue.ToString());
            }

            return new TimeSpan();
        }

        /// <summary>
        /// Pour obtenir une date nullable
        /// </summary>
        /// <param name="readerValue">Valeur source</param>
        /// <returns>Valeur cible</returns>
        public static TimeSpan? ToTimeSpanNullable(object readerValue)
        {
            if (readerValue == DBNull.Value)
            {
                return null;
            }

            return new TimeSpan?(TimeSpan.Parse(readerValue.ToString()));
        }

        /// <summary>
        /// Pour obtenir un entier 32 bits
        /// </summary>
        /// <param name="readerValue">Valeur source</param>
        /// <returns>Valeur cible</returns>
        public static int ToInt(object readerValue)
        {
            if (readerValue != DBNull.Value)
            {
                return int.Parse(readerValue.ToString());
            }

            return 0;
        }

        /// <summary>
        /// Pour obtenir un décimal
        /// </summary>
        /// <param name="readerValue">Valeur source</param>
        /// <returns>Valeur cible</returns>
        public static decimal ToDecimal(object readerValue)
        {
            if (readerValue != DBNull.Value)
            {
                return decimal.Parse(readerValue.ToString());
            }

            return 0;
        }

        /// <summary>
        /// Pour obtenir un décimal nullable
        /// </summary>
        /// <param name="readerValue">Valeur source</param>
        /// <returns>Valeur cible</returns>
        public static decimal? ToDecimalNullable(object readerValue)
        {
            if (readerValue == DBNull.Value)
            {
                return null;
            }

            return decimal.Parse(readerValue.ToString());
        }

        /// <summary>
        /// Pour obtenir un entier 32 bits nullable
        /// </summary>
        /// <param name="readerValue">Valeur source</param>
        /// <returns>Valeur cible</returns>
        public static int? ToIntNullable(object readerValue)
        {
            if (readerValue == DBNull.Value)
            {
                return null;
            }

            return new int?(int.Parse(readerValue.ToString()));
        }

        /// <summary>
        /// Pour obtenir un entier 64 bit
        /// </summary>
        /// <param name="readerValue">Valeur source</param>
        /// <returns>Valeur cible</returns>
        public static double ToDouble(object readerValue)
        {
            if (readerValue != DBNull.Value)
            {
                return double.Parse(readerValue.ToString());
            }

            return 0;
        }

        /// <summary>
        /// Pour transformer un flux binaire d'une colonne
        /// </summary>
        /// <param name="reader">Reader source</param>
        /// <param name="ordinal">Index de la colonne</param>
        /// <returns>Tableau d'octet</returns>
        public static byte[] ToRaw(DbDataReader reader, int ordinal)
        {
            byte[] buffer = null;

            // Check index
            if (reader[ordinal] == DBNull.Value)
            {
                return null;
            }

            // The buffer is dimensioned
            buffer = new byte[reader.GetBytes(ordinal, 0L, null, 0, 0)];

            // We copy into buffer
            reader.GetBytes(ordinal, 0L, buffer, 0, buffer.Length);

            // It returns the buffer
            return buffer;
        }

        /// <summary>
        /// Pour transformer un flux binaire d'une colonne
        /// </summary>
        /// <param name="reader">Reader source</param>
        /// <param name="col">Nom de la colonne</param>
        /// <returns>Tableau d'octet</returns>
        public static byte[] ToRaw(DbDataReader reader, string col)
        {
            // We browse the columns
            for (int i = 0; i < reader.FieldCount; i++)
            {
                // If it is found sending data
                if (reader.GetName(i).ToLower().Equals(col.ToLower()))
                {
                    return ConnectionHelper.ToRaw(reader, i);
                }
            }

            // Exception if column not found
            throw new ArgumentException("Le nom de la colonne est introuvable.", col);
        }

        /// <summary>
        /// Transforme un object en byte[]
        /// </summary>
        /// <param name="readerValue">Valeur du reader</param>
        /// <returns>Tableau d'octet</returns>
        public static byte[] ToByte(object readerValue)
        {
            byte[] buffer = null;

            if (readerValue != DBNull.Value)
            {
                return (byte[])readerValue;
            }

            return buffer;
        }

        /// <summary>
        /// Transforme un objet en chaine
        /// </summary>
        /// <param name="readerValue">Valeur source</param>
        /// <returns>Valeur cible</returns>
        public static string ToString(object readerValue)
        {
            if (readerValue != DBNull.Value)
            {
                return readerValue.ToString().TrimEnd(null);
            }

            return string.Empty;
        }

        /// <summary>
        /// Nombre de millisecondes depuis le 1 Janvier 1970
        /// </summary>
        /// <returns>Millisecondes double</returns>
        public static double GetMilliFromEpoch()
        {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        /// <summary>
        /// Retourne le code erreur de l'exception sql
        /// </summary>
        /// <param name="messageException">Message renvoyé par le raise error de la procédure stockée</param>
        /// <returns>Numéro d'erreur</returns>
        public static int GetNumberException(string messageException)
        {
            int errorCode = -1;
            int firstIndexOfDelimiter = messageException.IndexOf(CsDelimiterInSqlException);

            errorCode = int.Parse(messageException.Substring(0, firstIndexOfDelimiter));

            return errorCode;
        }

        /// <summary>
        /// Transforme un objet en booléen
        /// </summary>
        /// <param name="readerValue">Valeur du reader</param>
        /// <returns>Tableau d'octet</returns>
        public static bool? ToBoolNullable(object readerValue)
        {
            if (readerValue == DBNull.Value)
            {
                return null;
            }

            return new bool?(bool.Parse(readerValue.ToString()));
        }

        #endregion  Static functions to get data and tools

        #region Protected static functions

        /// <summary>
        /// Convertir les objets null vers DBNull
        /// </summary>
        /// <param name="value">Valeur source</param>
        /// <returns>Valeur cible</returns>
        protected static object CheckValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            return value;
        }

        /// <summary>
        /// Création d'un objet commande
        /// </summary>
        /// <param name="connection">Instance de la connexion</param>
        /// <param name="commandType">Type de la commande</param>
        /// <param name="commandText">Requête texte</param>
        /// <param name="parameters">Paramètres à transmettre</param>
        /// <returns>Commande créée</returns>
        protected static DbCommand CreateCommand(DbConnection connection, CommandType commandType, string commandText, List<DbParameter> parameters)
        {
            DbCommand command = null;

            // If the connection is not open is done
            if (connection != null && connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            // Initialize the object command with the internal parameters
            command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;

            // Processing parameters
            if (parameters != null)
            {
                // We browse the passed parameters 
                foreach (DbParameter parameter in parameters)
                {
                    // We test the null
                    parameter.Value = ConnectionHelper.CheckValue(parameter.Value);

                    // It is added to the command
                    command.Parameters.Add(parameter);
                }
            }

            // It returns the command
            return command;
        }

        #endregion Protected static functions
    }
}