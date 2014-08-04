using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBErrorDeletingDocumentException : Exception
    {
        public QuickDBErrorDeletingDocumentException()
        {
        }

        public QuickDBErrorDeletingDocumentException(string message)
            : base(message)
        {
        }

        public QuickDBErrorDeletingDocumentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}