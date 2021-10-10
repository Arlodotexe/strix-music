namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract
{
    public abstract class PageNavigationRequestedMessage
    {
        public PageNavigationRequestedMessage(bool record = true)
        {
            RecordNavigation = record;
        }

        public bool RecordNavigation { get; set; }
    }
}
