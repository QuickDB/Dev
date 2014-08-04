using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBPermanentDocumentDeleteException : Exception
    {
        public QuickDBPermanentDocumentDeleteException()
        {
        }

        public QuickDBPermanentDocumentDeleteException(string message)
            : base(message)
        {
        }

        public QuickDBPermanentDocumentDeleteException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}