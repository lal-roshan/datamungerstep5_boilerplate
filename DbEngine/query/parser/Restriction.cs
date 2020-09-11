
namespace DbEngine.Query.Parser
{
    /// <summary>
    /// Class representing a condition in the query
    /// </summary>
    public class Restriction
    {
        #region Properties
        /// <summary>
        /// The property name part of the condition
        /// </summary>
        public string propertyname;

        /// <summary>
        /// The value part of the condition
        /// </summary>
        public string propertyValue;

        /// <summary>
        /// The condition used in the condition
        /// </summary>
        public string condition;
        #endregion

        #region Constructor
        /// <summary>
        /// Parametrised constructor initialising the properties
        /// </summary>
        /// <param name="propertyName">Property name part of condition</param>
        /// <param name="propertyValue">Property value part of condition</param>
        /// <param name="condition">The condition used in condition</param>
        public Restriction(string propertyName, string propertyValue, string condition)
        {
            this.propertyname = propertyName;
            this.propertyValue = propertyValue;
            this.condition = condition;
        }
        #endregion
    }
}