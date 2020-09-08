namespace DbEngine.Query.Parser
{
    /// <summary>
    /// This class contains the field and function name of aggregate function
    /// </summary>
    public class AggregateFunction
    {
        #region Properties
        /// <summary>
        /// Field on which aggregate function will be performed
        /// </summary>
        public string field;

        /// <summary>
        /// The name of the aggregate function used
        /// </summary>
        public string function;
        #endregion

        #region Constructor
        /// <summary>
        /// Parametrised constructor for initializing field and function names
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="function">Function name</param>
        public AggregateFunction(string field, string function)
        {
            this.field = field;
            this.function = function;
        } 
        #endregion
    }
}