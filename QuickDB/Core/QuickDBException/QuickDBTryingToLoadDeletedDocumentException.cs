using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBTryingToLoadDeletedDocumentException : Exception
    {
        public QuickDBTryingToLoadDeletedDocumentException()
        {
        }

        public QuickDBTryingToLoadDeletedDocumentException(string message)
            : base(message)
        {
        }

        public QuickDBTryingToLoadDeletedDocumentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}