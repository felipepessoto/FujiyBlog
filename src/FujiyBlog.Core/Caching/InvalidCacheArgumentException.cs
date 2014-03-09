using System;
using System.Runtime.Serialization;

namespace FujiyBlog.Core.Caching
{
    [Serializable]
    public class InvalidCacheArgumentException : Exception
    {
        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object</param>
        /// <param name="context"></param>
        protected InvalidCacheArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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
