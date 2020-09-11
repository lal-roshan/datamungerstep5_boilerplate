using System;

namespace DbEngine.exceptions
{
    /// <summary>
    /// Exception to be thrown in case of invalid sql query
    /// </summary>
    public class InvalidQueryException: Exception
    {
        public InvalidQueryException(string message = "")
            :base($"Invalid query{(string.IsNullOrEmpty(message)? "" :": " + message)}!!!") { }
    }
}
