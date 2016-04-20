using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDVMedicaux.Dal.Base;
using RDVMedicaux.Model;
using System.Data;

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


        #region Select

        /// <summary>
        /// Récupération de la liste des practiciens en fonctions des criteres de la recherche
        /// </summary>
        /// <param name="filter">Critères de filtre</param>
        /// <returns>Liste des practiciens</returns>
        public List<Practicien> ListPracticiens(Dictionary<Practicien.Criteria, object> filter)
        {
            // Initialisation
            QueryBuilder qb = new QueryBuilder();
            List<DbParameter> sqlParams = new List<DbParameter>();
            List<Practicien> result = new List<Practicien>();

            // Construction de la requête
            qb.AddSelect(
            @" PRA.ID,
            PRA.Nom,
            PRA.Prenom,
            PRA.Specialite1,
            PRA.accepteCarteVitale,
            PRA.Ville,
            PRA.Adresse,
            PRA.CP,
            PRA.refSecteurTarifaire
            ")
            .AddFrom(ConstDb.Tables.Practicien, "PRA");

            if (filter != null)
            {
                // Ajout des critères de filtre
                if (filter.ContainsKey(Practicien.Criteria.Lieu))
                {
                    qb.AddWhere("PRA.Ville LIKE @PRA_VILLE");
                    SqlServerConnectionHelper.AddParamToList(sqlParams, "@PRA_VILLE", "%" + filter[Practicien.Criteria.Lieu] + "%");
                }

                if (filter.ContainsKey(Practicien.Criteria.Specialite))
                {
                    qb.AddWhere("( PRA.specialite1 = @PRA_SPE1 OR PRA.specialite2 = @PRA_SPE2 OR PRA.specialite3 = @PRA_SPE3 )");
                    SqlServerConnectionHelper.AddParamToList(sqlParams, "@PRA_SPE1", filter[Practicien.Criteria.Specialite]);
                    SqlServerConnectionHelper.AddParamToList(sqlParams, "@PRA_SPE2", filter[Practicien.Criteria.Specialite]);
                    SqlServerConnectionHelper.AddParamToList(sqlParams, "@PRA_SPE3", filter[Practicien.Criteria.Specialite]);
                }

                if (filter.ContainsKey(Practicien.Criteria.SortBySecteur1))
                {
                    if (filter[Practicien.Criteria.SortBySecteur1].ToString().Equals("True"))
                    {
                        qb.AddWhere("PRA.refSecteurTarifaire LIKE @PRA_Secteur1");
                        SqlServerConnectionHelper.AddParamToList(sqlParams, "@PRA_Secteur1", 1);
                    }
                }
            }

            // Exécution de la requête
            using (IDataReader reader = SqlServerConnectionHelper.GetReader(qb.ToSql(), this.Connection, sqlParams))
            {
                while (ConnectionHelper.LimitedRead(reader, result.Count))
                {
                    Practicien practicient = new Practicien();

                    practicient.Id = SqlServerConnectionHelper.ToInt(reader["ID"]);
                    practicient.Nom = SqlServerConnectionHelper.ToString(reader["Nom"]);
                    practicient.Prenom = SqlServerConnectionHelper.ToString(reader["Prenom"]);
                    practicient.Specialite1 = SqlServerConnectionHelper.ToString(reader["Specialite1"]);
                    practicient.Ville = SqlServerConnectionHelper.ToString(reader["Ville"]);
                    practicient.AcceptCarteVitale = SqlServerConnectionHelper.ToBool(reader["accepteCarteVitale"]);
                    practicient.SecteurTarifaire = SqlServerConnectionHelper.ToString(reader["refSecteurTarifaire"]);
                    practicient.Adresse = SqlServerConnectionHelper.ToString(reader["Adresse"]);
                    practicient.CP = SqlServerConnectionHelper.ToInt(reader["CP"]);

                    result.Add(practicient);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locLat"></param>
        /// <param name="locLong"></param>
        /// <returns></returns>
        public List<Practicien> GetPracticiensAround15km(Dictionary<Practicien.Criteria, object> filter)
        {
            // Initialisation
            QueryBuilder qb = new QueryBuilder();
            List<DbParameter> sqlParams = new List<DbParameter>();
            List<Practicien> result = new List<Practicien>();

            // Construction de la requête
            qb.AddSelect(
            @" PRA.ID,
            PRA.Nom,
            PRA.Prenom,
            PRA.Specialite1,
            PRA.accepteCarteVitale,
            PRA.Ville,
            PRA.Adresse,
            PRA.CP,
            PRA.LocLat,
            PRA.LocLong,
            PRA.refSecteurTarifaire
            ")
            .AddFrom(ConstDb.Tables.Practicien, "PRA");

            if (filter != null)
            {
                if (filter.ContainsKey(Practicien.Criteria.Specialite))
                {
                    qb.AddWhere("( PRA.specialite1 = @PRA_SPE1 OR PRA.specialite2 = @PRA_SPE2 OR PRA.specialite3 = @PRA_SPE3 )");
                    SqlServerConnectionHelper.AddParamToList(sqlParams, "@PRA_SPE1", filter[Practicien.Criteria.Specialite]);
                    SqlServerConnectionHelper.AddParamToList(sqlParams, "@PRA_SPE2", filter[Practicien.Criteria.Specialite]);
                    SqlServerConnectionHelper.AddParamToList(sqlParams, "@PRA_SPE3", filter[Practicien.Criteria.Specialite]);
                }

                if (filter.ContainsKey(Practicien.Criteria.SortBySecteur1))
                {
                    if (filter[Practicien.Criteria.SortBySecteur1].ToString().Equals("True"))
                    {
                        qb.AddWhere("PRA.refSecteurTarifaire LIKE @PRA_Secteur1");
                        SqlServerConnectionHelper.AddParamToList(sqlParams, "@PRA_Secteur1", 1);
                    }
                }
            }

            // Exécution de la requête
            using (IDataReader reader = SqlServerConnectionHelper.GetReader(qb.ToSql(), this.Connection, sqlParams))
            {
                while (ConnectionHelper.LimitedRead(reader, result.Count))
                {
                    double locLat = double.Parse(filter[Practicien.Criteria.LocLat].ToString());
                    double locLong = double.Parse(filter[Practicien.Criteria.LocLong].ToString());
                    int distance = Convert.ToInt32(Util.Distance(locLat, locLong, SqlServerConnectionHelper.ToDouble(reader["LocLat"]), SqlServerConnectionHelper.ToDouble(reader["LocLong"])));
                    
                    if (distance <= 15)
                    {
                        Practicien practicient = new Practicien();

                        practicient.Id = SqlServerConnectionHelper.ToInt(reader["ID"]);
                        practicient.Nom = SqlServerConnectionHelper.ToString(reader["Nom"]);
                        practicient.Prenom = SqlServerConnectionHelper.ToString(reader["Prenom"]);
                        practicient.Specialite1 = SqlServerConnectionHelper.ToString(reader["Specialite1"]);
                        practicient.Ville = SqlServerConnectionHelper.ToString(reader["Ville"]);
                        practicient.AcceptCarteVitale = SqlServerConnectionHelper.ToBool(reader["accepteCarteVitale"]);
                        practicient.Adresse = SqlServerConnectionHelper.ToString(reader["Adresse"]);
                        practicient.SecteurTarifaire = SqlServerConnectionHelper.ToString(reader["refSecteurTarifaire"]);
                        practicient.CP = SqlServerConnectionHelper.ToInt(reader["CP"]);
                        practicient.Distance = distance;

                        result.Add(practicient);
                    }
                }
            }

            return result;
        }
        #endregion Select
    }
}
