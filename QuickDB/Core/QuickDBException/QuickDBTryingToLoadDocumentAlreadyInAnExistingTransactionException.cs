using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBTryingToEnterIntoTransactionWithADocumentAlreadyInAnExistingTransactionException : Exception
    {
        public QuickDBTryingToEnterIntoTransactionWithADocumentAlreadyInAnExistingTransactionException()
        {
        }

        public QuickDBTryingToEnterIntoTransactionWithADocumentAlreadyInAnExistingTransactionException(string message)
            : base(message)
        {
        }

        public QuickDBTryingToEnterIntoTransactionWithADocumentAlreadyInAnExistingTransactionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}