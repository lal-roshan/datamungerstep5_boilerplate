using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DbEngine.helper
{
    #region Class
    public class Common
    {
        #region Constants

        /// <summary>
        /// Message to display if there is no file name in query
        /// </summary>
        public const string NoFileName = "Query doesn't contain any file name";

        /// <summary>
        /// String to display if there are no filters in query while getting filter
        /// </summary>
        public const string NoFilterString = "Query doesn't contain any filters";

        /// <summary>
        /// String to display if there are no logical operators in the filter part of query
        /// </summary>
        public const string NoLogicalOperatorsString = "Query doesn't contain any logical operators in filter";

        /// <summary>
        /// Message to be displayed when there is no order by clause in the query
        /// </summary>
        public const string NoOrderByClause = "Query doesn't contain order by clause";

        /// <summary>
        /// Message to be displayed when there is no group by clause in the query
        /// </summary>
        public const string NoGroupByClause = "Query doesn't contain group by clause";

        /// <summary>
        /// Message to be displayed when there is no order by clause in the base query
        /// </summary>
        public const string NoBaseOrderByClause = "Base query doesn't contain order by clause";

        /// <summary>
        /// Message to be displayed when there is no group by clause in the base query
        /// </summary>
        public const string NoBaseGroupByClause = "Base query doesn't contain group by clause";

        /// <summary>
        /// Message to be displayed when there are no aggregate functions in the base query
        /// </summary>
        public const string NoAggregateFunctions = "Base query doesn't contain any aggregate functions";
        #endregion

        #region Enums
        /// <summary>
        /// enum for denoting whether the first or last index is to be found
        /// </summary>
        public enum Index
        {
            First,
            Last
        }

        /// <summary>
        /// Enum for choosing what should the split funtion return from the input string
        /// DoNothing - Return parts of string by simply splitting the string at substirng(s)
        /// RemoveThis - Return parts of string by splitting the string at substirng(s) and
        /// removing substirng(s) form the collection
        /// RemoveAllButThis - Return parts of string which only constitutes the substring(s)
        /// </summary>
        public enum SplitType
        {
            DoNothing,
            RemoveThis,
            RemoveAllButThis
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Method to check whether all the where conditions are at valid positions
        /// </summary>
        /// <param name="queryString">input query</param>
        /// <returns>true or false based on query validity</returns>
        private static bool AreWherePositionsValid(string queryString)
        {
            ///Get all indexes of where keyword in string
            List<int> whereIndexes = GetStringIndexes(queryString, "where");
            if (whereIndexes.Count > 0)
            {
                ///Get all indexes of from keyword in string
                List<int> fromIndexes = GetStringIndexes(queryString, "from");

                ///In a valid query the number of where keyword should be equal to number of from keyword
                if (fromIndexes.Count == whereIndexes.Count)
                {
                    int index = 0;
                    while (index < fromIndexes.Count)
                    {
                        ///if where keyword comes between from keyword or if there are not even a single
                        ///character between from and where, the query is invalid
                        if ((whereIndexes[index] < fromIndexes[index]) ||
                            (whereIndexes[index] - fromIndexes[index] < 7))
                        {
                            return false;
                        }
                        index++;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Method to check whether order by and group by clauses are at valid positions
        /// </summary>
        /// <param name="queryString">Input query</param>
        /// <returns>true or false based on query validity</returns>
        private static bool AreOrderGroupPositionsValid(string queryString)
        {
            ///Get all the indexes of orderby  clause
            List<int> orderByIndexes = GetStringIndexes(queryString, "order by");
            ///Get all the indexes of groupby clause
            List<int> groupByIndexes = GetStringIndexes(queryString, "group by");

            /// if there are atleast one orderby and one groupby clause we have to check their position validity
            if (orderByIndexes.Count > 0 && groupByIndexes.Count > 0)
            {
                ///Get the lesser count value as we have to only ensure that each group by comes before each order by
                ///So if there are 3 order by and 2 group by, we only have to check whether
                ///the 2 group by comes before 2 order by
                int maxIndex = groupByIndexes.Count < orderByIndexes.Count ? groupByIndexes.Count :
                            orderByIndexes.Count;
                int index = 0;
                while (index < maxIndex)
                {
                    string inBetween = "";
                    /// All group by should be before order by, if not the group by should be a part of a substring, 
                    /// which is ensure by checkin whether ther is a ')' between group by and order by
                    if (orderByIndexes[index] < groupByIndexes[index])
                    {
                        inBetween = queryString.Substring(orderByIndexes[index],
                                    groupByIndexes[index] - orderByIndexes[index]);
                        if (!isPartOfSubQuery(inBetween))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        ///If order by comes after group by ensure that there are no and keyword in between unless it
                        ///is in having clause
                        inBetween = queryString.Substring(groupByIndexes[index],
                                    orderByIndexes[index] - groupByIndexes[index]);
                        int andIndex = GetStringIndex(inBetween, "and");
                        if (andIndex > -1)
                        {
                            int havingIndex = GetStringIndex(inBetween, "having");
                            if ((havingIndex == -1) || (andIndex < havingIndex &&
                               !isPartOfSubQuery(inBetween.Substring(andIndex, havingIndex - andIndex))))
                            {
                                return false;
                            }
                        }
                    }
                    index++;
                }
                return true;
            }
            return true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to do a basic checking whether or not a query is valid
        /// </summary>
        /// <param name="queryString">input query</param>
        /// <returns></returns>true or false
        public static bool IsValidQueryBasic(string queryString)
        {
            ///valid query will have atleast 16 characters before where Eg: select * from t
            ///(taking 15 as it is 0 based index)
            if (string.IsNullOrEmpty(queryString) || queryString.Length < 15)
            {
                return false;
            }
            else
            {
                int selectIndex = GetStringIndex(queryString, "select");
                int fromIndex = GetStringIndex(queryString, "from");

                ///Checks wether there is a select and from clause and whether there is atleast a character
                ///between them (selectIndex + 6 is for 5 letters of select plus one for whitespace)
                if (selectIndex == -1 || fromIndex == -1 || selectIndex + 7 == fromIndex)
                {
                    return false;
                }

                ///Calculating the count of each main keywords in string
                int selectCount = StringMatchCount(queryString, "select");
                int fromCount = StringMatchCount(queryString, "from");
                int whereCount = StringMatchCount(queryString, "where");
                int orderByCount = StringMatchCount(queryString, "order by");
                int groupByCount = StringMatchCount(queryString, "group by");

                ///if a query is valid there should be equal number of select and from keyword
                ///similarly if a query has where , order by or group clauses there count 
                ///cannot be greater than the select clause
                if (selectCount != fromCount ||
                   (whereCount > 0 && whereCount > selectCount) ||
                   (orderByCount > 0 && orderByCount > selectCount) ||
                   (groupByCount > 0 && groupByCount > selectCount) ||
                   (!AreWherePositionsValid(queryString)) ||
                   (!AreOrderGroupPositionsValid(queryString)))
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Get the index of an item from a list
        /// </summary>
        /// <param name="item">item to be found</param>
        /// <param name="items">list of items</param>
        /// <returns>Returns index of the item if found else -1</returns>
        public static int GetIndexFromList<T>(T item, List<T> items)
        {
            if(typeof(T) == typeof(string))
            {
                return items.FindIndex(i => string.Equals(i.ToString(), item.ToString()
                    , StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                return items.FindIndex(i => i.Equals(item));
            }
        }

        /// <summary>
        /// Method to find the first or last index of a substring in a string
        /// </summary>
        /// <param name="source">String in which substring is to be searched</param>
        /// <param name="pattern">Substring whose index is to be found</param>
        /// <param name="type">Enum value representing whether first or last index is required(Default is first)</param>
        /// <returns>Index of the pattern if present else -1</returns>
        public static int GetStringIndex(string source, string pattern, Index type = Index.First)
        {
            int index = -1;
            ///If source string is null
            if (string.IsNullOrEmpty(source))
            {
                return -1;
            }
            ///Else find the substring index based on type: first or last
            switch (type)
            {
                case Index.First:
                    index = source.Trim().IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase);
                    break;
                case Index.Last:
                    index = source.Trim().LastIndexOf(pattern, StringComparison.InvariantCultureIgnoreCase);
                    break;
            }
            return index;
        }

        /// <summary>
        /// Method to find all indexes of pattern occurence in source string
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="pattern">substring to be checked</param>
        /// <returns>List of all indexes</returns>
        public static List<int> GetStringIndexes(string source, string pattern)
        {
            List<int> indexes = new List<int>();
            int index = GetStringIndex(source, pattern);
            while (index > 0)
            {
                indexes.Add(index);
                index = GetStringIndex(source.Substring(index, source.Length - index), pattern);
            }
            return indexes;
        }

        /// <summary>
        /// Method to get the count of matched substring in source string
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="pattern">substring to be checked</param>
        /// <returns>Number of matches of substring found in source</returns>
        /// <param name="isRegex">Reperesents whether the pattern is a regex pattern</param>
        public static int StringMatchCount(string source, string pattern, bool isRegex = false)
        {
            int count;
            if (string.IsNullOrEmpty(source))
            {
                count = 0;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(pattern);

                ///If the pattern is a regex pattern having a \b at ends doesn't result in a match
                ///whereas it is required for normal patterns as whole word should be matched.
                ///That is, if we are checking 'or' in 'Banglore', without \b it would return true which is not expected
                if (isRegex)
                {
                    sb.Insert(0, "(");
                    sb.Append(")");
                }
                else
                {
                    sb.Insert(0, "\\b(");
                    sb.Append(")\\b");
                }

                count = Regex.Matches(source, sb.ToString(), RegexOptions.IgnoreCase).Count;
            }
            return count;
        }

        /// <summary>
        /// Method to replace the occurence of a substring from the source with a substitute
        /// </summary>
        /// <param name="source">source string</param>
        /// <param name="substring">part to be replaced</param>
        /// <param name="substitute">part to be added (Default is empty string)</param>
        /// <returns>new string with replaced substring</returns>
        public static string StringReplace(string source, string substring, string substitute = "")
        {
            return Regex.Replace(source, substring, substitute, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Method to split the source string by substring(s)
        /// </summary>
        /// <param name="source">The input string that is to be split</param>
        /// <param name="substrings">coma seperated substrings at which source has to be split
        /// (If no string is provided the source will be split by space and split type will be ignored)</param>
        /// <param name="type">Type of operation that should be performed on the returned collection</param>
        /// <param name="isRegex">Denotes whether the substring provided is a regex pattern</param>
        /// <returns>Collection of split words</returns>
        public static List<string> SplitByString(string source, string substrings,
            SplitType type = SplitType.DoNothing, bool isRegex = false)
        {
            List<string> splitResult = new List<string>();

            ///If no substrings are provided then split the source by whitespace
            if (string.IsNullOrEmpty(substrings))
            {
                splitResult = source.Split(' ').ToList();
            }
            else
            {
                ///Creating a regex filter with all substrings
                StringBuilder sb = new StringBuilder();
                foreach (string substring in substrings.Split(','))
                {
                    sb.Append(substring.Trim() + "|");
                }
                sb.Remove(sb.Length - 1, 1);

                ///If the substring is a regex pattern having a \b at ends doesn't result in a match
                ///whereas it is required for normal patterns as whole word should be matched.
                ///That is, if we are spliting with 'or' in 'Banglore',
                ///without \b it would return 'Bangl', 'e' which is not expected
                if (isRegex)
                {
                    sb.Insert(0, "(");
                    sb.Append(")");
                }
                else
                {
                    sb.Insert(0, "\\b(");
                    sb.Append(")\\b");
                }

                ///Split the souce based on substrings provided
                splitResult = Regex.Split(source, sb.ToString(), RegexOptions.IgnoreCase).ToList();

                ///An empty item in list will only be present if there are spaces between keywords mentioned in substring
                if (splitResult.Any(x => string.IsNullOrEmpty(x.Trim())))
                {
                    if (splitResult.Any(x => string.Equals(x.Trim().ToLower(), "not")))
                    {
                        splitResult = splitResult.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    }
                    else
                    {
                        return null;
                    }
                }

                ///Decide whether to remove the substrings from source or keep substrings
                if (splitResult.Count > 0)
                {
                    switch (type)
                    {
                        ///If ANY of the substrings gets matched with the item in splitresult it gets added to the result
                        case SplitType.RemoveAllButThis:
                            splitResult = splitResult.Where(x => substrings.Split(',')
                            .Any(part => StringMatchCount(x, part.Trim(), isRegex) > 0)).
                            Select(x => x.Trim()).ToList();
                            break;

                        ///If ALL of the substrings doesnt match with the item in splitresult it gets added to the result
                        case SplitType.RemoveThis:
                            splitResult = splitResult.Where(x => substrings.Split(',')
                            .All(part => StringMatchCount(x, part.Trim(), isRegex) == 0)).
                            Select(x => x.Trim()).ToList();
                            break;
                    }
                }
            }

            return splitResult;
        }

        /// <summary>
        /// Method to check whether the string is a part of sub query
        /// </summary>
        /// <param name="queryString">part of query</param>
        /// <returns></returns>
        public static bool isPartOfSubQuery(string queryString)
        {
            if (queryString.Contains(')'))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to get the filter condition into three parts (fields, operator and value) if it is valid
        /// </summary>
        /// <param name="conditionString">The condition string</param>
        /// <returns>Returns conition split into three parts or null</returns>
        public static List<string> SplitConditionWords(string conditionString)
        {
            List<string> conditionParts = new List<string>();
            if (!string.IsNullOrEmpty(conditionString.Trim()))
            {
                List<string> splitBySpace = conditionString.Split(' ').ToList();
                ///If there isnt three parts in a condition i.e, field, operator and value
                /// Then the query is invalid
                if (splitBySpace.Count != 3)
                {
                    ///We need to ensure that if there are any valid conditions that are written without
                    ///space between the three parts
                    List<string> splitParts = Common.SplitByString(conditionString,
                                        "=,!=,<,<=,>,>=", SplitType.DoNothing, true);
                    if (splitParts == null || splitParts.Count != 3)
                    {
                        conditionParts = null;
                    }
                    else
                    {
                        conditionParts.AddRange(splitParts);
                    }
                }
                else
                {
                    conditionParts.AddRange(splitBySpace);
                }
            }
            return conditionParts;
        }

        /// <summary>
        /// Method to split aggregate function and fields
        /// </summary>
        /// <param name="aggregateString">Aggregate string</param>
        /// <returns>Returns a list containing an aggregate function and correspoding field or null</returns>
        public static List<string> SplitAggregateFields(string aggregateString)
        {
            List<string> aggregateParts = new List<string>();

            if (string.IsNullOrEmpty(aggregateString.Trim()))
            {
                aggregateParts = null;
            }
            else
            {
                ///by splitting string at '(' we can get the aggregate function
                ///by trimming off the ')' from last part we can get the field
                ///reversing the order for convenience
                aggregateParts = aggregateString.Split('(').ToList();
                if (aggregateParts.Count > 2)
                {
                    aggregateParts = null;
                }
                else
                {
                    aggregateParts[1] = aggregateParts[1].Trim(')');
                    aggregateParts.Reverse();
                }
            }
            return aggregateParts;
        }
        #endregion
    }
    #endregion
}
