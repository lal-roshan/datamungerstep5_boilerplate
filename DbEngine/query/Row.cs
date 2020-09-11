namespace DbEngine.Query
{
    /// <summary>
    /// Class containing the data in a singe row
    /// </summary>
    public class Row
    {
        #region Properties
        /// <summary>
        /// Values representing a single data row
        /// </summary>
        public string[] RowValues { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Parametrised constructor for initialising the row values property
        /// </summary>
        /// <param name="rowValues"></param>
        public Row(string[] rowValues)
        {
            this.RowValues = rowValues;
        } 
        #endregion
    }
}