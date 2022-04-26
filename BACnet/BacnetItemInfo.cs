using System;
using BACnetAPA.Enums;

namespace BACnetAPA
{ 
    public class BacnetItemInfo : IEquatable<BacnetItemInfo>
    {
        public BacnetItemInfo()
        {
            //for serialization
        }

        public BacnetItemInfo(string ipAddress, int deviceId, string modelName, string vendorName,
            string propertyIdentifier, string propertyName, int propertyInstanceId, int propertyType,
            int propertyDataType, string propertyDescription,
            int subsriptionInterval)
        {
            DeviceId = deviceId;
            IpAddress = ipAddress;
            ModelName = modelName;
            VendorName = vendorName;
            PropertyName = propertyName;
            PropertyDescription = propertyDescription;
            PropertyIdentifier = propertyIdentifier;
            PropertyInstanceId = propertyInstanceId;
            PropertyType = (BacnetPropertyType)propertyType;
            Direction = BacnetItemInfoHelper.BacnetPropertyTypeToDirection((BacnetPropertyType)propertyType);
            DataType = BacnetItemInfoHelper.BacnetPropertyTypeToFlavor((BacnetPropertyType)propertyType);
            SubscriptionInterval = subsriptionInterval;
            PropertyDataType = (BacnetDataTypes)propertyDataType;
        }

        public BacnetItemInfo(BacnetItemInfo item)
        {
            DeviceId = item.DeviceId;
            IpAddress = item.IpAddress;
            ModelName = item.ModelName;
            VendorName = item.VendorName;
            PropertyName = item.PropertyName;
            PropertyDescription = item.PropertyDescription;
            PropertyIdentifier = item.PropertyIdentifier;
            PropertyInstanceId = item.PropertyInstanceId;
            PropertyType = item.PropertyType;
            Direction = BacnetItemInfoHelper.BacnetPropertyTypeToDirection(PropertyType);
            DataType = BacnetItemInfoHelper.BacnetPropertyTypeToFlavor(PropertyType);
            SubscriptionInterval = item.SubscriptionInterval;
            PropertyDataType = item.PropertyDataType;
        }
        
        public int DataType { get; set; }

        public int DeviceId { get; set; }
        
        public int Direction { get; set; }

        public string Identifier => $"{IpAddress}:{DeviceId}:{PropertyIdentifier}".GetHashCode().ToString();

        public string IpAddress { get; set; }

        public string ModelName { get; set; }

        public BacnetDataTypes PropertyDataType { get; set; }

        public string PropertyDescription { get; set; }

        public string PropertyIdentifier { get; set; }

        public int PropertyInstanceId { get; set; }

        public string PropertyName { get; set; }

        public BacnetPropertyType PropertyType { get; set; }

        public int SubscriptionInterval { get; set; }

        public string VendorName { get; set; }

        public bool Equals(BacnetItemInfo other)
        {
            return other != null && RelevantDataEqual(other);
        }

        public override string ToString()
        {
            return $"{IpAddress}, {PropertyInstanceId}";
        }

        private bool RelevantDataEqual(BacnetItemInfo other)
            => IpAddress.Equals(other.IpAddress, StringComparison.Ordinal) &&
               PropertyIdentifier.Equals(other.PropertyIdentifier, StringComparison.Ordinal) &&
               PropertyName.Equals(other.PropertyName, StringComparison.Ordinal) &&
               PropertyType == other.PropertyType && PropertyInstanceId == other.PropertyInstanceId &&
               DeviceId == other.DeviceId;
    }
}