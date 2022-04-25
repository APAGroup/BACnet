using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACnetAPA.EventArgs
{
    [CLSCompliant(false)]
    public sealed class DeviceFoundEventArgs : System.EventArgs
    {
        public DeviceFoundEventArgs(uint deviceId, string address)

        {
            DeviceId = deviceId;
            Address = address;
        }

        public string Address { get; }
        public BacnetAddress AddressBacnet { get; set; }
        public uint DeviceId { get; }
        public string ModelName { get; set; }
        public string Name { get; set; }
        public BacnetObjectTypes ObjectType { get; set; }
        public uint Segmentation { get; set; }
        public uint Status { get; set; }

        public string VendorName { get; set; }
    }
}
