using System;
using System.IO.BACnet;
using BACnetAPA.Helpers;

namespace BACnetAPA
{
    public class BacnetMonitoredDevice
    {
        private static int MaxIAmTimeout = 120;

        [CLSCompliant(false)]
        public BacnetMonitoredDevice(BacnetAddress address, int deviceId)
        {
            Address = address;
            DeviceId = deviceId;
            LastIAmTimeStamp = DateTime.Now;
        }

        [CLSCompliant(false)]
        public BacnetAddress Address { get; }

        public int DeviceId { get; }

        public DateTime LastIAmTimeStamp { get; set; }

        [CLSCompliant(false)]
        public bool IsEqual(BacnetAddress address)
        {
            return Address.GetIpFromAddress().Equals(address.GetIpFromAddress());
        }

        public bool ProbablyRestarted()
        {
            return DateTime.Now - LastIAmTimeStamp > TimeSpan.FromSeconds(MaxIAmTimeout);
        }
    }
}