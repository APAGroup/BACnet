namespace BACnetAPA.EventArgs
{
    public sealed class SubscriptionRefreshEventArgs : System.EventArgs
    {
        public SubscriptionRefreshEventArgs(SimpleSubsription subscription)
        {
            Subscription = subscription;
        }

        public SimpleSubsription Subscription { get; }
    }
}
