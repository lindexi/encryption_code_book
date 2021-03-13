using System;
using encryption_code_book.ViewModel;

namespace Framework.ViewModel
{
    public interface ISendMessage
    {
        EventHandler<Message> SendMessageHandler
        {
            set;
            get;
        }
    }
}