using System;
using System.Runtime.Serialization;

namespace NSrtm.Core.Pgm.Exceptions
{
    [Serializable]
    public class InvalidFileTypeException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidFileTypeException()
        {
        }

        public InvalidFileTypeException(string message)
            : base(message)
        {
        }

        public InvalidFileTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InvalidFileTypeException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}