using DbEngine.Query;
using DbEngine.Query.Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DbEngine.Reader
{
    /// <summary>
    /// Class containing various methods for fetching data from file
    /// </summary>
    public class CsvQueryProcessor : QueryProcessingEngine
    {
        #region Properties
        /// <summary>
        /// Represents the file name in the query
        /// </summary>
        private readonly string _fileName;

        /// <summary>
        /// Reader used for reading the file
        /// </summary>
        private StreamReader _reader;
        #endregion

        #region Constructor
        /// <summary>
        /// parameterized constructor to initialize filename.
        /// </summary>
        /// <param name="fileName"></param>
        public CsvQueryProcessor(string fileName)
        {
            if (File.Exists(fileName))
            {
                this._fileName = fileName;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Method to get selected fields from the row
        /// </summary>
        /// <param name="fields">The fields that are to be selected</param>
        /// <param name="rowValues">A single data row </param>
        /// <returns>A list of selected fields from the data row</returns>
        private List<string> GetSelectedRowsData(List<string> fields, List<string> rowValues)
        {
            ///If the stirng in fields property is * we can choose all column from the row
            if (string.Equals(fields?.First(), "*"))
            {
                rowValues = rowValues.Select(data =>
                string.IsNullOrEmpty(data.Trim()) ? null : data.Trim()
                ).ToList();
            }
            ///If the string in fields property is not * then we will find the positions of the selected 
            ///fields from header information and only choose the data at those position from the row
            else
            {
                List<int> fieldPositions = fields.Select(field => GetHeader().Headers.ToList()
                                            .IndexOf(field)).ToList();
                rowValues = fieldPositions.Select(position =>
                            rowValues[position]).ToList();
            }

            return rowValues;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to get the column headers from the file
        /// </summary>
        /// <returns>A Header object that contains the list of column headers</returns>
        public override Header GetHeader()
        {
            using (_reader = new StreamReader(_fileName))
            {
                string headerRow = _reader.ReadLine();
                if (!string.IsNullOrEmpty(headerRow))
                {
                    Header header = new Header(headerRow.Split(',').Select(x => x.Trim()).ToArray());
                    return header;
                }
            }
            return null;
        }

        /// <summary>
        /// Method to find the data type of each column in the file
        /// </summary>
        /// <returns>Returns DataTypeDefinitions object containing the list of data types of all column</returns>
        public override DataTypeDefinitions GetColumnType()
        {
            using (_reader = new StreamReader(_fileName))
            {
                ///Discard first line as it is the header row
                _reader.ReadLine();

                ///Read second line which represents the first data row
                string firstDataRow = _reader.ReadLine();

                if (!string.IsNullOrEmpty(firstDataRow))
                {
                    #region Regex Patterns
                    ///Various regex patterns used to find data type
                    string ddString = $"((0[1-9])|([12]\\d)|(3[01]))";
                    string mmString = $"((0[1-9])|(1[012]))";
                    string yyString = $"(\\d{{2}})";
                    string yyyyString = $"(\\d{{4}})";
                    string shortMonthString = $"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)";
                    string longMonthString = $"(January|February|March|April|May|June|July|August|September|October|November|December)";
                    #endregion

                    List<string> dataTypes = new List<string>();
                    foreach (string field in firstDataRow.Split(','))
                    {
                        /// checking for Integer
                        if (Regex.IsMatch(field, $"^([0-9]+)$"))
                        {
                            dataTypes.Add(typeof(System.Int32).ToString());
                        }
                        /// checking for floating point numbers
                        else if (Regex.IsMatch(field, $"^([0-9]+.[0-9]+)$"))
                        {
                            dataTypes.Add(typeof(System.Double).ToString());
                        }
                        /// checking for date format dd/mm/yyyy
                        else if (Regex.IsMatch(field,
                            $"^({ddString}/{mmString}/{yyyyString})$"))
                        {
                            dataTypes.Add(typeof(System.DateTime).ToString());
                        }
                        /// checking for date format mm/dd/yyyy
                        else if (Regex.IsMatch(field,
                            $"^({mmString}/{ddString}/{yyyyString})$"))
                        {
                            dataTypes.Add(typeof(System.DateTime).ToString());
                        }
                        /// checking for date format dd-mon-yy
                        else if (Regex.IsMatch(field,
                            $"^({ddString}-{shortMonthString}-{yyString})$", RegexOptions.IgnoreCase))
                        {
                            dataTypes.Add(typeof(System.DateTime).ToString());
                        }
                        /// checking for date format dd-mon-yyyy
                        else if (Regex.IsMatch(field,
                            $"^({ddString}-{shortMonthString}-{yyyyString})$",
                            RegexOptions.IgnoreCase))
                        {
                            dataTypes.Add(typeof(System.DateTime).ToString());
                        }
                        /// checking for date format dd-month-yy
                        else if (Regex.IsMatch(field,
                            $"^({ddString}-{longMonthString}-{yyString})$",
                            RegexOptions.IgnoreCase))
                        {
                            dataTypes.Add(typeof(System.DateTime).ToString());
                        }
                        /// checking for date format dd-month-yyyy
                        else if (Regex.IsMatch(field,
                            $"^({ddString}-{longMonthString}-{yyyyString})$",
                            RegexOptions.IgnoreCase))
                        {
                            dataTypes.Add(typeof(System.DateTime).ToString());
                        }
                        /// checking for date format yyyy-mm-dd
                        else if (Regex.IsMatch(field,
                            $"^({yyyyString}-{mmString}-{ddString})$"))
                        {
                            dataTypes.Add(typeof(System.DateTime).ToString());
                        }
                        else if (string.IsNullOrEmpty(field))
                        {
                            dataTypes.Add(typeof(System.Object).ToString());
                        }
                        else
                        {
                            dataTypes.Add(typeof(System.String).ToString());
                        }
                    }
                    DataTypeDefinitions dataTypeDefinitions = new DataTypeDefinitions(dataTypes.ToArray());
                    return dataTypeDefinitions;
                }
            }
            return null;
        }

        /*#region Unused methods for future needs
        /// <summary>
        /// Method to get specific row based on the row number
        /// </summary>
        /// <param name="rowNumber">The number of the row</param>
        /// <returns>Single row at the provided row number</returns>
        public Row GetRow(int rowNumber)
        {
            using (_reader = new StreamReader(_fileName))
            {
                while (rowNumber-- > 0)
                {
                    _reader.ReadLine();
                }

                string rowDatas = _reader.ReadLine();
                if (!string.IsNullOrEmpty(rowDatas))
                {
                    List<string> rowValues = new List<string>();
                    foreach (string data in rowDatas.Split(','))
                    {
                        rowValues.Add(data.Trim());
                    }
                    Row row = new Row(rowValues.ToArray());
                    return row;
                }

            }
            return null;
        }

        /// <summary>
        /// Method to get all rows from the file
        /// </summary>
        /// <returns>List of all rows in the file</returns>
        public List<Row> GetAllRows()
        {
            List<Row> rows = new List<Row>();
            using (_reader = new StreamReader(_fileName))
            {
                _reader.ReadLine();

                string rowDatas = _reader.ReadLine();
                while (!string.IsNullOrEmpty(rowDatas))
                {
                    List<string> rowValues = new List<string>();
                    foreach (string data in rowDatas.Split(','))
                    {
                        rowValues.Add(data.Trim());
                    }
                    rows.Add(new Row(rowValues.ToArray()));
                }
            }
            if (rows.Any())
            {
                return rows;
            }
            return null;
        }
        #endregion*/

        /// <summary>
        /// Method to get the dataset from file based on the query
        /// </summary>
        /// <param name="queryParameter">Object containing properties from the query</param>
        /// <returns>The dataset from file based on the query or null</returns>
        public override DataSet GetDataRow(QueryParameter queryParameter)
        {
            if (queryParameter != null)
            {
                List<Row> rows = new List<Row>();

                using (StreamReader rowReader = new StreamReader(_fileName))
                {
                    rowReader.ReadLine();

                    string rowDatas = rowReader.ReadLine();
                    while (!string.IsNullOrEmpty(rowDatas))
                    {
                        List<string> rowValues = new List<string>();

                        ///Get the current row values into a list
                        rowValues = rowDatas.Split(',').Select(data => data.Trim()).ToList();

                        ///Restriction not null means there is some conditions applied in the query
                        if (queryParameter.Restrictions != null)
                        {
                            ///Apply the conditions in the query on current row and choose the data from the current row
                            ///only if the conditions satisfy
                            if (Filter.EvaluateExpression(queryParameter, rowValues))
                            {
                                rows.Add(new Row(GetSelectedRowsData(queryParameter.Fields,
                                    rowValues).ToArray()));
                            }
                        }
                        ///If restrictions property is null it means there is no where clause
                        else
                        {
                            rows.Add(new Row(GetSelectedRowsData(queryParameter.Fields,
                                    rowValues).ToArray()));
                        }

                        ///Read next row
                        rowDatas = rowReader.ReadLine();
                    }
                }

                ///If any satisfying rows where found
                if (rows.Any())
                {
                    return new DataSet(rows.ToArray());
                }
            }

            return null;
        }
        #endregion
    }
}