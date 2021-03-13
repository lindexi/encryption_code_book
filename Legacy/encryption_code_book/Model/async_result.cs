using System;
using System.Threading;

namespace encryption_code_book
{
    public class async_result : IAsyncResult
    {
        public async_result(bool complete)
        {
            IsCompleted = IsCompleted;
            AsyncState = new object();
        }

        public async_result(object asyncState, WaitHandle asyncWaitHandle, bool completedSynchronously, bool isCompleted)
        {
            AsyncState = asyncState;
            AsyncWaitHandle = asyncWaitHandle;
            CompletedSynchronously = completedSynchronously;
            IsCompleted = isCompleted;
        }

        public object AsyncState { get; }

        public WaitHandle AsyncWaitHandle { get; }

        public bool CompletedSynchronously { get; }

        public bool IsCompleted { get; }
    }
}