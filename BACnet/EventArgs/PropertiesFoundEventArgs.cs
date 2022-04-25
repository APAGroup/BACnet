using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACnetAPA.EventArgs
{
    public class PropertiesFoundEventArgs : System.EventArgs
    {
        public PropertiesFoundEventArgs(IEnumerable<DeviceProperty> properties, bool aborted)
        {
            Properties = properties;
            Aborted = aborted;
        }

        public bool Aborted { get; }
        public IEnumerable<DeviceProperty> Properties { get; }
    }
}
