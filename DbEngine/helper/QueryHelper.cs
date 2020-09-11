using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DbEngine.helper
{    public class QueryHelper
    {
        #region GetRoot()
        /// <summary>
        /// Method to get the root directory of the project
        /// </summary>
        /// <returns>Root directory</returns>
        public static string GetRoot()
        {
            var mydirectory = Environment.CurrentDirectory;
            int indexofmyvar = mydirectory.IndexOf("bin");
            /// index -1 for removing the folder seperator character
            string substr = mydirectory.Substring(0, indexofmyvar - 1);
            string projectRoot = Directory.GetParent(substr).ToString();
            if (Directory.Exists(projectRoot))
            {
                return projectRoot + "\\";
            }
            return "";
        }

        #endregion

        #region Split into words
        /// <summary>
        /// Method to split queries into list of words in it
        /// </summary>
        /// <param name="queryString">input string</param>
        /// <returns>List of words in query or null</returns>
        public static List<string> SplitQueryWords(string queryString)
        {
            List<string> queryResult;
            if (!Common.IsValidQueryBasic(queryString))
            {
                queryResult = null;
            }
            else
            {
                queryResult = queryString.Split(' ').ToList();
            }
            return queryResult;
        }
        #endregion

        #region Get File Name
        /// <summary>
        /// Method to get the file name from input query
        /// </summary>
        /// <param name="queryString">The string containing file name</param>
        /// <param name="withDirectory">bool representing whether or not to append root directory if not present</param>
        /// <returns>File name or null</returns>
        public static string GetFileNameFromQuery(string queryString, bool withDirectory = false)
        {
            if (Common.IsValidQueryBasic(queryString))
            {
                queryString = queryString.Split(' ').FirstOrDefault(part => part.EndsWith(".csv"));
                if (!string.IsNullOrEmpty(queryString))
                {
                    /*
                     ****Commenting this as appending root path made test cases in hobbes fail
                     ***Whereas for getting values by reading input from user either user will have to enter full
                     *** directory path or we have to add root like this.
                     
                    //if (Common.GetStringIndex(queryString, GetRoot()) < 0 && withDirectory)
                    //{
                    //    queryString = GetRoot() + queryString;
                    //}
                    */
                    return queryString;
                }
            }
            return null; ;
        }
        #endregion

        #region Base Part
        /// <summary>
        /// Method to get the base part of the wuery
        /// </summary>
        /// <param name="queryString">input query</param>
        /// <returns>Base part of string or null</returns>
        public static string GetBasePartFromQuery(string queryString)
        {
            if (Common.IsValidQueryBasic(queryString))
            {
                int index = -1;

                int whereIndex = Common.GetStringIndex(queryString, "where", Common.Index.Last);
                if (whereIndex > -1)
                {
                    index = whereIndex;
                }
                else
                {
                    int groupIndex = Common.GetStringIndex(queryString, "group by", Common.Index.Last);
                    if (groupIndex > -1)
                    {
                        index = groupIndex;
                    }
                    else
                    {
                        int orderIndex = Common.GetStringIndex(queryString, "order by", Common.Index.Last);
                        if (orderIndex > -1)
                        {
                            index = orderIndex;
                        }
                    }
                }
                ///if the query contains either of where, order by or group by clauses
                ///
                if (index > -1)
                {
                    queryString = queryString.Substring(0, index).Trim();

                    if ((Common.StringMatchCount(queryString, "where") > 0 ||
                       Common.StringMatchCount(queryString, "order by") > 0 ||
                       Common.StringMatchCount(queryString, "group by") > 0) &&
                       (!queryString.Contains('(') && !queryString.Contains(')')))
                    {
                        return null;
                    }
                    else
                    {
                        return queryString; ;
                    }
                }
                else
                {
                    return queryString.Trim();
                }
            }
            return null;
        }
        #endregion

        #region Get Selected Fields
        /// <summary>
        /// Method to get selected fields forma input query
        /// </summary>
        /// <param name="queryString">input string</param>
        /// <returns>List of fields selected or null</returns>
        public static List<string> GetSelectedFields(string queryString)
        {
            List<string> queryResult = new List<string>();
            if (Common.IsValidQueryBasic(queryString))
            {
                ///Look whether the query starts with select and contains from
                int selectIndex = Common.GetStringIndex(queryString, "select");
                int fromIndex = Common.GetStringIndex(queryString, "from", Common.Index.Last);

                if ((selectIndex == 0) && (fromIndex > -1))
                {
                    ///select the part of query starting from select upto first from
                    ///(even if there is no from in query it will be handled later)
                    queryString = queryString.Trim().Substring(selectIndex + 6, fromIndex - 6);

                    ///if select keyword is found at first position and if there is a from in the query and
                    ///if there are no other select or from keyword between the first select and from
                    if (Common.StringMatchCount(queryString, "select") == Common.StringMatchCount(queryString, "from"))
                    {
                        ///Get the strings between the select and from keyword as a list
                        queryResult = queryString.Split(',').Select(part => part.Trim()).ToList();

                        ///Checks whether there are any selected field ending with ',' and
                        ///whether there are any selected field keyword with space 
                        ///(a part that has space will be an assignment)
                        if (queryResult.Any(part => string.IsNullOrEmpty(part) ||
                            (part.Contains(' ') && !part.Contains('='))))
                        {
                            queryResult = null;
                        }
                    }
                    else
                    {
                        queryResult = null;
                    }
                }
                else
                {
                    queryResult = null;
                }
            }
            else
            {
                queryResult = null;
            }
            return queryResult;
        }
        #endregion

        #region Filter part
        /// <summary>
        /// Method to get filer part form query
        /// </summary>
        /// <param name="queryString">input string</param>
        /// <returns></returns>
        public static string GetFilterPart(string queryString)
        {
            string queryResult = "";
            if (Common.IsValidQueryBasic(queryString))
            {
                /// get index after four letters of where word (5 as index starts from 0)
                int whereIndex = Common.GetStringIndex(queryString, "where", Common.Index.Last) + 5;

                ///a valid query will have a where clause minimum at 16th position.Eg: select * from t 
                if (whereIndex > 15)
                {
                    int endIndex = -1;
                    int orderIndex = Common.GetStringIndex(queryString, "order by", Common.Index.Last);
                    int groupIndex = Common.GetStringIndex(queryString, "group by", Common.Index.Last);

                    ///get the index of whichever keyword that comes first 'order by' | 'group by'
                    if ((orderIndex > groupIndex && groupIndex == -1))
                    {
                        endIndex = orderIndex;
                    }
                    else
                    {
                        endIndex = groupIndex;
                    }

                    ///if the query contains both order by , group by clause and order by comes before group by
                    ///and if order by is not part of any subquery in that case the query is wrong as 
                    ///ORDER BY clause should be the last part of query
                    if (orderIndex > -1 && groupIndex > -1 &&
                        groupIndex > orderIndex && !queryString.Substring(orderIndex, groupIndex - orderIndex).Contains(")"))
                    {
                        queryResult = null;
                    }
                    ///if 'order by' or 'group by' comes before 'where' keyword
                    ///either it means it has a subquery or the query is wrong
                    else if (endIndex > whereIndex)
                    {
                        ///Getting the part of query between 'where' and 'orderby' or 'group by' clause
                        queryString = queryString.Substring(whereIndex, endIndex - whereIndex).Trim();

                        ///if there is a bracket it means where keyword is inside a subquery and
                        ///outer query only has order by which means there are no filters
                        if (queryString.Contains(")") ||
                            queryString.Length == 0)
                        {
                            queryResult = Common.NoFilterString;
                        }
                        else
                        {
                            queryString = Common.StringReplace(queryString, "where").Trim();
                            /// minimum length for a filter without white space is 3 and
                            /// with white space is 5 Eg: 1=3,1 = 3
                            if ((queryString.Length < 5 && queryString.Contains(" ")) ||
                                (queryString.Length < 3 && !queryString.Contains(" ")))
                            {
                                queryResult = null;
                            }
                            else
                            {
                                queryResult = queryString;
                            }
                        }
                    }
                    ///if 'order by' or 'group by' comes before 'where' keyword
                    ///either it means it has a subquery or the query is wrong
                    ///if it has a subquery there must be more than one 'select' keyword or else it is wrong
                    else if (endIndex == -1 ||
                            Common.StringMatchCount(queryString.Substring(0, endIndex), "select") > 1)
                    {
                        queryResult = queryString.Substring(whereIndex);
                    }
                    ///the query is wrong
                    else
                    {
                        queryResult = null;
                    }
                }
                ///If query doesnt have a where clause
                ///(checking with 4 as we added 5 to the index of where
                ///and hence if it was -1 by adding 5 to it it becomes 4
                else if (whereIndex == 4)
                {
                    queryResult = Common.NoFilterString;
                }
                else
                {
                    queryResult = null;
                }
            }
            else
            {
                queryResult = null;
            }
            return queryResult;
        }
        #endregion

        #region Conditions filter
        /// <summary>
        /// Method to get the conditions in the filter part of the query
        /// </summary>
        /// <param name="queryString">input query</param>
        /// <returns></returns>
        public static List<string> GetConditionInFilter(string queryString)
        {
            List<string> queryResult = new List<string>();

            ///Get the filter part of the query
            string queryFilter = GetFilterPart(queryString);

            ///if the query is not valid
            if (string.IsNullOrEmpty(queryFilter))
            {
                queryResult = null;
            }
            ///if there is no filter part in the query
            else if (string.Equals(queryFilter, Common.NoFilterString))
            {
                queryResult.Add(Common.NoFilterString);
            }
            ///find the conditions in filter part
            else
            {
                ///Split the filter part of the query with 'and' and 'or' keywords and
                ///remove those keywords from resultant list
                queryResult = Common.SplitByString(queryFilter, "and, or",
                              Common.SplitType.RemoveThis);

                ///checking whether the conditions in filter part are valid
                if (queryResult != null &&
                    queryResult.Any(x => Common.SplitConditionWords(x) == null))
                {
                    queryResult = null;
                }
            }
            return queryResult;
        }
        #endregion

        #region Logical Operators
        /// <summary>
        /// Method to get the logical operators used in filter part
        /// </summary>
        /// <param name="queryString">Input query/param>
        /// <returns>List of all logical operators used in the query</returns>
        public static List<string> GetLogicalOperators(string queryString)
        {
            List<string> queryResult = new List<string>();
            if (Common.IsValidQueryBasic(queryString))
            {
                string filterPart = GetFilterPart(queryString);
                if (string.IsNullOrEmpty(filterPart))
                {
                    queryResult = null;
                }
                else if (string.Equals(filterPart, Common.NoFilterString))
                {
                    queryResult.Add(filterPart);
                }
                else
                {
                    ///Splits the filter part based on operators and removes all other part
                    queryResult = Common.SplitByString(filterPart, "and, or, not",
                                  Common.SplitType.RemoveAllButThis);
                    if (queryResult != null && queryResult.Count == 0)
                    {
                        queryResult.Add(Common.NoLogicalOperatorsString);
                    }
                }
            }
            else
            {
                queryResult = null;
            }
            return queryResult;
        }
        #endregion

        #region Get Order field
        /// <summary>
        /// Method to get fields used in order clause
        /// </summary>
        /// <param name="queryString">input query</param>
        /// <returns>returns list of fields used in order clause if any
        /// or message if there is no order by clause or null if query is invalid</returns>
        public static List<string> GetOrderField(string queryString)
        {
            List<string> queryResult = new List<string>();
            if (Common.IsValidQueryBasic(queryString))
            {
                queryResult = Common.SplitByString(queryString, "order by");
                ///split result with count 1 means there was nt order by clause in the query
                if (queryResult?.Count == 1)
                {
                    queryResult.Clear();
                    queryResult.Add(Common.NoOrderByClause);
                }
                else if (queryResult?.Count > 2)
                {
                    ///Check whether second last part is order by, if yes then last part will be the order by field
                    ///for valid queries
                    if (string.Equals(queryResult[queryResult.Count - 2], "order by",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        ///if the part after order by contains where or group by and if they are not
                        ///in substring , then query is invalid
                        if (Common.StringMatchCount(queryResult.Last(), "where") > 0 ||
                            Common.StringMatchCount(queryResult.Last(), "group by") > 0)
                        {
                            if (Common.isPartOfSubQuery(queryResult.Last()))
                            {
                                queryResult.Clear();
                                queryResult.Add(Common.NoBaseOrderByClause);
                            }
                            else
                            {
                                queryResult = null;
                            }
                        }
                        ///if last part is part of subquery
                        else if (Common.isPartOfSubQuery(queryResult.Last()) &&
                                !queryResult.Last().Contains('('))
                        {
                            queryResult.Clear();
                            queryResult.Add(Common.NoBaseOrderByClause);
                        }
                        ///else the last part will be the order by field
                        else
                        {
                            queryResult = queryResult.Last().Split(',').Select(x => x.Trim()).ToList();
                        }
                    }
                    ///If second last part of query is not order by it won't be in base query
                    else
                    {
                        queryResult.Clear();
                        queryResult.Add(Common.NoBaseOrderByClause);
                    }
                }
                else
                {
                    queryResult = null;
                }
            }
            else
            {
                queryResult = null;
            }
            return queryResult;
        }
        #endregion

        #region Get Group Field
        /// <summary>
        /// Method to get group by fields from a query
        /// </summary>
        /// <param name="queryString">Input query</param>
        /// <returns>returns list of fields used in group by clause if any
        /// or message if there is no order by clause or null if query is invalid</returns>
        public static List<string> GetGroupByField(string queryString)
        {
            List<string> queryResult = new List<string>();
            if (Common.IsValidQueryBasic(queryString))
            {
                ///Split the query based on group by clause
                queryResult = Common.SplitByString(queryString, "group by");

                ///If the split result count is one, it means there is no group by clause in it
                if (queryResult?.Count == 1)
                {
                    queryResult.Clear();
                    queryResult.Add(Common.NoGroupByClause);
                }
                else if (queryResult?.Count > 2)
                {
                    ///If the second last item in list is group by, for valid query the last item
                    ///in the list will be the group by fields
                    if (string.Equals(queryResult[queryResult.Count - 2], "group by",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        /// checks whether the group by part contains any where clause
                        if (Common.StringMatchCount(queryResult.Last(), "where") > 0)
                        {
                            ///If last part contains where and ')' it means the only group by in the string is
                            ///a part of substring and hence no group by in base query
                            if (Common.isPartOfSubQuery(queryResult.Last()))
                            {
                                queryResult.Clear();
                                queryResult.Add(Common.NoBaseGroupByClause);
                            }
                            ///If group by isnt a part of subquery and where clause is present at last part, then the query is invalid
                            else
                            {
                                queryResult = null;
                            }
                        }
                        ///Checks whether the lsat part contains a ')' but no '(', implying that the group by is in a substring
                        else if (Common.isPartOfSubQuery(queryResult.Last()) &&
                                !queryResult.Last().Contains('('))
                        {
                            queryResult.Clear();
                            queryResult.Add(Common.NoBaseGroupByClause);
                        }
                        else
                        {
                            ///If there is a order by clause after group by then split the string at order by
                            ///so that we get the group by field only
                            string endPart = Common.SplitByString(queryResult.Last(), "order by")?.First();
                            if (!string.IsNullOrEmpty(endPart))
                            {
                                if (char.Equals(endPart.Last(), ')'))
                                {
                                    endPart = endPart.Trim(')');
                                }
                                ///Get all the coma separated group by fields
                                queryResult = endPart.Split(',').Select(x => x.Trim()).ToList();
                            }
                        }
                    }
                    else if (queryResult != null)
                    {
                        queryResult.Clear();
                        queryResult.Add(Common.NoBaseGroupByClause);
                    }
                }
                else
                {
                    queryResult = null;
                }
            }
            else
            {
                queryResult = null;
            }
            return queryResult;
        }
        #endregion

        #region Get Aggregate Functions
        /// <summary>
        /// Method to get the aggregate functions from query
        /// </summary>
        /// <param name="queryString">Input query</param>
        /// <returns>List of aggregate functions in query</returns>
        public static List<string> GetAggregateFunctions(string queryString)
        {
            List<string> queryResult = new List<string>();
            if (Common.IsValidQueryBasic(queryString))
            {
                ///Get the base part of query as the aggregate functions on selected fields are to be found
                string basePart = GetBasePartFromQuery(queryString);
                if (!string.IsNullOrEmpty(basePart))
                {
                    ///Create coma seperated pattern string for each aggregate function
                    StringBuilder aggregatePatterns = new StringBuilder();
                    aggregatePatterns.Append("avg\\([a-zA-Z0-9_]+\\),");
                    aggregatePatterns.Append("min\\([a-zA-Z0-9_]+\\),");
                    aggregatePatterns.Append("max\\([a-zA-Z0-9_]+\\),");
                    aggregatePatterns.Append("count\\([a-zA-Z0-9_]+\\),");
                    aggregatePatterns.Append("sum\\([a-zA-Z0-9_]+\\)");

                    ///Splits the actual query into parts seperated by aggregate functions if any
                    ///and get only the parts that are aggregate functions
                    queryResult = Common.SplitByString(basePart, aggregatePatterns.ToString(),
                                  Common.SplitType.RemoveAllButThis, true);


                    if (queryResult != null && queryResult.Count == 0)
                    {
                        queryResult.Clear();
                        queryResult.Add(Common.NoAggregateFunctions);
                    }
                }
                else
                {
                    queryResult = null;
                }
            }
            else
            {
                queryResult = null;
            }
            return queryResult;
        }
        #endregion
    }
}
