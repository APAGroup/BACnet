using System;

namespace BACnetAPA
{
    public sealed class SimpleSubsription
    {
        private DateTime _lastUpdate;

        public SimpleSubsription(BacnetItemInfo item, int id)
        {
            Item = item;
            _lastUpdate = DateTime.Now;
            Id = id;
            ReadsWaitInterval = TimeSpan.FromMilliseconds(0);
        }

        public int Id { get; }
        public BacnetItemInfo Item { get; }

        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set
            {
                ReadsWaitInterval = value - _lastUpdate;
                _lastUpdate = value;
            }
        }

        public bool WaitingRead { get; set; }
        private TimeSpan ReadsWaitInterval { get; set; }

        public bool IsReadyToQueue(DateTime time)
        {
            if (WaitingRead)
            {
                return false;
            }

            var span = time - LastUpdate;
            if (span.TotalMilliseconds > Item.SubscriptionInterval)
            {
                return true;
            }

            return false;
        }
    }
}
