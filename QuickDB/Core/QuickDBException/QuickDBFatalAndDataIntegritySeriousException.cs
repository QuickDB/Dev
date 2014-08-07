using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBFatalAndDataIntegritySeriousException : Exception
    {
        public QuickDBFatalAndDataIntegritySeriousException()
        {
        }

        public QuickDBFatalAndDataIntegritySeriousException(string message)
            : base(message)
        {
        }

        public QuickDBFatalAndDataIntegritySeriousException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}