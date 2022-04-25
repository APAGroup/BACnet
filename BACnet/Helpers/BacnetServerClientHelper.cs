using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.IO.BACnet.Helpers;
using System.Linq;
using System.Threading;
using BACnetAPA.Enums;
using BACnetAPA.EventArgs;

namespace BACnetAPA
{ 
    public static class BacnetServerClientHelper
	{
		public const int MaxQueueSize = 100000;

		[CLSCompliant(false)]
		public static void BacnetStatusToQuality(BacnetStatusFlags status, out bool inAlarm, out bool overriden, out bool outOfService, out bool fault)
		{
			if (status == 0)
			{
				fault = false;
				inAlarm = false;
				outOfService = false;
				overriden = false;
				return;
			}
			fault = (status & BacnetStatusFlags.STATUS_FLAG_FAULT) == BacnetStatusFlags.STATUS_FLAG_FAULT;
			inAlarm = (status & BacnetStatusFlags.STATUS_FLAG_IN_ALARM) == BacnetStatusFlags.STATUS_FLAG_IN_ALARM;
			outOfService = (status & BacnetStatusFlags.STATUS_FLAG_OUT_OF_SERVICE) ==
						   BacnetStatusFlags.STATUS_FLAG_OUT_OF_SERVICE;
			overriden = (status & BacnetStatusFlags.STATUS_FLAG_OVERRIDDEN) == BacnetStatusFlags.STATUS_FLAG_OVERRIDDEN;
		}

		[CLSCompliant(false)]
		public static void MakeCovSubscriptions(this BacnetClient bacnet, IEnumerable<BacnetItemInfo> items, BacnetSubscriptions notSubscribed, string traceId, CancellationToken token, EventHandler<ErrorEventArgs> errorHandler)
		{
			var copy = items.ToList();
			var idIndex = 0;

			foreach (var item in copy)
			{
				if (token.IsCancellationRequested)
				{
					return;
				}
				var adr = new BacnetAddress(BacnetAddressTypes.IP, item.IpAddress);
				var objectId = new BacnetObjectId((BacnetObjectTypes)item.PropertyType, (uint)item.PropertyInstanceId);
				try
				{
					if (!BacnetItemInfoHelper.CanSubscribe(item.PropertyType))
					{
						continue;
					}
					if (!bacnet.SubscribeCOVRequest(adr, objectId, 0, false, false, (uint)item.SubscriptionInterval))
					{
						notSubscribed.Add(new SimpleSubsription(item, idIndex));
						idIndex++;
						errorHandler?.Invoke(null, new ErrorEventArgs($"[{traceId}] Bacnet property does not support COV: {item.IpAddress} - {item.PropertyIdentifier}", $"[{traceId}] Bacnet property does not support COV: {item.IpAddress} - {item.PropertyIdentifier}", ErrorCodes.Warning));
                    }
					else
					{
						errorHandler?.Invoke(null, new ErrorEventArgs(string.Empty, $"[{traceId}] Bacnet property: {item.IpAddress} - {item.PropertyIdentifier} COV successfull.", ErrorCodes.InfoOptional));
                    }
				}
				catch (Exception ex)
				{
					notSubscribed.Add(new SimpleSubsription(item, idIndex));
					idIndex++;
					errorHandler?.Invoke(null, new ErrorEventArgs($"[{traceId}] COV subscription error : {ex.Message} for {item}", $"[{traceId}] COV subscription error : {ex.Message} for {item}", ErrorCodes.Error));
                }
			}
		}

		[CLSCompliant(false)]
		public static BacnetAggregationData Read(this BacnetClient bacnet, BacnetReadData data, string traceId, EventHandler<ErrorEventArgs> errorHandler) 
		{
			try
			{
				var adr = new BacnetAddress(BacnetAddressTypes.IP, data.Client.IpAddress);
				var objectId = new BacnetObjectId((BacnetObjectTypes)data.Client.PropertyType,
					(uint)data.Client.PropertyInstanceId);

				var statuses = bacnet.ReadPropertyAsync(adr, objectId, BacnetPropertyIds.PROP_STATUS_FLAGS).Result;
				var fault = false;
				var overriden = false;
				var outOfService = false;
				var inAlarm = false;
				if (statuses.Count > 0)
				{
					var status = (BacnetStatusFlags)((BacnetBitString)statuses.First().Value).ConvertToInt();
					BacnetStatusToQuality(status, out inAlarm, out overriden, out outOfService, out fault);
				}

				var values = bacnet.ReadPropertyAsync(adr, objectId, BacnetPropertyIds.PROP_PRESENT_VALUE).Result;

				if (values.Count == 1)
				{
					return new BacnetAggregationData(values.First().Value, fault, inAlarm, outOfService, overriden);
				}
			}
			catch (AggregateException ex)
			{
				foreach (var item in ex.InnerExceptions)
				{
					errorHandler?.Invoke(null, new ErrorEventArgs($"[{traceId}] Read error {data.Client.IpAddress} - {data.Client.PropertyIdentifier}: {item.Message}", $"[{traceId}] {item.Message}", ErrorCodes.Error));
                }
			}
			catch (Exception ex)
			{
				errorHandler?.Invoke(null, new ErrorEventArgs($"[{traceId}] Read error {data.Client.IpAddress} - {data.Client.PropertyIdentifier}: {ex.Message}", $"[{traceId}] {ex.Message}", ErrorCodes.Error));
            }
			return new BacnetAggregationData(null, true, false, true, false);
		}

		[CLSCompliant(false)]
		public static BacnetAggregationData ReadMultiple(this BacnetClient bacnet, BacnetReadData data, string traceId, EventHandler<ErrorEventArgs> errorHandler)
		{
			try
			{
				var adr = new BacnetAddress(BacnetAddressTypes.IP, data.Client.IpAddress);
				var objectId = new BacnetObjectId((BacnetObjectTypes)data.Client.PropertyType, (uint)data.Client.PropertyInstanceId);

				var results = bacnet.ReadPropertyMultipleAsync(adr, objectId, BacnetPropertyIds.PROP_PRESENT_VALUE,
					BacnetPropertyIds.PROP_STATUS_FLAGS).Result;
				if (results.Count == 2)
				{
					var status = results.FirstOrDefault(a =>
						a.property.GetPropertyId() == BacnetPropertyIds.PROP_STATUS_FLAGS);

					var statusvalue = (BacnetStatusFlags)((BacnetBitString)status.value.First().Value).ConvertToInt();
					BacnetStatusToQuality(statusvalue, out var inAlarm, out var overriden, out var outOfService, out var fault);
					var value = results.FirstOrDefault(a =>
						a.property.GetPropertyId() == BacnetPropertyIds.PROP_PRESENT_VALUE);

					return new BacnetAggregationData(value.value.First().Value, fault, inAlarm, outOfService,
						overriden);
				}
			}
			catch (AggregateException ex)
			{
				foreach (var item in ex.InnerExceptions)
				{
					errorHandler?.Invoke(null, new ErrorEventArgs($"[{traceId}] Read multiple error {data.Client.IpAddress} - {data.Client.PropertyIdentifier}: {item.Message}", $"[{traceId}] {item.Message}", ErrorCodes.Error));
                }
			}
			catch (Exception ex)
			{
                errorHandler?.Invoke(null, new ErrorEventArgs($"[{traceId}] Read multiple error {data.Client.IpAddress} - {data.Client.PropertyIdentifier}: {ex.Message}", $"[{traceId}] {ex.Message}", ErrorCodes.Error));
            }
			return new BacnetAggregationData(null, true, false, true, false);
		}

		[CLSCompliant(false)]
		public static void ReMakeCovSubscriptions(this BacnetClient bacnet, int deviceId,
			IEnumerable<BacnetItemInfo> items, BacnetSubscriptions notSubscribed, string traceId, CancellationToken token, EventHandler<ErrorEventArgs> errorHandler) 
		{
			var copy = items.Where(a => a.DeviceId == deviceId).ToList();
			var idIndex = 0;
			notSubscribed.RemoveByDevice(deviceId);

			foreach (var item in copy)
			{
				if (token.IsCancellationRequested)
				{
					return;
				}
				var adr = new BacnetAddress(BacnetAddressTypes.IP, item.IpAddress);
				var objectId = new BacnetObjectId((BacnetObjectTypes)item.PropertyType, (uint)item.PropertyInstanceId);
				try
				{
					if (!BacnetItemInfoHelper.CanSubscribe(item.PropertyType))
					{
						continue;
					}
					if (!bacnet.SubscribeCOVRequest(adr, objectId, 0, false, false, (uint)item.SubscriptionInterval))
					{
						notSubscribed.Add(new SimpleSubsription(item, idIndex));
						idIndex++;
						errorHandler?.Invoke(null, new ErrorEventArgs($"[{traceId}] Bacnet property does not support COV: {item.IpAddress} - {item.PropertyIdentifier}", $"[{traceId}] Bacnet property does not support COV: {item.IpAddress} - {item.PropertyIdentifier}", ErrorCodes.Warning));
                    }
					else
					{
						errorHandler?.Invoke(null, new ErrorEventArgs(string.Empty, $"[{traceId}] Bacnet property: {item.IpAddress} - {item.PropertyIdentifier} COV successfull.", ErrorCodes.InfoOptional));
                    }
				}
				catch (Exception ex)
				{
					notSubscribed.Add(new SimpleSubsription(item, idIndex));
					idIndex++;
					errorHandler?.Invoke(null, new ErrorEventArgs($"[{traceId}] COV subscription error : {ex.Message} for {item}", $"[{traceId}] COV subscription error : {ex.Message} for {item}", ErrorCodes.Error));

				}
			}
		}

		[CLSCompliant(false)]
		public static BacnetAggregationData Write(this BacnetClient bacnet, BacnetWriteData data, string traceId, EventHandler<ErrorEventArgs> errorHandler) 
		{
			try
			{
				var values = new List<BacnetValue>();

				switch (data.Client.DataType)
				{
					case Flavor.Bool:
						values.Add(TranslateBoolToBacnetType((BacnetApplicationTags)data.Client.PropertyDataType, (bool)data.Data.Data));
						break;

					case Flavor.Num:

						values.Add(GetNumericValue(data.Client.PropertyDataType, data.Data.Data));
						break;

					case Flavor.Text:
						values.Add(new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_CHARACTER_STRING, (string)data.Data.Data));
						break;
				}

				var adr = new BacnetAddress(BacnetAddressTypes.IP, data.Client.IpAddress);
				var objectId = new BacnetObjectId((BacnetObjectTypes)data.Client.PropertyType,
					(uint)data.Client.PropertyInstanceId);
				if (values.HasError())
				{
					errorHandler?.Invoke(null, new ErrorEventArgs(string.Empty, $"[{traceId}] Values to write has errors : {adr} {objectId}", ErrorCodes.Error));
                    return new BacnetAggregationData(null, true, false, true, false);
				}
				if (!bacnet.WritePropertyRequest(adr, objectId,
					BacnetPropertyIds.PROP_PRESENT_VALUE, values))
				{
					errorHandler?.Invoke(null, new ErrorEventArgs($"[{traceId}] Write error {adr} - {objectId}", $"[{traceId}] Write error to : {adr} {objectId}", ErrorCodes.Error));
                    return new BacnetAggregationData(null, true, false, true, false);
				}
				return new BacnetAggregationData(null, false, false, false, false);
			}
			catch (Exception ex)
			{
				errorHandler?.Invoke(null, new ErrorEventArgs($"[{traceId}] Write error {data.Client.IpAddress} - {data.Client.PropertyIdentifier}: {ex.Message}", $"[{traceId}] {ex}", ErrorCodes.Error));
                return new BacnetAggregationData(null, true, false, true, false);
			}
		}

		private static BacnetValue GetNumericValue(BacnetDataTypes type, object value)
		{
			switch (type)
			{
				case BacnetDataTypes.BacnetApplicationTagNull:
					return new BacnetValue(null);

				case BacnetDataTypes.BacnetApplicationTagBoolean:
					return new BacnetValue(Convert.ToBoolean(value));

				case BacnetDataTypes.BacnetApplicationTagUnsignedInt:
					return new BacnetValue(Convert.ToUInt32(value));

				case BacnetDataTypes.BacnetApplicationTagSignedInt:
					return new BacnetValue(Convert.ToInt32(value));

				case BacnetDataTypes.BacnetApplicationTagReal:
					return new BacnetValue(Convert.ToSingle(value));

				case BacnetDataTypes.BacnetApplicationTagDouble:
					return new BacnetValue(Convert.ToDouble(value));

				case BacnetDataTypes.BacnetApplicationTagOctetString:
					return new BacnetValue(Convert.ToString(value));

				case BacnetDataTypes.BacnetApplicationTagCharacterString:
					return new BacnetValue(Convert.ToString(value));

				default:
					return new BacnetValue(value);
			}
		}

		private static BacnetValue TranslateBoolToBacnetType(BacnetApplicationTags type, bool value)
		{
			if (type == BacnetApplicationTags.BACNET_APPLICATION_TAG_ENUMERATED)
			{
				if (value)
				{
					return new BacnetValue(type, 1);
				}
				return new BacnetValue(type, 0);
			}
			return new BacnetValue(BacnetApplicationTags.BACNET_APPLICATION_TAG_BOOLEAN, value);
		}
	}
}
