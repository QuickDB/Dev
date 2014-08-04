using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBTryingToReadNonExistentDocumentException : Exception
    {
        public QuickDBTryingToReadNonExistentDocumentException()
        {
        }

        public QuickDBTryingToReadNonExistentDocumentException(string message)
            : base(message)
        {
        }

        public QuickDBTryingToReadNonExistentDocumentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}