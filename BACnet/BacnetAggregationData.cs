namespace BACnetAPA
{ 
    public class BacnetAggregationData
    {
        public BacnetAggregationData(object item, bool fault, bool inalarm, bool outOfService, bool overriden)
        {
            Item = item;
            Fault = fault;
            InAlarm = inalarm;
            OutOfService = outOfService;
            Overriden = overriden;
        }

        public BacnetAggregationData(object item)
        {
            Item = item;
            Fault = false;
            InAlarm = false;
            OutOfService = false;
            Overriden = false;
        }

        public bool Fault { get; }
        public bool InAlarm { get; }
        public object Item { get; }
        public bool OutOfService { get; }

        public bool Overriden { get; }
    }
}
