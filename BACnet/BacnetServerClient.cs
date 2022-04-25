using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.BACnet;
using System.IO.BACnet.Storage;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BACnetAPA.Enums;
using BACnetAPA.EventArgs;
using BACnetAPA.Helpers;

namespace BACnetAPA
{
    public sealed class BacnetServerClient : IBacnetServerClient
    {
        private const int QueueSize = 100000;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly uint _deviceId;
        private readonly DeviceStorage _deviceStorage;
        private readonly ManualResetEvent _disposeEvent;
        private readonly ManualResetEvent _event;
        private readonly IEnumerable<BacnetItemInfo> _items;
        private readonly string _localEndpoint;
        private readonly BacnetDevicesStatus _monitoredDevices;
        private readonly int _port;
        private readonly BlockingCollection<IBacnetRequest> _requestQueue;
        private readonly BacnetSubscriptions _subscriptions;
        private readonly string _traceId;
        private bool _disposed;

        public BacnetServerClient(int port, string localEndpoint, int deviceId, string name, IEnumerable<BacnetItemInfo> items, string traceId)
        {
            _subscriptions = new BacnetSubscriptions();
            _cancellationTokenSource = new CancellationTokenSource();
            _subscriptions.OnSubscription += SubscriptionsOnOnSubscription;
            _traceId = traceId;
            _items = items.ToList();
            _port = port;
            _localEndpoint = localEndpoint;
            _event = new ManualResetEvent(false);
            _disposeEvent = new ManualResetEvent(false);
            _requestQueue = new BlockingCollection<IBacnetRequest>();
            _deviceId = (uint)deviceId;
            _deviceStorage = BacnetHelper.CreateStorage(deviceId, name);
            _monitoredDevices = new BacnetDevicesStatus(_items);
            _monitoredDevices.OnDeviceCouldBeRestarted += MonitoredDevicesOnOnDeviceCouldBeRestarted;
        }

        ~BacnetServerClient()
        {
            Dispose(false);
        }

        public event EventHandler<BacnetConnectionEventArgs> OnConnectionChange;

        public event EventHandler<BacnetAggregationDataEventArgs> OnDataRead;

        public event EventHandler<ErrorEventArgs> OnErrorOccured;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Run()
        {
            Task.Run(ServerTask);
        }

        public bool WriteToBacnet(BacnetItemInfo client, BacnetDataEventArgs data)
        {
            if (_requestQueue.Count < QueueSize)
            {
                _requestQueue.Add(new BacnetWriteData(client, data));
                return true;
            }
            return false;
        }

        private void BacnetOnCovNotfiedItem(BacnetPropertyValue value, BacnetItemInfo client)
        {
            switch ((BacnetPropertyIds)value.property.propertyIdentifier)
            {
                case BacnetPropertyIds.PROP_PRESENT_VALUE:
                    if (value.value != null)
                    {
                        OnDataRead?.Invoke(this,
                            new BacnetAggregationDataEventArgs(new BacnetAggregationData(value.value[0]),
                                client));
                    }

                    break;

                case BacnetPropertyIds.PROP_STATUS_FLAGS:

                    if (value.value != null && value.value.Count > 0)
                    {
                        var status = (BacnetStatusFlags)((BacnetBitString)value.value[0].Value).ConvertToInt();
                        BacnetServerClientHelper.BacnetStatusToQuality(status, out var inAlarm, out var overriden, out var outOfService, out var fault);
                        OnDataRead?.Invoke(this,
                            new BacnetAggregationDataEventArgs(new BacnetAggregationData(null, fault, inAlarm, outOfService, overriden),
                                client));
                    }
                    break;
            }
        }

        private void BacnetOnOnCovNotification(BacnetClient sender, BacnetAddress adr, byte invokeid,
                    uint subscriberprocessidentifier, BacnetObjectId initiatingdeviceidentifier,
            BacnetObjectId monitoredobjectidentifier, uint timeremaining, bool needconfirm,
            ICollection<BacnetPropertyValue> values, BacnetMaxSegments maxsegments)
        {
            var address = $"{adr}";
            var propertyIdentifier = $"{monitoredobjectidentifier}";
            var client = _items.FirstOrDefault(a =>
                a.IpAddress.Equals(address) && a.PropertyIdentifier.Equals(propertyIdentifier));
            if (client == null)
            {
                if (monitoredobjectidentifier.Type == BacnetObjectTypes.OBJECT_DEVICE && CheckIfDeviceRestarted(sender, values, monitoredobjectidentifier))
                {
                    _monitoredDevices.UpdateIAm(sender, adr, true);
                }
                return;
            }
            foreach (var value in values)
            {
                BacnetOnCovNotfiedItem(value, client);
            }

            if (needconfirm)
            {
                sender.SimpleAckResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_COV_NOTIFICATION, invokeid);
            }
        }

        private void BacnetOnOnError(BacnetClient sender, BacnetAddress adr, BacnetPduTypes type, BacnetConfirmedServices service, byte invokeId, BacnetErrorClasses errorClass, BacnetErrorCodes errorCode, byte[] buffer, int offset, int length)
        {
            var text = _traceId + $" Device {adr} reported an error: {errorClass} with code: {errorCode}";
            OnErrorOccured?.Invoke(this, new ErrorEventArgs(text, text, ErrorCodes.Warning));
        }

        private void BacnetOnOnIam(BacnetClient sender, BacnetAddress adr, uint deviceid, uint maxapdu, BacnetSegmentations segmentation, ushort vendorid)
        {
            _monitoredDevices.UpdateIAm(sender, adr, false);
        }

        private void BacnetOnOnReadPropertyRequest(BacnetClient sender, BacnetAddress adr, byte invokeId,
                            BacnetObjectId objectId, BacnetPropertyReference property, BacnetMaxSegments maxSegments)
        {
            lock (_deviceStorage)
            {
                try
                {
                    var code = _deviceStorage.ReadProperty(objectId, (BacnetPropertyIds)property.propertyIdentifier,
                        property.propertyArrayIndex, out var value);
                    if (code == DeviceStorage.ErrorCodes.Good)
                        sender.ReadPropertyResponse(adr, invokeId, sender.GetSegmentBuffer(maxSegments), objectId,
                            property, value);
                    else
                        sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROPERTY, invokeId,
                            BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_OTHER);
                }
                catch (Exception ex)
                {
                    OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"[{_traceId}] {ex}", ErrorCodes.Error));
                    sender.ErrorResponse(adr, BacnetConfirmedServices.SERVICE_CONFIRMED_READ_PROPERTY, invokeId,
                        BacnetErrorClasses.ERROR_CLASS_DEVICE, BacnetErrorCodes.ERROR_CODE_OTHER);
                }
            }
        }

        private void BacnetOnOnWhoIs(BacnetClient sender, BacnetAddress adr, int lowlimit, int highlimit)
        {
            sender.Iam(_deviceId, new BacnetSegmentations());
        }

        private bool CheckIfDeviceRestarted(BacnetClient bacnet, ICollection<BacnetPropertyValue> values, BacnetObjectId monitoredobjectidentifier)
        {
            if (values.All(a => (BacnetPropertyIds)a.property.propertyIdentifier != BacnetPropertyIds.PROP_LAST_RESTART_REASON))
            { return false; }

            var restartInfo = values.FirstOrDefault(a =>
                (BacnetPropertyIds)a.property.propertyIdentifier == BacnetPropertyIds.PROP_LAST_RESTART_REASON);
            var restartTime = values.FirstOrDefault(a =>
                (BacnetPropertyIds)a.property.propertyIdentifier == BacnetPropertyIds.PROP_TIME_OF_DEVICE_RESTART);
            if (restartInfo.value.Count > 0)
            {
                var reason = restartInfo.value[0].As<BacnetRestartReason>();
                if (restartTime.value.Count > 0 && restartTime.value[0].Value is IEnumerable<BacnetValue> dtList)
                {
                    var dateTimes = (dtList).ToList();
                    var date = dateTimes.First().As<DateTime>().Date;
                    var time = dateTimes.Last().As<DateTime>().TimeOfDay;
                    OnErrorOccured?.Invoke(this, new ErrorEventArgs(string.Empty, $"[{_traceId}] Device - {monitoredobjectidentifier} - informed about restart casused by: {reason} at {date + time}. Reinitializing COVS...", ErrorCodes.Info));
                }
            }

            _subscriptions.Stop();
            bacnet.ReMakeCovSubscriptions((int)monitoredobjectidentifier.instance, _items, _subscriptions, _traceId, _cancellationTokenSource.Token, OnErrorOccured);
            _subscriptions.Run();
            return true;
        }

        private BacnetClient CreateClient()
        {
            var transport = new BacnetIpUdpProtocolTransport(_port, false, false, 1472, _localEndpoint);
            var bacnet = new BacnetClient(transport);
            bacnet.OnCOVNotification += BacnetOnOnCovNotification;
            bacnet.OnWhoIs += BacnetOnOnWhoIs;
            bacnet.VendorId = BacnetHelper.VendorId;
            bacnet.OnReadPropertyRequest += BacnetOnOnReadPropertyRequest;
            bacnet.OnError += BacnetOnOnError;
            bacnet.OnIam += BacnetOnOnIam;

            try
            {
                bacnet.Start();
            }
            catch (Exception ex)
            {
                var text = $"[{_traceId}] Initialization error: {ex}";
                OnErrorOccured?.Invoke(this, new ErrorEventArgs(text, text, ErrorCodes.Error));
                DisposeBacnet(ref bacnet);
            }
            return bacnet;
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            if (!_disposed)
            {
                _subscriptions.OnSubscription -= SubscriptionsOnOnSubscription;
                _requestQueue.CompleteAdding();
                _event.Set();
                _cancellationTokenSource.Cancel();
                _disposeEvent.WaitOne();
                _disposeEvent.Dispose();
                _subscriptions.Dispose();
                _cancellationTokenSource.Dispose();
            }
            _disposed = true;
        }

        private void DisposeBacnet(ref BacnetClient bacnet)
        {
            if (bacnet == null)
            {
                return;
            }
            bacnet.OnCOVNotification -= BacnetOnOnCovNotification;
            bacnet.OnWhoIs -= BacnetOnOnWhoIs;
            bacnet.VendorId = BacnetHelper.VendorId;
            bacnet.OnReadPropertyRequest -= BacnetOnOnReadPropertyRequest;
            bacnet.OnError -= BacnetOnOnError;
            bacnet.OnIam -= BacnetOnOnIam;
            bacnet.Dispose();
            bacnet = null;
        }

        private void MonitoredDevicesOnOnDeviceCouldBeRestarted(object sender, DeviceRestartInfoEventArgs e)
        {
            _subscriptions.Stop();
            e.Client.ReMakeCovSubscriptions(e.DeviceId, _items, _subscriptions, _traceId, _cancellationTokenSource.Token, OnErrorOccured);
            _subscriptions.Run();
        }

        private void ReadRequest(IBacnetRequest item, BacnetClient bacnet)
        {
            if (!(item is BacnetReadData source))
            {
                return;
            }
            var read = bacnet.ReadMultiple(source, _traceId, OnErrorOccured);
            if (read == null)
            {
                return;
            }
            _subscriptions.UpdateRead(source.SubscriptionId);
            OnDataRead?.Invoke(this, new BacnetAggregationDataEventArgs(read, source.Client));
        }

        private void ServerTask()
        {
            BacnetClient bacnet = null;

            try
            {
                while (!_requestQueue.IsAddingCompleted)
                {
                    if (!ServiceRunning(ref bacnet))
                    {
                        continue;
                    }
                    if (!_requestQueue.TryTake(out var item, BacnetHelper.WaitCycle))
                    {
                        continue;
                    }
                    if (item.ReadRequest)
                    {
                        ReadRequest(item, bacnet);
                        continue;
                    }
                    if (item is BacnetWriteData writeSource)
                    {
                        WriteRequest(bacnet, writeSource);
                    }
                }
            }
            finally
            {
                _requestQueue.Dispose();
                DisposeBacnet(ref bacnet);
                _disposeEvent.Set();
                _event.Dispose();
            }
        }

        private bool ServiceRunning(ref BacnetClient bacnet)
        {
            if (bacnet == null)
            {
                bacnet = CreateClient();
                if (bacnet != null)
                {
                    bacnet.MakeCovSubscriptions(_items, _subscriptions, _traceId, _cancellationTokenSource.Token, OnErrorOccured);
                    _subscriptions.Run();
                    OnConnectionChange?.Invoke(this, new BacnetConnectionEventArgs(true));
                }
                else
                {
                    OnConnectionChange?.Invoke(this, new BacnetConnectionEventArgs(false));
                    _event.WaitOne(5000);
                    return false;
                }
            }

            return true;
        }

        private void SubscriptionsOnOnSubscription(object sender, SubscriptionRefreshEventArgs args)
        {
            _requestQueue.Add(new BacnetReadData(args.Subscription.Item, args.Subscription.Id));
        }

        private void WriteRequest(BacnetClient bacnet, BacnetWriteData writeSource)
        {
            var write = bacnet.Write(writeSource, _traceId, OnErrorOccured);
            if (write != null)
            {
                OnDataRead?.Invoke(this, new BacnetAggregationDataEventArgs(write, writeSource.Client));
            }
        }
    }
}
