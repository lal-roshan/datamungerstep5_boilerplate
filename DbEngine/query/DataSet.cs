#region Namespace
namespace DbEngine.Query
{
    #region  Class
    /// <summary>
    /// Class representing the set of rows of data
    /// </summary>
    public class DataSet
    {
        #region Properties
        /// <summary>
        /// Represents the set of rows
        /// </summary>
        public Row[] Rows { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor for initialising the rows set
        /// </summary>
        /// <param name="rows"></param>
        public DataSet(Row[] rows)
        {
            this.Rows = rows;
        } 
        #endregion
    } 
    #endregion
}

#endregion