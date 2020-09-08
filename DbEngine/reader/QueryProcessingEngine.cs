#region Usings
using DbEngine.Query;
using DbEngine.Query.Parser;
#endregion

#region Namespace
namespace DbEngine.Reader
{
    #region Abstract Class
    /// <summary>
    /// Abstract class containing method signatures that ll query processors should implement
    /// </summary>
    public abstract class QueryProcessingEngine
    {
        #region Method signatures
        public abstract Header GetHeader();
        public abstract DataSet GetDataRow(QueryParameter queryParameter);
        public abstract DataTypeDefinitions GetColumnType(); 
        #endregion
    } 
    #endregion
} 
#endregion