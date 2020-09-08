using DbEngine.helper;
using DbEngine.Query;
using System;
using System.IO;
using System.Linq;

namespace DbEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read the query from the user
            Console.WriteLine("Please enter the query:\n\n");
            string queryString = Console.ReadLine();

            DataSet data = new Query.Query().executeQuery(queryString);

            string fileName = QueryHelper.GetFileNameFromQuery(queryString);

            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = fileName.Split('.').First();
            }

            if(Common.GetStringIndex(queryString, QueryHelper.GetRoot()) < 0)
            {
                fileName = QueryHelper.GetRoot() + fileName;
            }

            fileName = fileName + ".json";

            writer.JsonWriter writer = new writer.JsonWriter();
            writer.WriteToJson(data, fileName);

            /*
              Instantiate Query class. This class is responsible for:
              1. Parsing the query.
              2. Select the appropriate type of query processor.
              3. Get the DataSet which is populated by the Query Processor
             */


            /*
             * Instantiate JsonWriter class. This class is responsible for writing the DataSet into a JSON file
             */

            /*
             * call executeQuery() method of Query class to get the resultSet. Pass this resultSet as parameter to writeToJson() method of JsonWriter class to write the resultSet into a JSON file
             */
        }
    }
}
