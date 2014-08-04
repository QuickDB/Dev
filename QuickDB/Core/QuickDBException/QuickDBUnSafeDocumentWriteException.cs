using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBUnSafeDocumentWriteException : Exception
    {
        public QuickDBUnSafeDocumentWriteException()
        {
        }

        public QuickDBUnSafeDocumentWriteException(string message)
            : base(message)
        {
        }

        public QuickDBUnSafeDocumentWriteException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}