using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Microsoft.SqlServer.Server;
using RDVMedicaux.Dal.Base;
using RDVMedicaux.AppException;

namespace RDVMedicaux.Dal.Base
{
    /// <summary>
    /// Utilitaire de gestion des accès spécifique à SqlServer
    /// </summary>
    public class SqlServerConnectionHelper : ConnectionHelper
    {
        #region Constantes

        /// <summary>
        /// Constante associée au message d'erreur quand le type de valeur est inconnu
        /// </summary>
        private const string TYPEVALUENOTRECOGNIZED = "Type of value is not recognized.";

        #endregion Constants

        #region Manage parameters

        /// <summary>
        /// Ajout d'un paramètre à une liste de paramètres existante
        /// </summary>
        /// <param name="list">Liste de paramètres</param>
        /// <param name="name">Nom du paramètre</param>
        /// <param name="value">Valeur du paramètre</param>
        /// <returns>La liste avec les paramètres ajoutés</returns>
        public static List<DbParameter> AddParamToList(List<DbParameter> list, string name, object value)
        {
            // Test if list initialized
            if (list == null)
            {
                throw new ArgumentNullException("liste", TYPEVALUENOTRECOGNIZED);
            }

            // Added parameter
            list.Add(SqlServerConnectionHelper.CreateParam(name, value, ParameterDirection.Input));

            // Back to list
            return list;
        }

        /// <summary>
        /// Ajout d'un paramètre à une liste de paramètres existante
        /// </summary>
        /// <param name="list">Liste de paramètres</param>
        /// <param name="name">Nom du paramètre</param>
        /// <param name="value">Valeur du paramètre</param>
        /// <param name="direction">Sens du paramètre</param>
        /// <returns>La liste avec les paramètres ajoutés</returns>
        public static List<DbParameter> AddParamToList(List<DbParameter> list, string name, object value, ParameterDirection direction)
        {
            // Test if list is initialized
            if (list == null)
            {
                throw new ArgumentNullException("list", TYPEVALUENOTRECOGNIZED);
            }

            // Added parameter
            list.Add(SqlServerConnectionHelper.CreateParam(name, value, direction));

            // Back to list
            return list;
        }

        /// <summary>
        /// Ajout d'un paramètre de type <c>TableValueParameter</c>
        /// </summary>
        /// <param name="query">la requête à exécuter</param>
        /// <param name="list">Liste de paramètres</param>
        /// <param name="name">Nom du paramètre</param>
        /// <param name="value">Liste de données</param>
        /// <param name="typeName">Nom du type table</param>
        public static void AddParamTvp(ref string query, List<DbParameter> list, string name, List<SqlDataRecord> value, string typeName)
        {
            // Test if list is initialized
            if (list == null)
            {
                throw new ArgumentNullException("list", TYPEVALUENOTRECOGNIZED);
            }

            // si un param de type table est vide => erreur
            // on ne peut lui affecter la valeur DBNull ou une valeur par défaut => erreur
            // pour l'initialiser à une valeur "vide", on ajoute la déclaration directment depuis le sql
            // et ainsi il est n'est pas nécessaire de gérer sa valeur côté c#
            if (value.Count > 0)
            {
                list.Add(new SqlParameter(name, SqlDbType.Structured) { TypeName = typeName, Value = value });
            }
            else
            {   // ex : "declare @listCapacite alix.capacite_list_tbltype; " + query;
                query = string.Format("declare {0} {1}; ", name, typeName) + query;
            }
        }

        /// <summary>
        /// Création d'un paramètre en fonction du type d'objet
        /// </summary>
        /// <param name="name">Nom du paramètre</param>
        /// <param name="value">Valeur du paramètre</param>
        /// <param name="direction">Sens du paramètre</param>
        /// <returns>Paramètre initialisé</returns>
        public static DbParameter CreateParam(string name, object value, ParameterDirection direction)
        {
            SqlParameter param = null;

            // Depending on the type of the parameter we make a link
            // with SqlDbType corresponding in .NET
            if (value == null)
            {
                param = new SqlParameter();
                param.ParameterName = name;
                param.Value = DBNull.Value;
            }
            else if (value is string && value.ToString().Length < 256)
            {
                param = new SqlParameter(name, SqlDbType.NVarChar);
            }
            else if (value is string)
            {
                param = new SqlParameter(name, SqlDbType.Text);
            }
            else if (value is short)
            {
                param = new SqlParameter(name, SqlDbType.SmallInt);
            }
            else if (value is int)
            {
                param = new SqlParameter(name, SqlDbType.Int);
            }
            else if (value is long)
            {
                param = new SqlParameter(name, SqlDbType.BigInt);
            }
            else if (value is decimal)
            {
                param = new SqlParameter(name, SqlDbType.Decimal);
            }
            else if (value is float)
            {
                param = new SqlParameter(name, SqlDbType.Float);
            }
            else if (value is double)
            {
                param = new SqlParameter(name, SqlDbType.Float);
            }
            else if (value is DateTime)
            {
                param = new SqlParameter(name, SqlDbType.DateTime2);
            }
            else if (value is TimeSpan)
            {
                param = new SqlParameter(name, SqlDbType.Time);
            }
            else if (value is bool)
            {
                // The type Boolean is passed as int (0 or 1)
                if ((bool)value)
                {
                    value = 1;
                }
                else
                {
                    value = 0;
                }

                // Creation of parameter
                param = new SqlParameter(name, SqlDbType.Int);
            }
            else if (value is bool[])
            {
                int[] array = new int[((Array)value).Length];

                // We transfer the values in the new table
                for (int i = 0; i < ((Array)value).Length; i++)
                {
                    if ((bool)((Array)value).GetValue(i))
                    {
                        array[i] = 1;
                    }
                    else
                    {
                        array[i] = 0;
                    }
                }

                // Creation of parameter
                value = array;
                param = new SqlParameter(name, SqlDbType.Int);
            }
            else if (((value is ushort) || (value is uint)) || (value is ulong))
            {
                value = Convert.ToInt64(value);
                param = new SqlParameter(name, SqlDbType.BigInt);
            }
            else if (value is byte[])
            {
                param = new SqlParameter(name, SqlDbType.VarBinary);
            }
            else
            {
                throw new ArgumentException(TYPEVALUENOTRECOGNIZED, "value");
            }

            // It affects the value calculated or native
            param.Value = value;
            param.Direction = direction;

            // It returns the created parameter
            return param;
        }

        #endregion Manage parameters

        #region Get dataset

        /// <summary>
        /// Retourne un DataSet depuis un SqlDataAdapter
        /// </summary>
        /// <param name="commandText">Requête à exécuter</param>
        /// <param name="connection">Connexion à la base</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns>Dataset alimenté depuis la liste de résultat</returns>
        public static DataSet GetDataSet(string commandText, DbConnection connection, List<DbParameter> parameters)
        {
            DataSet dataSet = new DataSet();

            // init DataAdapter
            using (DbDataAdapter tempAdapter = new System.Data.SqlClient.SqlDataAdapter())
            {
                // init Command
                using (DbCommand command = ConnectionHelper.CreateCommand(connection, CommandType.Text, commandText, parameters))
                {
                    // apply select on adapter
                    tempAdapter.SelectCommand = command;

                    // Filling the Dataset
                    tempAdapter.Fill(dataSet);
                }
            }

            // return dataset
            return dataSet;
        }

        #endregion Get dataset

        #region Execute procedure

        /// <summary>
        /// Exécute un procédure sans paramètre
        /// </summary>
        /// <param name="commandText">Nom de la procédure</param>
        /// <param name="connexion">Connexion à la base de donnée</param>
        public static void ExecuteProc(string commandText, DbConnection connexion)
        {
            SqlServerConnectionHelper.ExecuteProc(commandText, connexion, null, false);
        }

        /// <summary>
        /// Exécute un procédure avec paramètres
        /// Si <see cref="checkConcurrency"/> est vrai on n'utilise pas la valeur en retour de <see cref="ExecuteNonQuery"/>.
        /// <see cref="RowCount"/> doit être effectué manuellement dans toutes les procédures.
        /// </summary>
        /// <param name="commandText">Nom de la procédure</param>
        /// <param name="connection">Connexion à la base de donnée</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <param name="checkConcurrency">Concurrence à vérifier oui non</param>
        public static void ExecuteProc(string commandText, DbConnection connection, List<DbParameter> parameters, bool checkConcurrency)
        {
            // to check concurrency add parameter
            // stored procedure must contain an output param named @nbModifiedLine
            if (checkConcurrency)
            {
                SqlServerConnectionHelper.AddParamToList(parameters, "@nbModifiedLine", 0, ParameterDirection.Output);
            }

            using (DbCommand command = ConnectionHelper.CreateCommand(connection, CommandType.StoredProcedure, commandText, parameters))
            {
                try
                {
                    // Command execution : 
                    // dont use the return value of ExecuteNonQuery because NOCOUNT could be OFF or ON
                    // manage nb lines manually with @nbModifiedLine
                    command.ExecuteNonQuery();

                    if (checkConcurrency)
                    {
                        // read parameter to check concurreny
                        DbParameter paramCount = parameters.First(p => p.ParameterName == "@nbModifiedLine");
                        int count = int.Parse(paramCount.Value.ToString());

                        if (count <= 0)
                        {
                            throw new CustomException(CustomExceptionErrorCode.ConcurrentAccess);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    int errorNumber = ex.Number;

                    // Managing error code sent from procedure with the try catch
                    // If an error occured in the procedure, a message is sent with all the informations
                    // (the error code is at the beginning of the message)
                    if (errorNumber == 50000)
                    {
                        errorNumber = SqlServerConnectionHelper.GetNumberException(ex.Message);
                    }

                    // throw custom exception 
                    SqlServerConnectionHelper.ManageExceptionByErrorNumber(errorNumber, ex);

                    // default
                    throw;
                }
            }
        }

        /// <summary>
        /// Exécute un procédure avec paramètres en vérifiant la concurrence
        /// </summary>
        /// <param name="commandText">Nom de la procédure</param>
        /// <param name="connection">Connexion à la base de donnée</param>
        /// <param name="parameters">Liste de paramètres</param>
        public static void ExecuteProcWithConcurrency(string commandText, DbConnection connection, List<DbParameter> parameters)
        {
            SqlServerConnectionHelper.ExecuteProc(commandText, connection, parameters, true);
        }

        /// <summary>
        /// Exécute un procédure avec paramètres
        /// </summary>
        /// <param name="commandText">Nom de la procédure</param>
        /// <param name="connection">Connexion à la base de donnée</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns><see cref="DbDataReader"/> alimenté avec le résultat de la requête</returns>
        public static DbDataReader ExecuteProcWithReader(string commandText, DbConnection connection, List<DbParameter> parameters)
        {
            DbDataReader reader = null;

            using (DbCommand command = ConnectionHelper.CreateCommand(connection, CommandType.StoredProcedure, commandText, parameters))
            {
                try
                {
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (SqlException ex)
                {
                    int errorNumber = ex.Number;

                    // Managing error code sent from procedure with the try catch
                    // If an error occured in the procedure, a message is sent with all the informations
                    // (the error code is at the beginning of the message)
                    if (errorNumber == 50000)
                    {
                        errorNumber = SqlServerConnectionHelper.GetNumberException(ex.Message);
                    }

                    // throw custom exception 
                    SqlServerConnectionHelper.ManageExceptionByErrorNumber(errorNumber, ex);

                    // default
                    throw;
                }
            }

            return reader;
        }

        /// <summary>
        /// Exécute un procédure avec paramètres en vérifiant la concurrence
        /// </summary>
        /// <param name="commandText">Nom de la procédure</param>
        /// <param name="connection">Connexion à la base de donnée</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <param name="actionReader">Méthode appelée lors de chaque itération sur le datareader</param>
        public static void ExecuteProcWithReaderWithConcurrency(string commandText, DbConnection connection, List<DbParameter> parameters, Action<IDataReader> actionReader)
        {
            // to check concurrency add parameter
            // stored procedure must contain an output param named @nbModifiedLine
            SqlServerConnectionHelper.AddParamToList(parameters, "@nbModifiedLine", 0, ParameterDirection.Output);

            using (DbCommand command = ConnectionHelper.CreateCommand(connection, CommandType.StoredProcedure, commandText, parameters))
            {
                try
                {
                    using (DbDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            // if we want to get output parameter we need to process and close the datareader.
                            // we pass the treatment on datareader as parameter 
                            actionReader(reader);
                        }
                    }

                    // Get output params AFTER we've processed and CLOSED the SqlDataReadeer 
                    int count = (int)command.Parameters["@nbModifiedLine"].Value;

                    if (count <= 0)
                    {
                        throw new CustomException(CustomExceptionErrorCode.ConcurrentAccess);
                    }
                }
                catch (SqlException ex)
                {
                    int errorNumber = ex.Number;

                    // Managing error code sent from procedure with the try catch
                    // If an error occured in the procedure, a message is sent with all the informations
                    // (the error code is at the beginning of the message)
                    if (errorNumber == 50000)
                    {
                        errorNumber = SqlServerConnectionHelper.GetNumberException(ex.Message);
                    }

                    // throw custom exception 
                    SqlServerConnectionHelper.ManageExceptionByErrorNumber(errorNumber, ex);

                    // default
                    throw;
                }
            }
        }

        /// <summary>
        /// Exécute un procédure avec paramètres en vérifiant la concurrence
        /// </summary>
        /// <param name="commandText">Nom de la procédure</param>
        /// <param name="connection">Connexion à la base de donnée</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <param name="actionReader">Méthode appelée lors de chaque itération sur le datareader</param>
        /// <param name="actionReader2">Méthode appelée lors de chaque itération du second datareader</param>
        public static void ExecuteProcWithReaderWithConcurrency(string commandText, DbConnection connection, List<DbParameter> parameters, Action<IDataReader> actionReader, Action<IDataReader> actionReader2)
        {
            // to check concurrency add parameter
            // stored procedure must contain an output param named @nbModifiedLine
            SqlServerConnectionHelper.AddParamToList(parameters, "@nbModifiedLine", 0, ParameterDirection.Output);

            using (DbCommand command = ConnectionHelper.CreateCommand(connection, CommandType.StoredProcedure, commandText, parameters))
            {
                try
                {
                    using (DbDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            // if we want to get output parameter we need to process and close the datareader.
                            // we pass the treatment on datareader as parameter 
                            actionReader(reader);
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            // if we want to get output parameter we need to process and close the datareader.
                            // we pass the treatment on datareader as parameter 
                            actionReader2(reader);
                        }
                    }

                    // Get output params AFTER we've processed and CLOSED the SqlDataReadeer 
                    int count = (int)command.Parameters["@nbModifiedLine"].Value;

                    if (count <= 0)
                    {
                        throw new CustomException(CustomExceptionErrorCode.ConcurrentAccess);
                    }
                }
                catch (SqlException ex)
                {
                    int errorNumber = ex.Number;

                    // Managing error code sent from procedure with the try catch
                    // If an error occured in the procedure, a message is sent with all the informations
                    // (the error code is at the beginning of the message)
                    if (errorNumber == 50000)
                    {
                        errorNumber = SqlServerConnectionHelper.GetNumberException(ex.Message);
                    }

                    // throw custom exception 
                    SqlServerConnectionHelper.ManageExceptionByErrorNumber(errorNumber, ex);

                    // default
                    throw;
                }
            }
        }

        #endregion Execute procedure

        #region Execute requête INSERT, UPDATE or DELETE

        /// <summary>
        /// Requête de type INSERT, UPDATE or DELETE
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="connexion">Connexion courante</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <param name="checkConcurrency">Concurrence à vérifier oui non</param>
        /// <returns>Nombre de lignes modifiées</returns>
        public static int ExecuteSql(string commandText, DbConnection connexion, List<DbParameter> parameters, bool checkConcurrency)
        {
            int rowCount = 0;

            try
            {
                // Ajout d'un RowCount pour déterminer le nombres de lignes modifiées par la requête
                commandText = commandText + " SELECT @@ROWCOUNT ";

                rowCount = ConnectionHelper.ExecuteScalar<int>(commandText, CommandType.Text, connexion, parameters);

                // check concurreny
                if (checkConcurrency)
                {
                    if (rowCount <= 0)
                    {
                        throw new CustomException(CustomExceptionErrorCode.ConcurrentAccess);
                    }
                }
            }
            catch (SqlException ex)
            {
                int errorNumber = ex.Number;

                // throw custom exception 
                SqlServerConnectionHelper.ManageExceptionByErrorNumber(errorNumber, ex);

                // default
                throw;
            }

            return rowCount;
        }

        /// <summary>
        /// Requête de type INSERT, UPDATE or DELETE
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="connexion">Connexion courante</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns>Nombre de lignes modifiées</returns>
        public static int ExecuteSql(string commandText, DbConnection connexion, List<DbParameter> parameters)
        {
            return SqlServerConnectionHelper.ExecuteSql(commandText, connexion, parameters, false);
        }

        /// <summary>
        /// Requête de type INSERT avec retour de la ligne insérée
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="connexion">Connexion courante</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns>ID de la ligne insérée</returns>
        public static int ExecuteSqlWithIdentity(string commandText, DbConnection connexion, List<DbParameter> parameters)
        {
            int id = 0;

            try
            {
                // Récupération de l'ID qui vient d'être inséré
                commandText = commandText + " SELECT ISNULL(SCOPE_IDENTITY(), 0) ";

                id = (int)ConnectionHelper.ExecuteScalar<decimal>(commandText, CommandType.Text, connexion, parameters);
            }
            catch (SqlException ex)
            {
                int errorNumber = ex.Number;

                // throw custom exception 
                SqlServerConnectionHelper.ManageExceptionByErrorNumber(errorNumber, ex);

                // default
                throw;
            }

            return id;
        }

        /// <summary>
        /// Requête de type INSERT, UPDATE or DELETE avec gestion de la concurrence d'accès
        /// </summary>
        /// <param name="commandText">Requête texte</param>
        /// <param name="connexion">Connexion courante</param>
        /// <param name="parameters">Liste de paramètres</param>
        /// <returns>Nombre de lignes modifiées</returns>
        public static int ExecuteSqlWithConcurrency(string commandText, DbConnection connexion, List<DbParameter> parameters)
        {
            return SqlServerConnectionHelper.ExecuteSql(commandText, connexion, parameters, true);
        }

        #endregion

        #region Private

        /// <summary>
        /// Lance des exceptions spécifiques en fonction du numéro d'erreur
        /// </summary>
        /// <param name="errorNumber">Numéro d'erreur</param>
        /// <param name="ex">exception d'origine</param>
        private static void ManageExceptionByErrorNumber(int errorNumber, Exception ex)
        {
            switch (errorNumber)
            {
                // Error code for "Violation of UNIQUE KEY constraint"
                case 2627:
                    throw new CustomException(ex.Message, ex, CustomExceptionErrorCode.UniqueKeyConstraint);

                // Error code for "Cannot insert duplicate key row with UNIQUE INDEX"
                case 2601:
                    throw new CustomException(ex.Message, ex, CustomExceptionErrorCode.UniqueKeyConstraint);

                // Error code for "Conflict with the FOREIGN KEY constraint"
                case 547:
                    throw new CustomException(ex.Message, ex, CustomExceptionErrorCode.DeleteForeignKey);
            }
        }
        #endregion
    }
}