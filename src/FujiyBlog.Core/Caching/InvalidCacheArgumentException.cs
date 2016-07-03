using System;

namespace FujiyBlog.Core.Caching
{
    public class InvalidCacheArgumentException : Exception
    {
        /// <summary>
        /// Class Constructor
        /// </summary>
        public InvalidCacheArgumentException()
        { }

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="message">Text that will be displayed</param>
        public InvalidCacheArgumentException(string message)
            : base(message)
        { }

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="message">Text that will be displayed</param>
        /// <param name="innerException">Wrapped exception</param>
        public InvalidCacheArgumentException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
