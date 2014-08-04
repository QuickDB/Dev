using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBTryingToWriteToAReadOnlyDocumentException : Exception
    {
        public QuickDBTryingToWriteToAReadOnlyDocumentException()
        {
        }

        public QuickDBTryingToWriteToAReadOnlyDocumentException(string message)
            : base(message)
        {
        }

        public QuickDBTryingToWriteToAReadOnlyDocumentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}