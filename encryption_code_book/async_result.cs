using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace encryption_code_book
{
    public class async_result : IAsyncResult
    {
        public async_result(bool complete)
        {
            _completed = IsCompleted;
            _asyncState = new object();            
        }
        public async_result(object AsyncState, WaitHandle AsyncWaitHandle , bool CompletedSynchronously , bool IsCompleted)
        {
            _asyncState = AsyncState;
            _asyncWaitHandle = AsyncWaitHandle;
            _CompletedSynchronously = CompletedSynchronously;
            _completed = IsCompleted;
        }
        public object AsyncState
        {
            get
            {
                return _asyncState;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return _asyncWaitHandle;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                return _CompletedSynchronously;
            }
        }
        public bool IsCompleted
        {
            get
            {
                return _completed;
            }
        }
        private object _asyncState;
        private WaitHandle _asyncWaitHandle;
        private bool _CompletedSynchronously;
        private bool _completed;
    }
}
