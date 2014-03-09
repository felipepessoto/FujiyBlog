using System;
using System.Runtime.Serialization;

namespace FujiyBlog.Core.Caching
{
    [Serializable]
    public class InvalidCachedFuncException : Exception
    {
        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object</param>
        /// <param name="context"></param>
        protected InvalidCachedFuncException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

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
