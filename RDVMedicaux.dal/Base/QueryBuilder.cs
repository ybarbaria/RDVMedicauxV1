using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace RDVMedicaux.Dal.Base
{
    /// <summary>
    /// Utilitaire pour faciliter la création de requête Sql
    /// </summary>
    public class QueryBuilder
    {
        #region Fields

        /// <summary>
        /// Nom de la colonne contenant le compte total des records
        /// </summary>
        public const string OVERALLCOUNT = "OVERALLCOUNT";

        /// <summary>
        /// Pour construire la chaine select
        /// </summary>
        private StringBuilder selectPart = new StringBuilder("SELECT ");

        /// <summary>
        /// Pour construire la chaine from ou join
        /// </summary>
        private StringBuilder fromPart = new StringBuilder();

        /// <summary>
        /// Pour construire la chaine where
        /// </summary>
        private StringBuilder wherePart = new StringBuilder();

        /// <summary>
        /// Pour construire la chaine where de l'union
        /// </summary>
        private StringBuilder wherePartUnion = new StringBuilder();

        /// <summary>
        /// Pour contenir la clause order by
        /// </summary>
        private StringBuilder orderPart = new StringBuilder();

        /// <summary>
        /// Pour contenir la clause union 
        /// </summary>
        private StringBuilder unionPart = new StringBuilder();

        /// <summary>
        /// Pour construire la chaine from de l'union
        /// </summary>
        private StringBuilder fromPartUnion = new StringBuilder();

        /// <summary>
        /// Pour contenir la clause group by
        /// </summary>
        private string groupPart = null;

        /// <summary>
        /// Pagination demandée oui/non
        /// </summary>
        private bool paginate = false;

        /// <summary>
        /// Nombre d'élément dans une page
        /// </summary>
        private int pageSize = 0;

        /// <summary>
        /// Index de la page courante
        /// </summary>
        private int pageIndex = 0;

        #endregion

        #region Methodes Generales

        /// <summary>
        /// Ajout d'éléments dans la clause select.
        /// </summary>
        /// <param name="select">clause select à ajouter</param>
        /// <param name="alias">alias du champ</param>
        /// <param name="isLast">paramètre indiquant si ce champ est le dernier de la clause Select</param>
        /// <returns>l'instance courante pour chainer</returns>
        public QueryBuilder AddSelect(string select, string alias = "", bool isLast = false)
        {
            if (string.IsNullOrEmpty(alias))
            {
                this.selectPart.Append(select);
            }
            else
            {
                this.selectPart.AppendFormat("{0} AS {1}{2}", select, alias, isLast ? string.Empty : ",");
            }

            return this;
        }

        /// <summary>
        /// Ajout d'éléments dans la clause select.
        /// </summary>
        /// <param name="selectFormat">clause select sous forme format à ajouter</param>
        /// <param name="args">les paramètres à passer au format</param>
        /// <returns>l'instance courante pour chainer</returns>
        public QueryBuilder AddFormatSelect(string selectFormat, params object[] args)
        {
            this.AddSelect(string.Format(selectFormat, args));
            return this;
        }

        /// <summary>
        /// Ajout d'éléments dans la clause from
        /// </summary>
        /// <param name="from">table à ajouter</param>
        /// <param name="alias">alias de la table</param>
        /// <returns>l'instance courante pour chainer</returns>
        public QueryBuilder AddFrom(string from, string alias = "")
        {
            if (this.fromPart.Length == 0)
            {
                this.fromPart.Append(" FROM ");
            }

            this.fromPart.AppendFormat(" {0} {1}", from, alias);
            return this;
        }

        /// <summary>
        /// Ajout d'éléments dans la clause from
        /// </summary>
        /// <param name="fromFormat">table et clause join sous forme format à ajouter</param>
        /// <param name="args">les paramètres à passer au format</param>
        /// <returns>l'instance courante pour chainer</returns>
        public QueryBuilder AddFormatFrom(string fromFormat, params object[] args)
        {
            this.AddFrom(string.Format(fromFormat, args));
            return this;
        }

        /// <summary>
        /// Ajout d'un critère dans la clause where
        /// </summary>
        /// <param name="where">condition à ajouter</param>
        /// <returns>l'instance courante pour chainer</returns>
        public QueryBuilder AddWhere(string where)
        {
            if (this.wherePart.Length == 0)
            {
                this.wherePart.AppendFormat(" WHERE {0} ", where);
            }
            else
            {
                this.wherePart.AppendFormat(" AND {0} ", where);
            }

            return this;
        }

        /// <summary>
        /// Ajout d'un critère dans la clause where
        /// </summary>
        /// <param name="whereFormat">condition sous forme format à ajouter</param>
        /// <param name="args">les paramètres à passer à la condition</param>
        /// <returns>l'instance courante pour chainer</returns>*
        public QueryBuilder AddFormatWhere(string whereFormat, params object[] args)
        {
            this.AddWhere(string.Format(whereFormat, args));

            return this;
        }

        /// <summary>
        /// Ajout de la clause OrderBy avec un seul critère
        /// </summary>
        /// <param name="groupCol">la colonne de regroupement</param>
        /// <returns>l'instance courante pour chainer</returns>
        public QueryBuilder AddGroupBy(string groupCol)
        {
            this.groupPart = string.Format(" GROUP BY {0} ", groupCol);
            return this;
        }

        /// <summary>
        /// Ajout de la clause OrderBy avec un seul critère
        /// </summary>
        /// <param name="orderCol">le colonne de tri</param>
        /// <param name="orderSens">le sens du tri</param>
        /// <returns>l'instance courante pour chainer</returns>
        public QueryBuilder AddOrderBy(string orderCol, string orderSens)
        {
            if (this.orderPart.Length == 0)
            {
                this.orderPart.AppendFormat(" ORDER BY {0} {1}", orderCol, orderSens);
            }
            else
            {
                this.orderPart.AppendFormat(", {0} {1}", orderCol, orderSens);
            }

            return this;
        }

        /// <summary>
        /// Ajout de la clause GroupBy avec multiples critères
        /// </summary>
        /// <param name="groupCol">Tableau de colonnes de regroupement</param>
        /// <returns>l'instance courante pour chainer</returns>
        public QueryBuilder AddMultipleGroupBy(string[] groupCol)
        {
            // pour conserver une liste de critère unique
            Dictionary<string, string> group = new Dictionary<string, string>();

            for (int i = 0; i < groupCol.Length; i++)
            {
                // on s'assure que l'on ajoute une seule fois le critère de tri
                if (!group.ContainsKey(groupCol[i]))
                {
                    group.Add(groupCol[i], groupCol[i]);
                }
            }

            this.groupPart = " GROUP BY " + string.Join(",", group.Values);

            return this;
        }

        /// <summary>
        /// Ajout de la clause OrderBy avec multiples critères
        /// </summary>
        /// <param name="orderCol">Tableau de colonnes de tri</param>
        /// <param name="orderSens">Tableau du sens des tris</param>
        /// <returns>l'instance courante pour chainer</returns>
        public QueryBuilder AddMultipleOrderBy(string[] orderCol, string[] orderSens)
        {
            // pour conserver une liste de critère unique
            Dictionary<string, string> order = new Dictionary<string, string>();

            for (int i = 0; i < orderCol.Length; i++)
            {
                // on s'assure que l'on ajoute une seule fois le critère de tri
                if (!order.ContainsKey(orderCol[i]))
                {
                    order.Add(orderCol[i], string.Format("{0} {1}", orderCol[i], orderSens[i]));
                }
            }

            if (this.orderPart.Length == 0)
            {
                this.orderPart.AppendFormat(" ORDER BY " + string.Join(",", order.Values));
            }
            else
            {
                this.orderPart.AppendFormat(", " + string.Join(",", order.Values));
            }

            return this;
        }

        /// <summary>
        /// Pour obtenir la chaine à exécuter
        /// </summary>
        /// <returns>la chaine Sql obtenue</returns>
        public string ToSql()
        {
            StringBuilder builder = new StringBuilder();

            if (this.selectPart.Length == 0)
            {
                throw new Exception("the select part is not defined");
            }

            // select
            builder.Append(this.selectPart.ToString());

            if (this.paginate)
            {
                // ajout du total lignes dans select
                builder.Append(", OVERALLCOUNT = COUNT(*) OVER() ");
            }

            // from
            if (this.fromPart.Length > 0)
            {
                builder.Append(this.fromPart.ToString());
            }

            // concat where
            if (this.wherePart.Length > 0)
            {
                builder.Append(this.wherePart.ToString());
            }

            // concat group by
            if (!string.IsNullOrEmpty(this.groupPart))
            {
                builder.Append(this.groupPart.ToString());
            }

            // concat order by
            if (this.orderPart.Length > 0)
            {
                if (this.paginate)
                {
                    builder.AppendFormat(" {0} OFFSET ({1}-1)*{2} ROWS FETCH NEXT {2} ROWS ONLY ", this.orderPart.ToString(), this.pageIndex, this.pageSize);
                }
                else
                {
                    builder.Append(this.orderPart.ToString());
                }
            }

            return builder.ToString();
        }

        #endregion

        #region Methodes Pagination SqlServer 2012

        /// <summary>
        /// Ajout de la gestion de la pagination. Une clause OrderBy est obligatoire.
        /// Sous sql server 2012 on peut utiliser la requête suivante qui permet
        /// - de paginer simplement
        /// - de récupérer le nombre total d'enregistrement
        /// <code>
        ///    DECLARE 
        ///      @PageSize INT = 10, 
        ///      @PageNum  INT = 1;
        ///    SELECT 
        ///      name, object_id, 
        ///      overall_count = COUNT(*) OVER()
        ///    FROM sys.all_objects
        ///    ORDER BY name
        ///      OFFSET (@PageNum-1)*@PageSize ROWS
        ///      FETCH NEXT @PageSize ROWS ONLY;
        /// </code>
        /// </summary>
        /// <param name="pageIndex">l'index de la page courante</param>
        /// <param name="pageSize">le nombre d'item par page</param>
        /// <param name="orderCol">le colonne de tri</param>
        /// <param name="orderSens">le sens du tri</param>
        public void AddPaginate(int pageIndex, int pageSize, string orderCol, string orderSens)
        {
            if (string.IsNullOrEmpty(orderCol))
            {
                throw new ArgumentNullException("orderPart param is mandatory for paging");
            }

            this.AddOrderBy(orderCol, orderSens);

            // pas de pagination si le nombre d'item par page = 0
            if (pageSize == 0)
            {
                return;
            }

            this.paginate = true;
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
        }

        /// <summary>
        /// Ajout de la gestion de la pagination. Une clause OrderBy est obligatoire.
        /// Gestion de critère de tri multiple
        /// </summary>
        /// <param name="pageIndex">l'index de la page courante</param>
        /// <param name="pageSize">le nombre d'item par page</param>
        /// <param name="orderCol">Tableau de colonnes de tri</param>
        /// <param name="orderSens">Tableau du sens des tris</param>
        public void AddPaginate(int pageIndex, int pageSize, string[] orderCol, string[] orderSens)
        {
            if (orderCol == null || orderCol.Length == 0)
            {
                throw new ArgumentNullException("orderCol param is mandatory for paging");
            }

            if (orderSens == null || orderSens.Length == 0)
            {
                throw new ArgumentNullException("orderSens param is mandatory for paging");
            }

            this.AddMultipleOrderBy(orderCol, orderSens);

            // pas de pagination si le nombre d'item par page = 0
            if (pageSize == 0)
            {
                return;
            }

            this.paginate = true;
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
        }

        #endregion
    }
}