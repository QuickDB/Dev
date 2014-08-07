using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBTryingToSaveADocumentThatIsAlreadyInATransactionException : Exception
    {
        public QuickDBTryingToSaveADocumentThatIsAlreadyInATransactionException()
        {
        }

        public QuickDBTryingToSaveADocumentThatIsAlreadyInATransactionException(string message)
            : base(message)
        {
        }

        public QuickDBTryingToSaveADocumentThatIsAlreadyInATransactionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}