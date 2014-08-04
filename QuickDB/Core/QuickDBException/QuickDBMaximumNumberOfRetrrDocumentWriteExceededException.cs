using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBMaximumNumberOfRetrrDocumentWriteExceededException : Exception
    {
        public QuickDBMaximumNumberOfRetrrDocumentWriteExceededException()
        {
        }

        public QuickDBMaximumNumberOfRetrrDocumentWriteExceededException(string message)
            : base(message)
        {
        }

        public QuickDBMaximumNumberOfRetrrDocumentWriteExceededException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}