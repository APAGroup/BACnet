using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BACnetAPA.EventArgs;

namespace BACnetAPA
{
	public sealed class BacnetSubscriptions : IDisposable

	{
		private const int RefreshInterval = 100;
		private readonly object _lock;
		private readonly Timer _refreshTimer;
		private readonly IList<SimpleSubsription> _subscriptions;
		private bool _disposed;

		public BacnetSubscriptions()
		{
			_subscriptions = new List<SimpleSubsription>();
			_refreshTimer = new Timer(Callback, null, Timeout.Infinite, Timeout.Infinite);
			_lock = new object();
		}

		~BacnetSubscriptions()
		{
			Dispose(false);
		}

		public event EventHandler<SubscriptionRefreshEventArgs> OnSubscription;

		public void Add(SimpleSubsription item)
		{
			lock (_lock)
			{ _subscriptions.Add(item); }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}

            if (_disposed) return;
            _refreshTimer.Dispose();
            _disposed = true;
        }

		public void RemoveByDevice(int deviceId)
		{
			lock (_lock)
			{
				var toDelete = _subscriptions.Where(a => a.Item.DeviceId == deviceId);
				foreach (var item in toDelete)
				{
					_subscriptions.Remove(item);
				}
			}
		}

		public void Run()
		{
			_refreshTimer.Change(RefreshInterval, Timeout.Infinite);
		}

		public void Stop()
		{
			_refreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		public void UpdateRead(int id)
		{
			lock (_lock)
			{
				var item = _subscriptions.FirstOrDefault(a => a.Id == id);
                if (item == null) return;
                item.LastUpdate = DateTime.Now;
                item.WaitingRead = false;
            }
		}

		private void Callback(object state)
		{
			try
			{
				if (_disposed)
				{
					return;
				}
				lock (_lock)
				{
					var nowTime = DateTime.Now;
					var toUpdate = _subscriptions.Where(a => a.IsReadyToQueue(nowTime)).ToList();
					foreach (var item in toUpdate)
					{
						item.WaitingRead = true;
						OnSubscription?.Invoke(this, new SubscriptionRefreshEventArgs(item));
					}
				}
			}
			finally
			{
				if (!_disposed)
				{
					_refreshTimer.Change(RefreshInterval, Timeout.Infinite);
				}
			}
		}
	}
}