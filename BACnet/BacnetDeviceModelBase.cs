using System.IO.BACnet;
using System.Runtime.CompilerServices;

namespace BACnetAPA
{ 
    [CompilerGenerated]
    public class BacnetDeviceModelBase : NotifyPropertyChangedBase
    {
        public BacnetDeviceModelBase(int segmentation)
        {
            Segmentation = (BacnetSegmentations)segmentation;
        }

        public BacnetSegmentations Segmentation { get; set; }

    }
}
