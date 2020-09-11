using DbEngine.helper;
using DbEngine.Query.Parser;
using DbEngine.Reader;

namespace DbEngine.Query
{
    /// <summary>
    /// Class that executes the query and return the resulting dataset
    /// </summary>
    public class Query
    {
        #region  Properties
        /// <summary>
        /// Query parser object for parsing and populating properties of query
        /// </summary>
        QueryParser queryParser = null;

        /// <summary>
        /// Query parameter object for saving the properties of query
        /// </summary>
        QueryParameter queryParameter = null;
        #endregion

        #region Public methods
        /// <summary>
        /// Method to execute the query and return the resulting dataset
        /// </summary>
        /// <param name="queryString">The input query</param>
        /// <returns>Returns the resultant dataset if any or null</returns>
        public DataSet executeQuery(string queryString)
        {
            if (Common.IsValidQueryBasic(queryString))
            {
                queryParser = new QueryParser();
                queryParameter = queryParser.ParseQuery(queryString);
                CsvQueryProcessor queryProcessor = new CsvQueryProcessor(queryParameter.File);

                if (queryProcessor != null)
                {
                    if (string.Equals(queryParameter.QueryType, "SIMPLE_QUERY"))
                    {
                        return queryProcessor.GetDataRow(queryParameter);
                    }
                }
            }
            return null;
        } 
        #endregion
    }
}