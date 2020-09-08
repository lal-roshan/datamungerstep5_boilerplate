#region Namespace
namespace DbEngine.Query
{
    #region Class
    /// <summary>
    /// Class representing the data types of each columns
    /// </summary>
    public class DataTypeDefinitions
    {
        #region Properties
        /// <summary>
        /// Represents the set of data types of columns
        /// </summary>
        public string[] DataTypes { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Parameterized constructor for initializing the data types set
        /// </summary>
        /// <param name="dataTypes"></param>
        public DataTypeDefinitions(string[] dataTypes)
        {
            this.DataTypes = dataTypes;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Method to convert the array of datatypes to string seperated by comma
        /// </summary>
        /// <returns>string of data types of each columns seperated by comma</returns>
        public override string ToString()
        {
            return string.Join(", ", DataTypes);
        }
        #endregion
    } 
    #endregion
} 
#endregion