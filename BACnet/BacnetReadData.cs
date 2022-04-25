namespace BACnetAPA
{
    public sealed class BacnetReadData : IBacnetRequest
    {
        public BacnetReadData(BacnetItemInfo client, int subscriptionId)
        {
            Client = client;
            SubscriptionId = subscriptionId;
        }

        public BacnetItemInfo Client { get; }

        public bool ReadRequest => true;

        public int SubscriptionId { get; }
    }
}