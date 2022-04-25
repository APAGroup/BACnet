using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BACnetAPA.EventArgs;

namespace BACnetAPA
{
    public sealed class BacnetWriteData : IBacnetRequest
    {
        public BacnetWriteData(BacnetItemInfo client, BacnetDataEventArgs data)
        {
            Client = client;
            Data = data;
        }

        public BacnetItemInfo Client { get; }
        public BacnetDataEventArgs Data { get; }
        public bool ReadRequest => false;
    }


}