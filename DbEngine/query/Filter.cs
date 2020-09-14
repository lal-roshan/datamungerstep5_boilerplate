using DbEngine.helper;
using DbEngine.Query.Parser;
using DbEngine.Reader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbEngine.Query
{
    /// <summary>
    /// Class containing methods to evaluating expressions in the query
    /// </summary>
    public class Filter
    {
        #region Properties
        /// <summary>
        /// String representing the file name
        /// </summary>
        private static string _file;

        /// <summary>
        /// List of all column headers in the file
        /// </summary>
        private static List<string> _headers;

        /// <summary>
        /// List of data types of all columns in the file
        /// </summary>
        private static List<string> _dataTypes;

        /// <summary>
        /// Object of query processor for accessing different properties of query
        /// </summary>
        private static CsvQueryProcessor _queryProcessor; 
        #endregion

        #region Private Methods
        /// <summary>
        /// Method to evaluate '=' conditions
        /// </summary>
        /// <param name="exp1">First value</param>
        /// <param name="exp2">Second value</param>
        /// <param name="dataType">Type of the values</param>
        /// <returns>true if values are equal else false</returns>
        private static bool EqualToComparer(string exp1, string exp2, string dataType)
        {
            try
            {
                switch (dataType)
                {
                    case "System.Int32":
                        return Convert.ToInt32(exp1) == Convert.ToInt32(exp2);
                    case "System.Double":
                        return Convert.ToDouble(exp1) == Convert.ToDouble(exp2);
                    case "System.DateTime":
                        return Convert.ToDateTime(exp1) == Convert.ToDateTime(exp2);
                    case "System.String":
                        return string.Equals(exp1, exp2, StringComparison.InvariantCultureIgnoreCase);
                    default:
                        return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to perform '!=' operation
        /// </summary>
        /// <param name="exp1">First value</param>
        /// <param name="exp2">Second value</param>
        /// <param name="dataType">Type of the values</param>
        /// <returns>true if the values are not equal else false</returns>
        private static bool NotEqualToComparer(string exp1, string exp2, string dataType)
        {
            try
            {
                switch (dataType)
                {
                    case "System.Int32":
                        return Convert.ToInt32(exp1) != Convert.ToInt32(exp2);
                    case "System.Double":
                        return Convert.ToDouble(exp1) != Convert.ToDouble(exp2);
                    case "System.DateTime":
                        return Convert.ToDateTime(exp1) != Convert.ToDateTime(exp2);
                    case "System.String":
                        return !string.Equals(exp1, exp2, StringComparison.InvariantCultureIgnoreCase);
                    default:
                        return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to perform '<' operation
        /// </summary>
        /// <param name="exp1">First value</param>
        /// <param name="exp2">Second value</param>
        /// <param name="dataType">Type of the values</param>
        /// <returns>true if the first value is less than second value else false</returns>
        private static bool LessThanComparer(string exp1, string exp2, string dataType)
        {
            try
            {
                switch (dataType)
                {
                    case "System.Int32":
                        return Convert.ToInt32(exp1) < Convert.ToInt32(exp2);
                    case "System.Double":
                        return Convert.ToDouble(exp1) < Convert.ToDouble(exp2);
                    case "System.DateTime":
                        return Convert.ToDateTime(exp1) < Convert.ToDateTime(exp2);
                    default:
                        return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to perform '<=' operation
        /// </summary>
        /// <param name="exp1">First value</param>
        /// <param name="exp2">Second value</param>
        /// <param name="dataType">Type of the values</param>
        /// <returns>true if the first value is less than or equal to second value else false</returns>
        private static bool LessThanOrEqualComparer(string exp1, string exp2, string dataType)
        {
            try
            {
                switch (dataType)
                {
                    case "System.Int32":
                        return Convert.ToInt32(exp1) <= Convert.ToInt32(exp2);
                    case "System.Double":
                        return Convert.ToDouble(exp1) <= Convert.ToDouble(exp2);
                    case "System.DateTime":
                        return Convert.ToDateTime(exp1) <= Convert.ToDateTime(exp2);
                    default:
                        return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to perform '>' operation
        /// </summary>
        /// <param name="exp1">First value</param>
        /// <param name="exp2">Second value</param>
        /// <param name="dataType">Type of the values</param>
        /// <returns>true if the first value is greater than the second value else false</returns>
        private static bool GreaterThanComparer(string exp1, string exp2, string dataType)
        {
            try
            {
                switch (dataType)
                {
                    case "System.Int32":
                        return Convert.ToInt32(exp1) > Convert.ToInt32(exp2);
                    case "System.Double":
                        return Convert.ToDouble(exp1) > Convert.ToDouble(exp2);
                    case "System.DateTime":
                        return Convert.ToDateTime(exp1) > Convert.ToDateTime(exp2);
                    default:
                        return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to perform '>=' operation
        /// </summary>
        /// <param name="exp1">First value</param>
        /// <param name="exp2">Second value</param>
        /// <param name="dataType">Type of the values</param>
        /// <returns>true if the first value is greater than or equal to the second value else false</returns>
        private static bool GreaterThanOrEqualComparer(string exp1, string exp2, string dataType)
        {
            try
            {
                switch (dataType)
                {
                    case "System.Int32":
                        return Convert.ToInt32(exp1) >= Convert.ToInt32(exp2);
                    case "System.Double":
                        return Convert.ToDouble(exp1) >= Convert.ToDouble(exp2);
                    case "System.DateTime":
                        return Convert.ToDateTime(exp1) >= Convert.ToDateTime(exp2);
                    default:
                        return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to call the required comparison function based on the condition operator
        /// </summary>
        /// <param name="exp1">First value</param>
        /// <param name="exp2">Second value</param>
        /// <param name="dataType">Data type of the values</param>
        /// <param name="condition">The condition operator</param>
        /// <returns>True if condition satisfies else false</returns>
        private static bool PerformEvaluation(string exp1, string exp2, string dataType, string condition)
        {
            switch (condition)
            {
                case "=":
                    return EqualToComparer(exp1, exp2, dataType);
                case "!=":
                    return NotEqualToComparer(exp1, exp2, dataType);
                case "<":
                    return LessThanComparer(exp1, exp2, dataType);
                case "<=":
                    return LessThanOrEqualComparer(exp1, exp2, dataType);
                case ">":
                    return GreaterThanComparer(exp1, exp2, dataType);
                case ">=":
                    return GreaterThanOrEqualComparer(exp1, exp2, dataType);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to perform logical operations
        /// </summary>
        /// <param name="val1">First value</param>
        /// <param name="val2">Second value</param>
        /// <param name="operation">The logical operation</param>
        /// <returns>True if the result of the logical operation is true else false</returns>
        private static bool PerformLogicalOperation(bool val1, bool? val2, string operation)
        {
            switch (operation.ToUpper())
            {
                case "AND":
                    return val1 && (val2 ?? false);
                case "OR":
                    return val1 || (val2 ?? false);
                case "NOT":
                    return !val1;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to perform all occurences of a logical operator in the filter part
        /// </summary>
        /// <param name="operation">The logical operation</param>
        /// <param name="conditions">The conditions for operation to operate on</param>
        /// <param name="operators">The list of all operators in filter</param>
        private static void PerformSpecificLogicalOperations(string operation, List<bool> conditions, List<string> operators)
        {
            int logicalIndex = Common.GetIndexFromList(operation, operators);
            while (logicalIndex > -1)
            {
                conditions[logicalIndex] = PerformLogicalOperation(conditions[logicalIndex],
                    conditions[logicalIndex + 1], operation);
                conditions.RemoveAt(logicalIndex + 1);
                operators.RemoveAt(logicalIndex);
                logicalIndex = Common.GetIndexFromList(operation, operators);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to evaluate all the conditions in the query
        /// </summary>
        /// <param name="queryParameter">The query parameter containing properties of the query</param>
        /// <param name="rowValues">The field values of a row</param>
        /// <returns>True or false based on whether the condition saitisfies or fails</returns>
        public static bool EvaluateExpression(QueryParameter queryParameter, List<string> rowValues)
        {
            ///If the file name is different than previous file name then recalculate all the properties related to it
            if (!string.Equals(_file, queryParameter.File))
        
            {
                _file = queryParameter.File;
                _queryProcessor = new CsvQueryProcessor(queryParameter.File);
                _headers = _queryProcessor.GetHeader().Headers.ToList();
                _dataTypes = _queryProcessor.GetColumnType().DataTypes.ToList();
            }

            ///Find the column positions that are being referred by the where clause
            List<int> fieldPositions = queryParameter.Restrictions
                .Select(restriction => _headers.IndexOf(restriction.propertyname)).ToList();

            ///List containing the result of each conditions
            List<bool> conditions = new List<bool>();
            ///Find the result of each result
            for (int i = 0; i < queryParameter.Restrictions.Count; i++)
            {
                conditions.Add(PerformEvaluation(rowValues[fieldPositions[i]],
                                        queryParameter.Restrictions[i].propertyValue,
                                        _dataTypes[fieldPositions[i]], queryParameter.Restrictions[i].condition));


            }

            ///After finding the result of each condition logical operators should be applied if any
            if (queryParameter.LogicalOperators != null)
            {
                List<string> logicalOperators = queryParameter.LogicalOperators.ToList();
                ///Execute all and conditions first due to priority
                PerformSpecificLogicalOperations("and", conditions, logicalOperators);

                ///Execute or conditions after and
                PerformSpecificLogicalOperations("or", conditions, logicalOperators);

            }
            ///The last value in the condition bool list will be the resulting bool
            return conditions.Last();
        } 
        #endregion

    }
}
