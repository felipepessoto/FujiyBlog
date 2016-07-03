using System;

namespace FujiyBlog.Core.Caching
{
    public class InvalidCachedFuncException : Exception
    {
        /// <summary>
        /// Class Constructor
        /// </summary>
        public InvalidCachedFuncException()
        { }

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="message">Text that will be displayed</param>
        public InvalidCachedFuncException(string message)
            : base(message)
        { }

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="message">Text that will be displayed</param>
        /// <param name="innerException">Wrapped exception</param>
        public InvalidCachedFuncException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
