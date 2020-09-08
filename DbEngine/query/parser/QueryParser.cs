#region Usings
using System.Linq;
using System.Collections.Generic;
using DbEngine.helper;
#endregion

#region Namespace
namespace DbEngine.Query.Parser
{
    #region Class
    /// <summary>
    /// Class containing logic to parse and fetch different properties of the input query
    /// </summary>
    public class QueryParser
    {
        #region Fields
        /// <summary>
        /// property containing different properties of the input query
        /// </summary>
        private QueryParameter queryParameter;
        #endregion

        #region Properties
        /// <summary>
        /// Getter function for the field queryparameter
        /// </summary>
        public QueryParameter QueryParameter { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor where query parameter property gets initialised
        /// </summary>
        public QueryParser()
        {
            queryParameter = new QueryParameter();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Method to set the properties to queryparameter
        /// </summary>
        /// <param name="queryString">Input query</param>
        private void SetQueryParameterValues(string queryString)
        {
            queryParameter.QueryString = queryString;
            queryParameter.File = QueryHelper.GetFileNameFromQuery(queryString, true);
            queryParameter.Fields = GetFields();
            queryParameter.Restrictions = GetRestrictions();
            queryParameter.LogicalOperators = GetLogicalOperators();
            queryParameter.AggregateFunctions = GetAggregateFunctions();
            queryParameter.GroupByFields = GetGroupByFields();
            queryParameter.OrderByFields = GetOrderByFields();
            queryParameter.QueryType = GetQueryType();
        }

        /// <summary>
        /// Method to find and assign the selected fields property of the queryparameter
        /// </summary>
        /// <returns>List of selected fields if any else null</returns>
        private List<string> GetFields()
        {
            return QueryHelper.GetSelectedFields(queryParameter?.QueryString);
        }

        /// <summary>
        /// Method to find and assign the conditions property of the query parameter
        /// </summary>
        /// <returns>Returns list of restrictions in the query if any else null</returns>
        private List<Restriction> GetRestrictions()
        {
            List<string> restrictionsList = QueryHelper.GetConditionInFilter(queryParameter.QueryString);

            if (restrictionsList != null &&
               !string.Equals(restrictionsList.First(), Common.NoFilterString))
            {
                List<Restriction> filters = new List<Restriction>();
                foreach (string restriction in restrictionsList)
                {
                    List<string> parts = Common.SplitConditionWords(restriction);
                    if (parts != null && parts.Count == 3)
                    {
                        filters.Add(new Restriction(propertyName: parts[0],
                                    propertyValue: parts[2].Trim('\''), condition: parts[1]));
                    }
                }
                if (filters.Any())
                {
                    return filters;
                }
            }
            return null;
        }

        /// <summary>
        /// Method to find and assign the logical operators of the queryparameter
        /// </summary>
        /// <returns>List of logical operators used in the query if any else null</returns>
        private List<string> GetLogicalOperators()
        {
            List<string> logics = QueryHelper.GetLogicalOperators(queryParameter?.QueryString);
            if (logics != null && !string.Equals(logics.First(), Common.NoLogicalOperatorsString))
            {
                return logics;
            }
            return null;
        }

        /// <summary>
        /// Method to find and assign the aggregate funciton property of query parameter
        /// </summary>
        /// <returns>Lis tof aggregate functions used in query if any else null</returns>
        private List<AggregateFunction> GetAggregateFunctions()
        {
            List<string> aggregateStrings = QueryHelper.GetAggregateFunctions(queryParameter?.QueryString);
            if (aggregateStrings != null &&
                !string.Equals(aggregateStrings.First(), Common.NoAggregateFunctions))
            {
                List<AggregateFunction> aggregates = new List<AggregateFunction>();
                foreach (string aggregate in aggregateStrings)
                {
                    List<string> parts = Common.SplitAggregateFields(aggregate);
                    if (parts != null && parts.Count == 2)
                    {
                        aggregates.Add(new AggregateFunction(field: parts[0], function: parts[1]));
                    }
                }
                if (aggregates.Any())
                {
                    return aggregates;
                }
            }
            return null;
        }

        /// <summary>
        /// Method to find and assign the order by field property of the query parameter
        /// </summary>
        /// <returns>List of fields used in order by clause of the query if any else null</returns>
        private List<string> GetOrderByFields()
        {
            List<string> orders = QueryHelper.GetOrderField(queryParameter?.QueryString);
            if (orders != null && !string.Equals(orders.First(), Common.NoOrderByClause) &&
                !string.Equals(orders.First(), Common.NoBaseOrderByClause))
            {
                return orders;
            }
            return null;
        }

        /// <summary>
        /// Method to find and assign the group by field property of query parameter
        /// </summary>
        /// <returns>List of fields used in group by clause of the query if any else null</returns>
        private List<string> GetGroupByFields()
        {
            List<string> groups = QueryHelper.GetGroupByField(queryParameter?.QueryString);
            if (groups != null && !string.Equals(groups.First(), Common.NoGroupByClause) &&
                !string.Equals(groups.First(), Common.NoBaseGroupByClause))
            {
                return groups;
            }
            return null;
        }

        /// <summary>
        /// Method to assing the query type property of the query paremeter
        /// </summary>
        /// <returns>The query type</returns>
        private string GetQueryType()
        {
            if (queryParameter != null)
            {
                if (queryParameter.GroupByFields != null && queryParameter.GroupByFields.Any())
                {
                    return "GROUP_BY";
                }
                else if (queryParameter.OrderByFields != null && queryParameter.OrderByFields.Any())
                {
                    return "ORDER_BY";
                }
                else if (queryParameter.AggregateFunctions != null && queryParameter.AggregateFunctions.Any())
                {
                    return "AGGREGATE";
                }
                else
                {
                    return "SIMPLE_QUERY";
                }
            }
            return null;
        }
        #endregion

        #region  Public methods
        /// <summary>
        /// Method to parse the query and assign fields of query parameter property
        /// </summary>
        /// <param name="queryString">Input query</param>
        /// <returns>Returns the query parameter property containing details of the query</returns>
        public QueryParameter ParseQuery(string queryString)
        {
            if (Common.IsValidQueryBasic(queryString))
            {
                if (queryParameter == null)
                {
                    queryParameter = new QueryParameter();
                }
            }

            SetQueryParameterValues(queryString);
            return queryParameter;
        } 
        #endregion

    } 
    #endregion
} 
#endregion