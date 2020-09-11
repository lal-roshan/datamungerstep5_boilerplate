using DbEngine.Query;
using DbEngine.Query.Parser;

namespace DbEngine.Reader
{
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
}