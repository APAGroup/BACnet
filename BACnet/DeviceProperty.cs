using System.IO.BACnet;

namespace BACnetAPA
{
    public sealed class DeviceProperty
    {
        public DeviceProperty(uint instance, int type)
        {
            Instance = instance;
            ObjectType = type;
            ObjectId = new BacnetObjectId((BacnetObjectTypes)type, instance);
        }

        public uint Instance { get; }

        public BacnetObjectId ObjectId { get; set; }
        public int ObjectType { get; set; }
        public object PresentValue { get; set; }
        public string PropertyDescription { get; set; }
        public string PropertyIdentifier { get; set; }
        public string PropertyName { get; set; }
        public int ValueType { get; set; }
    }
}

   
