﻿namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract
{
    public abstract class PageNavigationRequestedMessage<T> : PageNavigationRequestedMessage
    {
        protected PageNavigationRequestedMessage(T data, bool record = true)
            : base(record)
        {
            PageData = data;
        }

        public T PageData { get; }
    }
}