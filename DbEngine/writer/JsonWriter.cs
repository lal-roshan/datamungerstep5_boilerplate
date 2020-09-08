#region Usings
using DbEngine.Query;
using Newtonsoft.Json;
using System.IO;
#endregion

#region Namespace
namespace DbEngine.writer
{
    #region Class
    /// <summary>
    /// Class for writing the dataset into json file
    /// </summary>
    class JsonWriter
    {
        #region Public Methods
        /// <summary>
        /// Method to write data to specified json file
        /// </summary>
        /// <param name="data">The data which is to be written to the file</param>
        /// <param name="fileName">The file to which the data is to be written</param>
        /// <returns>True if file is written successfully else false</returns>
        public bool WriteToJson(DataSet data, string fileName)
        {
            try
            {
                ///Converting dataset to json data
                string jsonData = JsonConvert.SerializeObject(data);

                ///If file already exists delete the existing file
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                ///Write the json data to json file
                using (StreamWriter file = new StreamWriter(fileName, true))
                {
                    file.WriteLine(jsonData.ToString());
                }
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        } 
        #endregion
    } 
    #endregion
} 
#endregion
