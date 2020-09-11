namespace DbEngine.Query
{
    /// <summary>
    /// Class containing the list of column headers in the file
    /// </summary>
    public class Header
    {
        #region Properties
        /// <summary>
        /// The list of column headers in the file
        /// </summary>
        public string[] Headers { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Parametrised constructor for initialising headers
        /// </summary>
        /// <param name="headers">array of string representing column headers</param>
        public Header(string[] headers)
        {
            this.Headers = headers;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Method to convert arrray of headers to a single comma seperated string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(", ", Headers);
        } 
        #endregion
    }
}