namespace BACnetAPA.EventArgs
{
    public sealed class BacnetDataEventArgs
    { 
        public BacnetDataEventArgs(object data, string senderIdentifier) 
        {

            Data = data;
            SenderIdentifier = senderIdentifier;
        }
        public object Data { get; private set; }

        public string SenderIdentifier { get; private set; }
    }

}
