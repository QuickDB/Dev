using System;

namespace QuickDB.Core.QuickDBException
{
    public class QuickDBUnableToLoadParticipantOfATransactionException : Exception
    {
        public QuickDBUnableToLoadParticipantOfATransactionException()
        {
        }

        public QuickDBUnableToLoadParticipantOfATransactionException(string message)
            : base(message)
        {
        }

        public QuickDBUnableToLoadParticipantOfATransactionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}