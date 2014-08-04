using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBTryingToCreateAnAlreadyExistingDocumentException : Exception
    {
        public QuickDBTryingToCreateAnAlreadyExistingDocumentException()
        {
        }

        public QuickDBTryingToCreateAnAlreadyExistingDocumentException(string message)
            : base(message)
        {
        }

        public QuickDBTryingToCreateAnAlreadyExistingDocumentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}