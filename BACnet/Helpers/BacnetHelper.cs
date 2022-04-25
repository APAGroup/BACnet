using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.IO.BACnet.Storage;
using Object = System.IO.BACnet.Storage.Object;

namespace BACnetAPA.Helpers
{
    public static class BacnetHelper
    {
        public const int VendorId = 1999;
        public const int WaitCycle = 100;
        private const string ModelName = "Nazca";
        private const string VendorName = "APA Sp z o.o.";

        [CLSCompliant(false)]
        public static DeviceStorage CreateStorage(int deviceId, string name)
        {
            var deviceStorage = new DeviceStorage();
            var deviceObject = new Object
            {
                Type = BacnetObjectTypes.OBJECT_DEVICE,
                Instance = (uint)deviceId
            };
            var props = new List<Property>();
            try
            {
                props.AddProperty(BacnetPropertyIds.PROP_OBJECT_IDENTIFIER,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID, $"OBJECT_DEVICE:{deviceId}");
                props.AddProperty(BacnetPropertyIds.PROP_OBJECT_LIST,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_OBJECT_ID, $"OBJECT_DEVICE:{deviceId}");
                props.AddProperty(BacnetPropertyIds.PROP_OBJECT_NAME,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_CHARACTER_STRING, $"{name}");
                props.AddProperty(BacnetPropertyIds.PROP_OBJECT_TYPE,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED, "8");
                props.AddProperty(BacnetPropertyIds.PROP_DESCRIPTION,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_CHARACTER_STRING, $"{name}");
                props.AddProperty(BacnetPropertyIds.PROP_VENDOR_NAME,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_CHARACTER_STRING, $"{VendorName}");
                props.AddProperty(BacnetPropertyIds.PROP_VENDOR_IDENTIFIER,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT, $"{VendorId}");
                props.AddProperty(BacnetPropertyIds.PROP_MODEL_NAME,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_CHARACTER_STRING, $"{ModelName}");
                props.AddProperty(BacnetPropertyIds.PROP_SYSTEM_STATUS,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED, "0");
                props.AddProperty(BacnetPropertyIds.PROP_FIRMWARE_REVISION,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_CHARACTER_STRING, "1.0");
                props.AddProperty(BacnetPropertyIds.PROP_APPLICATION_SOFTWARE_VERSION,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_CHARACTER_STRING, "1.0");
                props.AddProperty(BacnetPropertyIds.PROP_PROTOCOL_VERSION,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT, "1");
                props.AddProperty(BacnetPropertyIds.PROP_PROTOCOL_REVISION,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT, "1");
                props.AddProperty(BacnetPropertyIds.PROP_PROTOCOL_SERVICES_SUPPORTED,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_BIT_STRING, "0000000000001000000000000010000000100000");
                props.AddProperty(BacnetPropertyIds.PROP_PROTOCOL_OBJECT_TYPES_SUPPORTED,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_BIT_STRING, "000000001000000000000000000000000000000000000000000");
                props.AddProperty(BacnetPropertyIds.PROP_MAX_APDU_LENGTH_ACCEPTED,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT, "1476");
                props.AddProperty(BacnetPropertyIds.PROP_SEGMENTATION_SUPPORTED,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED, "3");
                props.AddProperty(BacnetPropertyIds.PROP_NUMBER_OF_APDU_RETRIES,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT, "3000");
                props.AddProperty(BacnetPropertyIds.PROP_APDU_TIMEOUT,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT, "3000");

                props.AddProperty(BacnetPropertyIds.PROP_DEVICE_ADDRESS_BINDING,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_NULL, null);

                props.AddProperty(BacnetPropertyIds.PROP_DATABASE_REVISION,
                    BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT, "0");

                deviceObject.Properties = props.ToArray();
                deviceStorage.DeviceId = (uint)deviceId;
                deviceStorage.Objects = (new List<Object> { deviceObject }).ToArray();
            }
            catch (Exception ex)
            {
               // Log.Bacnet.Error($"{ex}");
                return null;
            }
            return deviceStorage;
        }

        [CLSCompliant(false)]
        public static string GetIpFromAddress(this BacnetAddress address)
        {
            var complete = $"{address}";
            var pair = complete.Split(':');

            return pair[0];
        }

        private static void AddProperty(this ICollection<Property> items, BacnetPropertyIds id, BacnetApplicationTags tag,
                    string value)
        {
            items.Add(new Property
            {
                Id = id,
                Tag = tag,
                Value = new[] { value }
            });
        }
    }
}
