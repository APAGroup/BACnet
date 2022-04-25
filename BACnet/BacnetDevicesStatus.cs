using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.Linq;

namespace BACnetAPA
{
    [CLSCompliant(false)]
    public class BacnetDevicesStatus
    {
        private readonly IList<BacnetMonitoredDevice> _addresses;
        private readonly object _lock;

        public BacnetDevicesStatus(IEnumerable<BacnetItemInfo> data)
        {
            _addresses = new List<BacnetMonitoredDevice>();
            _lock = new object();
            var toObtain = data.GroupBy(a => a.DeviceId);
            foreach (var item in toObtain)
            {
                var device = item.FirstOrDefault();
                if (device == null)
                {
                    continue;
                }

                _addresses.Add(new BacnetMonitoredDevice(new BacnetAddress(BacnetAddressTypes.IP, device.IpAddress),
                    device.DeviceId));
            }
        }

        public event EventHandler<DeviceRestartInfoEventArgs> OnDeviceCouldBeRestarted;

        public void UpdateIAm(BacnetClient client, BacnetAddress address, bool ommitRestart)
        {
            lock (_lock)
            {
                var item = _addresses.FirstOrDefault(a => a.IsEqual(address));
                if (item == null)
                {
                    return;
                }

                if (item.ProbablyRestarted() && !ommitRestart)
                {
                    OnDeviceCouldBeRestarted?.Invoke(this, new DeviceRestartInfoEventArgs(client, item.DeviceId));
                }
                item.LastIAmTimeStamp = DateTime.Now;
            }
        }
    }

    public class DeviceRestartInfoEventArgs : System.EventArgs
    {
        public DeviceRestartInfoEventArgs(BacnetClient client, int deviceId)
        {
            Client = client;
            DeviceId = deviceId;
        }

        public BacnetClient Client { get; }
        public int DeviceId { get; }
    }
}