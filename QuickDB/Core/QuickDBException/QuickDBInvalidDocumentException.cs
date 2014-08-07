using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBInvalidDocumentException : Exception
    {
        public QuickDBInvalidDocumentException()
        {
        }

        public QuickDBInvalidDocumentException(string message)
            : base(message)
        {
        }

        public QuickDBInvalidDocumentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}