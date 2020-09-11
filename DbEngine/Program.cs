using DbEngine.helper;
using DbEngine.Query;
using System;
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
        }
    }
}
