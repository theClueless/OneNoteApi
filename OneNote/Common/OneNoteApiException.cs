using System;
using System.Runtime.Serialization;

namespace OneNoteApi.Common
{
    [Serializable]
    public class OneNoteApiException : Exception
    {
        public OneNoteApiException()
        {
        }

        public OneNoteApiException(string message) : base(message)
        {
        }

        public OneNoteApiException(string message, Exception inner) : base(message, inner)
        {
        }

        protected OneNoteApiException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}