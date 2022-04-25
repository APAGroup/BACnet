using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BACnetAPA.EventArgs;

namespace BACnetAPA
{
    [CLSCompliant(false)]
    public interface IScanner : IDisposable
    {
        /// <summary>
        /// Event on new bacnet element discovery
        /// </summary>
        event EventHandler<DeviceFoundEventArgs> OnFoundDevice;

        /// <summary>
        /// Event on new bacnet element discovery
        /// </summary>
        event EventHandler<PropertiesFoundEventArgs> OnPropertyFound;

        /// <summary>
        /// Event on new bacnet element discovery progress
        /// </summary>
        event EventHandler<BacnetSearchProgressEventArgs> OnPropertyReadProgress;

        /// <summary>
        /// Error event
        /// </summary>
        event EventHandler<ErrorEventArgs> OnErrorOccured;

        /// <summary>
        /// Scans network for devices
        /// </summary>
        void Scan();

        /// <summary>
        /// Scans network for devices
        /// </summary>
        void ScanProperties(string ip, int deviceId, bool segmentation);
    }
}
