using System;

namespace QuickDB.Core.Models
{
    public interface IQuickDBSession<T>
    {
        bool ReadOnly { set; get; }

        bool ThrowExceptionIfThereAreChangesSinceLastLoad { set; get; }

        int MaximumUpdateHistoryDepth { set; get; }

        T Load(bool createIfItDoesNotExist = false);

        void Save(T data);

        void DeleteDocumentPermanently(T data);

        void Delete(T data);

        void TrySave(Func<T> updateDocumentActionToBeRetried, int numberOfRetries, int intervalBetweenRetries);
    }
}