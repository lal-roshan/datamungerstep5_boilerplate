#region Usings
using System;
using System.Collections.Generic; 
#endregion

#region Namespace
namespace DbEngine.Query.Parser
{
    #region Class
    /// <summary>
    /// Class that contains different properties of a query
    /// </summary>
    public class QueryParameter
    {
        /// <summary>
        /// The input query string
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// List of conditions in where clause
        /// </summary>
        public List<Restriction> Restrictions { get; set; }

        /// <summary>
        /// List of logical operators(and, or and not) used in the query
        /// </summary>
        public List<String> LogicalOperators { get; set; }

        /// <summary>
        /// List of aggregate funtions that are used in the query
        /// </summary>
        public List<AggregateFunction> AggregateFunctions { get; set; }

        /// <summary>
        /// The file name in the qquery from which the data has to be fetched
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// The base part of the input query
        /// </summary>
        public string BaseQuery { get; set; }

        /// <summary>
        /// List of selected fields in the query
        /// </summary>
        public List<String> Fields { get; set; }

        /// <summary>
        /// List of fields that are used in the group by clause of the query
        /// </summary>
        public List<String> GroupByFields { get; set; }

        /// <summary>
        /// List of fields that are used in the order by clause in the query
        /// </summary>
        public List<String> OrderByFields { get; set; }
        
        /// <summary>
        /// Represents the type of the query which may be simple, group by , order by or aggregate
        /// </summary>
        public string QueryType { get; set; } = "SIMPLE_QUERY";
    }
    #endregion
} 
#endregion