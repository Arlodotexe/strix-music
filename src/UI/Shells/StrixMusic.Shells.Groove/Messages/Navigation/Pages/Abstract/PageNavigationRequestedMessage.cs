namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract
{
    public abstract class PageNavigationRequestedMessage<T>
        where T : class
    {
        protected PageNavigationRequestedMessage(T data) => PageData = data;

        public T PageData { get; }
    }
}
