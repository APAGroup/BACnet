namespace BACnetAPA.EventArgs
{
    public sealed class BacnetSearchProgressEventArgs : System.EventArgs
    {
        public BacnetSearchProgressEventArgs(double count, double actualRead)
        {
            Count = count;
            ActualRead = actualRead;
        }

        public double ActualRead { get; }
        public double Count { get; }
    }
}