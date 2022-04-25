using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BACnetAPA.Enums;
using BACnetAPA.EventArgs;

namespace BACnetAPA
{
	public sealed class BacnetNetworkScanner : IScanner
	{
		private const int WaitEventInterval = 1000000;
		private readonly BacnetClient _bacnet;
		private readonly ManualResetEvent _event;
		private bool _abort, _scanning;

		private bool _disposed;

		public BacnetNetworkScanner(int scanPort, string localEndpoint)
		{
			var transport = new BacnetIpUdpProtocolTransport(scanPort, false, false, 1472, localEndpoint);

			_bacnet = new BacnetClient(transport);
			_event = new ManualResetEvent(true);

			_bacnet.OnIam += OnIAm;

			_bacnet.Start();
		}

		~BacnetNetworkScanner()
		{
			Dispose(false);
		}

		public event EventHandler<DeviceFoundEventArgs> OnFoundDevice;

		public event EventHandler<PropertiesFoundEventArgs> OnPropertyFound;

        public event EventHandler<ErrorEventArgs> OnErrorOccured;

		public event EventHandler<BacnetSearchProgressEventArgs> OnPropertyReadProgress;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Scan()
		{
			try
			{
				OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Search start: {DateTime.Now}", ErrorCodes.Info));
                _bacnet.WhoIs();
			}
			catch (Exception ex)
			{
                OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Search start error: {ex}", ErrorCodes.Error));
            }
		}

		public void ScanProperties(string ip, int deviceId, bool segmentation)
		{
			try
			{
				Task.Run(async () =>
				{
					try
					{
						_abort = _scanning;
						_event.WaitOne(WaitEventInterval);
						if (_disposed)
						{
							return;
						}
						_event.Reset();
						_abort = false;
						_scanning = true;
						try
						{
							var adr = new BacnetAddress(BacnetAddressTypes.IP, ip);
							var deviceObjId = new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, (uint)deviceId);
							var properties = new List<DeviceProperty>();
							await ReadAllProperties(_bacnet, adr, deviceObjId, properties, segmentation);
							if (!_abort)
							{
								OnPropertyFound?.Invoke(this, new PropertiesFoundEventArgs(properties, false));
							}
							else
							{
								OnPropertyFound?.Invoke(this,
									new PropertiesFoundEventArgs(new List<DeviceProperty>(), true));
							}
						}
						finally
						{
							_scanning = false;
							if (!_disposed)
							{
								_event.Set();
							}
						}
					}
					catch (Exception)
					{
						//Na wszelki wypadek gdyby byl spory timeout.
					}
				});
			}
			catch (Exception ex)
			{
				OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Properties scan ({ip}) Exception: {ex}", ErrorCodes.Error));
            }
		}

		private async Task ReadAllDeviceData(BacnetClient sender, BacnetAddress adr, BacnetObjectId deviceId,
											DeviceFoundEventArgs deviceIngfo)
		{
			try
			{
				var name = await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_OBJECT_NAME);
				if (name.Any())
				{
					deviceIngfo.Name = (string)name.First().Value;
				}
				var objType = await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_OBJECT_TYPE);
				if (objType.Any())
				{
					deviceIngfo.ObjectType = (BacnetObjectTypes)(objType.First().Value);
				}
				var model = await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_MODEL_NAME);
				if (model.Any())
				{
					deviceIngfo.ModelName = (string)model.First().Value;
				}

				var vendor = await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_VENDOR_NAME);
				if (vendor.Any())
				{
					deviceIngfo.VendorName = (string)vendor.First().Value;
				}
				var segmentationSupported = await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_SEGMENTATION_SUPPORTED);
				if (segmentationSupported.Any())
				{
					deviceIngfo.Segmentation = (uint)segmentationSupported.First().Value;
				}
				var status = await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_SYSTEM_STATUS);
				if (status.Any())
				{
					deviceIngfo.Status = (uint)status.First().Value;
				}
			}
			catch (Exception e)
			{
				OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Error in device data ({adr} read: {e}", ErrorCodes.Error));
            }
		}

		private async Task ReadSingleProperty(BacnetClient sender, BacnetAddress adr, BacnetObjectId deviceId,
			DeviceProperty property)
		{
			try
			{
				try
				{
					var name = await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_OBJECT_NAME);
					if (name.Any())
					{
						property.PropertyName = (string)name.First().Value;
					}
				}
				catch (Exception)
				{
					OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Property OBJECT NAME is not available in ({adr}).", ErrorCodes.Error));
                }
				try
				{
					var objType = await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_OBJECT_TYPE);
					if (objType.Any())
					{
						property.ObjectType = (int)objType.First().Value;
					}
				}
				catch (Exception)
				{
                    OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Property OBJECT TYPE is not available in ({adr}).", ErrorCodes.Error));
                }
				try
				{
					var model = await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_DESCRIPTION);
					if (model.Any())
					{
						property.PropertyDescription = (string)model.First().Value;
					}
				}
				catch (Exception)
				{
                    OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Property DESCRIPTION is not available in ({adr}).", ErrorCodes.Error));
                }
				try
				{
					var vendor =
						await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_OBJECT_IDENTIFIER);
					if (vendor.Any())
					{
						property.PropertyIdentifier = ((BacnetObjectId)vendor.First().Value).ToString();
					}
				}
				catch (Exception)
				{
                    OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Property OBJECT IDENTIFIER is not available in ({adr}).", ErrorCodes.Error));
                }

				try
				{
					var status = await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_PRESENT_VALUE);
					if (status.Any())
					{
						property.PresentValue = status.First().Value;
						property.ValueType = (int)status.First().Tag;
					}
				}
				catch (Exception)
				{
                    OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Property PRESENT VALUE is not available in ({adr}).", ErrorCodes.Error));
                }
			}
			catch (Exception e)
			{
                OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Exception for ({adr}) : {e}", ErrorCodes.Error));
            }
		}

		private void Dispose(bool disposing)
		{
			if (!disposing)
			{ return; }
			if (!_disposed)
			{
				_bacnet.OnIam -= OnIAm;
				_bacnet.Dispose();
				_event.Set();
				_event.Dispose();
			}
			_disposed = true;
		}

		private async void OnIAm(BacnetClient sender, BacnetAddress adr, uint deviceid, uint maxapdu, BacnetSegmentations segmentation, ushort vendorid)
		{
			try
			{
				var deviceObjId = new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, deviceid);

				var device = new DeviceFoundEventArgs(deviceid, $"{adr}") { AddressBacnet = adr };

				await ReadAllDeviceData(sender, adr, deviceObjId, device);

				OnFoundDevice?.Invoke(this, device);
			}
			catch (Exception ex)
			{
                OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"OnIAM ({adr}) Exception: {ex}", ErrorCodes.Error));
            }
		}

		private async Task ReadAllProperties(BacnetClient sender, BacnetAddress adr, BacnetObjectId deviceId,
									ICollection<DeviceProperty> list, bool indexed)
		{
			try
			{
				if (indexed)
				{
					await Task.Run(() => ReadAllPropertiesIndexed(sender, adr, deviceId, list));
					return;
				}

				var objectIdList = await sender.ReadPropertyAsync(adr, deviceId, BacnetPropertyIds.PROP_OBJECT_LIST);
				var index = 0;
				OnPropertyReadProgress?.Invoke(this, new BacnetSearchProgressEventArgs(objectIdList.Count, 0));
				foreach (var objId in objectIdList)
				{
					if (_abort)
					{
						break;
					}
					index++;
					var item = (BacnetObjectId)objId.Value;
					var property = new DeviceProperty(item.Instance, (int)item.Type);
					await ReadSingleProperty(sender, adr, item, property);
					OnPropertyReadProgress?.Invoke(this, new BacnetSearchProgressEventArgs(objectIdList.Count, index));
					list.Add(property);
				}
			}
			catch (Exception e)
			{
                OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Error in device properties ({adr}) read: {e}. Trying to read indexed.", ErrorCodes.Error));
                if (e.Message.Equals("Abort from device: 4"))
				{
					await Task.Run(() => ReadAllPropertiesIndexed(sender, adr, deviceId, list));
				}
			}
		}

		private async Task ReadAllPropertiesIndexed(BacnetClient sender, BacnetAddress adr,
			BacnetObjectId deviceId,
			ICollection<DeviceProperty> list)
		{
			try
			{
				var propertyId = BacnetPropertyIds.PROP_OBJECT_LIST;
				sender.ReadPropertyRequest(adr, deviceId, propertyId, out var valueList, arrayIndex: 0);
				var objectCount = valueList.First().As<uint>();
				OnPropertyReadProgress?.Invoke(this, new BacnetSearchProgressEventArgs(objectCount, 0));
				for (uint i = 1; i <= objectCount; i++)
				{
					if (_abort)
					{
						break;
					}
					sender.ReadPropertyRequest(adr, deviceId, propertyId, out var nextvalueList, arrayIndex: i);
					var objectId = (nextvalueList.First().As<BacnetObjectId>());
					if (objectId.Type == BacnetObjectTypes.OBJECT_DEVICE)
					{
                        OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Found object DEVICE: {objectId}", ErrorCodes.Warning));
                        continue;
					}
					try
					{
						var property = new DeviceProperty(objectId.Instance, (int)objectId.Type);
						await ReadSingleProperty(sender, adr, objectId, property);
						OnPropertyReadProgress?.Invoke(this, new BacnetSearchProgressEventArgs(objectCount, i));
						list.Add(property);
					}
					catch (Exception ex)
					{
                        OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Exception for : {adr} on object: {objectId} : {ex.Message}", ErrorCodes.Error));
                    }
				}
			}
			catch (Exception e)
			{
                OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"Error in device properties ({adr}) read: {e}. Trying to read indexed.", ErrorCodes.Error));
            }
		}
	}
}
