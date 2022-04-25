using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BACnetAPA.EventArgs;

namespace BACnetAPA
{
    /// <summary>
    /// Bacnet client interface
    /// </summary>
    public interface IBacnetServerClient : IDisposable
    {
        /// <summary>
        /// Connection event
        /// </summary>
        event EventHandler<BacnetConnectionEventArgs> OnConnectionChange;

        /// <summary>
        /// Event on new data read
        /// </summary>
        event EventHandler<BacnetAggregationDataEventArgs> OnDataRead;

        /// <summary>
        /// Error event
        /// </summary>
        event EventHandler<ErrorEventArgs> OnErrorOccured;

        /// <summary>
        /// Starts the client
        /// </summary>
        void Run();

        /// <summary>
        /// Writes new data to bacnet network
        /// </summary>
        /// <param name="client">Client device info</param>
        /// <param name="data">Data to write</param>
        /// <returns></returns>
        bool WriteToBacnet(BacnetItemInfo client, BacnetDataEventArgs data);
    }

    public sealed class BacnetConnectionEventArgs : System.EventArgs
    {
        public BacnetConnectionEventArgs(bool connectionState)
        {
            ConnectionState = connectionState;
        }

        public bool ConnectionState { get; }
    }

    public class BacnetAggregationDataEventArgs
    {
        public BacnetAggregationDataEventArgs(BacnetAggregationData data, BacnetItemInfo item)
        {
            Data = data;
            Item = item;
        }

        public BacnetAggregationData Data { get; }

        public BacnetItemInfo Item { get; }
    }
}
