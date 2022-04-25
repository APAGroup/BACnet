using BACnetAPA.Enums;

namespace BACnetAPA
{
    public static class BacnetItemInfoHelper
    {
        public static bool BacnetPropertyTypeSupported(BacnetPropertyType type)
        {
            switch (type)
            {
                case BacnetPropertyType.ObjectBinaryInput:
                case BacnetPropertyType.ObjectBinaryOutput:
                case BacnetPropertyType.ObjectBinaryValue:
                case BacnetPropertyType.ObjectAnalogInput:
                case BacnetPropertyType.ObjectMultiStateInput:
                case BacnetPropertyType.ObjectAnalogOutput:
                case BacnetPropertyType.ObjectMultiStateOutput:
                case BacnetPropertyType.ObjectLightingOutput:
                case BacnetPropertyType.ObjectBinaryLightingOutput:
                case BacnetPropertyType.ObjectAnalogValue:
                case BacnetPropertyType.ObjectIntegerValue:
                case BacnetPropertyType.ObjectLargeAnalogValue:
                case BacnetPropertyType.ObjectPositiveIntegerValue:
                case BacnetPropertyType.ObjectMultiStateValue:
                case BacnetPropertyType.ObjectBitstringValue:
                case BacnetPropertyType.ObjectCharacterstringValue:
                    return true;

                case BacnetPropertyType.ObjectDatePatternValue:
                case BacnetPropertyType.ObjectDateValue:
                case BacnetPropertyType.ObjectDatetimePatternValue:
                case BacnetPropertyType.ObjectDatetimeValue:
                case BacnetPropertyType.ObjectOctetstringValue:
                case BacnetPropertyType.ObjectTimePatternValue:
                case BacnetPropertyType.ObjectTimeValue:
                case BacnetPropertyType.ObjectCalendar:
                case BacnetPropertyType.ObjectCommand:
                case BacnetPropertyType.ObjectDevice:
                case BacnetPropertyType.ObjectEventEnrollment:
                case BacnetPropertyType.ObjectFile:
                case BacnetPropertyType.ObjectGroup:
                case BacnetPropertyType.ObjectLoop:
                case BacnetPropertyType.ObjectNotificationClass:
                case BacnetPropertyType.ObjectProgram:
                case BacnetPropertyType.ObjectSchedule:
                case BacnetPropertyType.ObjectAveraging:
                case BacnetPropertyType.ObjectTrendlog:
                case BacnetPropertyType.ObjectLifeSafetyPoint:
                case BacnetPropertyType.ObjectLifeSafetyZone:
                case BacnetPropertyType.ObjectAccumulator:
                case BacnetPropertyType.ObjectPulseConverter:
                case BacnetPropertyType.ObjectEventLog:
                case BacnetPropertyType.ObjectGlobalGroup:
                case BacnetPropertyType.ObjectTrendLogMultiple:
                case BacnetPropertyType.ObjectLoadControl:
                case BacnetPropertyType.ObjectStructuredView:
                case BacnetPropertyType.ObjectAccessDoor:
                case BacnetPropertyType.ObjectTimer:
                case BacnetPropertyType.ObjectAccessCredential:
                case BacnetPropertyType.ObjectAccessPoint:
                case BacnetPropertyType.ObjectAccessRights:
                case BacnetPropertyType.ObjectAccessUser:
                case BacnetPropertyType.ObjectAccessZone:
                case BacnetPropertyType.ObjectCredentialDataInput:
                case BacnetPropertyType.ObjectNetworkSecurity:
                case BacnetPropertyType.ObjectNotificationForwarder:
                case BacnetPropertyType.ObjectAlertEnrollment:
                case BacnetPropertyType.ObjectChannel:
                    return false;

                default:
                    return false;
            }
        }

        public static Direction BacnetPropertyTypeToDirection(BacnetPropertyType type)
        {
            switch (type)
            {
                case BacnetPropertyType.ObjectAnalogInput:
                case BacnetPropertyType.ObjectBinaryInput:
                case BacnetPropertyType.ObjectMultiStateInput:
                    return Direction.Output;

                case BacnetPropertyType.ObjectAnalogOutput:
                case BacnetPropertyType.ObjectBinaryOutput:
                case BacnetPropertyType.ObjectMultiStateOutput:
                case BacnetPropertyType.ObjectLightingOutput:
                case BacnetPropertyType.ObjectBinaryLightingOutput:
                    return Direction.InputOutput;

                case BacnetPropertyType.ObjectAnalogValue:
                case BacnetPropertyType.ObjectBinaryValue:
                case BacnetPropertyType.ObjectBitstringValue:
                case BacnetPropertyType.ObjectCharacterstringValue:
                case BacnetPropertyType.ObjectDatePatternValue:
                case BacnetPropertyType.ObjectDateValue:
                case BacnetPropertyType.ObjectDatetimePatternValue:
                case BacnetPropertyType.ObjectDatetimeValue:
                case BacnetPropertyType.ObjectIntegerValue:
                case BacnetPropertyType.ObjectLargeAnalogValue:
                case BacnetPropertyType.ObjectOctetstringValue:
                case BacnetPropertyType.ObjectPositiveIntegerValue:
                case BacnetPropertyType.ObjectTimePatternValue:
                case BacnetPropertyType.ObjectTimeValue:
                case BacnetPropertyType.ObjectMultiStateValue:
                    return Direction.InputOutput;

                case BacnetPropertyType.ObjectCalendar:
                case BacnetPropertyType.ObjectCommand:
                case BacnetPropertyType.ObjectDevice:
                case BacnetPropertyType.ObjectEventEnrollment:
                case BacnetPropertyType.ObjectFile:
                case BacnetPropertyType.ObjectGroup:
                case BacnetPropertyType.ObjectLoop:
                case BacnetPropertyType.ObjectNotificationClass:
                case BacnetPropertyType.ObjectProgram:
                case BacnetPropertyType.ObjectSchedule:
                case BacnetPropertyType.ObjectAveraging:
                case BacnetPropertyType.ObjectTrendlog:
                case BacnetPropertyType.ObjectLifeSafetyPoint:
                case BacnetPropertyType.ObjectLifeSafetyZone:
                case BacnetPropertyType.ObjectAccumulator:
                case BacnetPropertyType.ObjectPulseConverter:
                case BacnetPropertyType.ObjectEventLog:
                case BacnetPropertyType.ObjectGlobalGroup:
                case BacnetPropertyType.ObjectTrendLogMultiple:
                case BacnetPropertyType.ObjectLoadControl:
                case BacnetPropertyType.ObjectStructuredView:
                case BacnetPropertyType.ObjectAccessDoor:
                case BacnetPropertyType.ObjectTimer:
                case BacnetPropertyType.ObjectAccessCredential:
                case BacnetPropertyType.ObjectAccessPoint:
                case BacnetPropertyType.ObjectAccessRights:
                case BacnetPropertyType.ObjectAccessUser:
                case BacnetPropertyType.ObjectAccessZone:
                case BacnetPropertyType.ObjectCredentialDataInput:
                case BacnetPropertyType.ObjectNetworkSecurity:
                case BacnetPropertyType.ObjectNotificationForwarder:
                case BacnetPropertyType.ObjectAlertEnrollment:
                case BacnetPropertyType.ObjectChannel:
                    return Direction.Output;

                default:
                    return Direction.Output;
            }
        }

        public static Flavor BacnetPropertyTypeToFlavor(BacnetPropertyType type)
        {
            switch (type)
            {
                case BacnetPropertyType.ObjectBinaryInput:
                case BacnetPropertyType.ObjectBinaryOutput:
                case BacnetPropertyType.ObjectBinaryValue:
                    return Flavor.Bool;

                case BacnetPropertyType.ObjectAnalogInput:
                case BacnetPropertyType.ObjectMultiStateInput:
                case BacnetPropertyType.ObjectAnalogOutput:
                case BacnetPropertyType.ObjectMultiStateOutput:
                case BacnetPropertyType.ObjectLightingOutput:
                case BacnetPropertyType.ObjectBinaryLightingOutput:
                case BacnetPropertyType.ObjectAnalogValue:
                case BacnetPropertyType.ObjectIntegerValue:
                case BacnetPropertyType.ObjectLargeAnalogValue:
                case BacnetPropertyType.ObjectPositiveIntegerValue:
                case BacnetPropertyType.ObjectMultiStateValue:
                    return Flavor.Num;

                case BacnetPropertyType.ObjectBitstringValue:
                case BacnetPropertyType.ObjectCharacterstringValue:
                case BacnetPropertyType.ObjectDatePatternValue:
                case BacnetPropertyType.ObjectDateValue:
                case BacnetPropertyType.ObjectDatetimePatternValue:
                case BacnetPropertyType.ObjectDatetimeValue:
                case BacnetPropertyType.ObjectOctetstringValue:
                case BacnetPropertyType.ObjectTimePatternValue:
                case BacnetPropertyType.ObjectTimeValue:
                case BacnetPropertyType.ObjectCalendar:
                case BacnetPropertyType.ObjectCommand:
                case BacnetPropertyType.ObjectDevice:
                case BacnetPropertyType.ObjectEventEnrollment:
                case BacnetPropertyType.ObjectFile:
                case BacnetPropertyType.ObjectGroup:
                case BacnetPropertyType.ObjectLoop:
                case BacnetPropertyType.ObjectNotificationClass:
                case BacnetPropertyType.ObjectProgram:
                case BacnetPropertyType.ObjectSchedule:
                case BacnetPropertyType.ObjectAveraging:
                case BacnetPropertyType.ObjectTrendlog:
                case BacnetPropertyType.ObjectLifeSafetyPoint:
                case BacnetPropertyType.ObjectLifeSafetyZone:
                case BacnetPropertyType.ObjectAccumulator:
                case BacnetPropertyType.ObjectPulseConverter:
                case BacnetPropertyType.ObjectEventLog:
                case BacnetPropertyType.ObjectGlobalGroup:
                case BacnetPropertyType.ObjectTrendLogMultiple:
                case BacnetPropertyType.ObjectLoadControl:
                case BacnetPropertyType.ObjectStructuredView:
                case BacnetPropertyType.ObjectAccessDoor:
                case BacnetPropertyType.ObjectTimer:
                case BacnetPropertyType.ObjectAccessCredential:
                case BacnetPropertyType.ObjectAccessPoint:
                case BacnetPropertyType.ObjectAccessRights:
                case BacnetPropertyType.ObjectAccessUser:
                case BacnetPropertyType.ObjectAccessZone:
                case BacnetPropertyType.ObjectCredentialDataInput:
                case BacnetPropertyType.ObjectNetworkSecurity:
                case BacnetPropertyType.ObjectNotificationForwarder:
                case BacnetPropertyType.ObjectAlertEnrollment:
                case BacnetPropertyType.ObjectChannel:
                    return Flavor.Text;

                default:
                    return Flavor.Text;
            }
        }

        public static string BacnetPropertyTypeToString(BacnetPropertyType type)
        {
            switch (type)
            {
                case BacnetPropertyType.ObjectBinaryInput:
                    return "Wejście binarne";

                case BacnetPropertyType.ObjectBinaryOutput:
                    return "Wyjście binarne";

                case BacnetPropertyType.ObjectBinaryValue:
                    return "Wartość binarna";

                case BacnetPropertyType.ObjectAnalogInput:
                    return "Wejście analogowe";

                case BacnetPropertyType.ObjectMultiStateInput:
                    return "Wejście wielostanowe";

                case BacnetPropertyType.ObjectAnalogOutput:
                    return "Wyjście analogowe";

                case BacnetPropertyType.ObjectMultiStateOutput:
                    return "Wyjście wielostanowe";

                case BacnetPropertyType.ObjectLightingOutput:
                    return "Wyjście oświetleniowe";

                case BacnetPropertyType.ObjectBinaryLightingOutput:
                    return "Wyjście binarne świetlne";

                case BacnetPropertyType.ObjectAnalogValue:
                    return "Wartość analogowa";

                case BacnetPropertyType.ObjectIntegerValue:
                    return "Wartość całkowita";

                case BacnetPropertyType.ObjectLargeAnalogValue:
                    return "Wartość analogowa";

                case BacnetPropertyType.ObjectPositiveIntegerValue:
                    return "Wartość całkowita dodatnia";

                case BacnetPropertyType.ObjectMultiStateValue:
                    return "Wartość wielostanowa";

                case BacnetPropertyType.ObjectBitstringValue:
                    return "Wartość bitowa";

                case BacnetPropertyType.ObjectCharacterstringValue:
                    return "Wartość tekstowa";

                case BacnetPropertyType.ObjectDatePatternValue:
                case BacnetPropertyType.ObjectDateValue:
                case BacnetPropertyType.ObjectDatetimePatternValue:
                case BacnetPropertyType.ObjectDatetimeValue:
                case BacnetPropertyType.ObjectOctetstringValue:
                case BacnetPropertyType.ObjectTimePatternValue:
                case BacnetPropertyType.ObjectTimeValue:
                case BacnetPropertyType.ObjectCalendar:
                case BacnetPropertyType.ObjectCommand:
                case BacnetPropertyType.ObjectDevice:
                case BacnetPropertyType.ObjectEventEnrollment:
                case BacnetPropertyType.ObjectFile:
                case BacnetPropertyType.ObjectGroup:
                case BacnetPropertyType.ObjectLoop:
                case BacnetPropertyType.ObjectNotificationClass:
                case BacnetPropertyType.ObjectProgram:
                case BacnetPropertyType.ObjectSchedule:
                case BacnetPropertyType.ObjectAveraging:
                case BacnetPropertyType.ObjectTrendlog:
                case BacnetPropertyType.ObjectLifeSafetyPoint:
                case BacnetPropertyType.ObjectLifeSafetyZone:
                case BacnetPropertyType.ObjectAccumulator:
                case BacnetPropertyType.ObjectPulseConverter:
                case BacnetPropertyType.ObjectEventLog:
                case BacnetPropertyType.ObjectGlobalGroup:
                case BacnetPropertyType.ObjectTrendLogMultiple:
                case BacnetPropertyType.ObjectLoadControl:
                case BacnetPropertyType.ObjectStructuredView:
                case BacnetPropertyType.ObjectAccessDoor:
                case BacnetPropertyType.ObjectTimer:
                case BacnetPropertyType.ObjectAccessCredential:
                case BacnetPropertyType.ObjectAccessPoint:
                case BacnetPropertyType.ObjectAccessRights:
                case BacnetPropertyType.ObjectAccessUser:
                case BacnetPropertyType.ObjectAccessZone:
                case BacnetPropertyType.ObjectCredentialDataInput:
                case BacnetPropertyType.ObjectNetworkSecurity:
                case BacnetPropertyType.ObjectNotificationForwarder:
                case BacnetPropertyType.ObjectAlertEnrollment:
                case BacnetPropertyType.ObjectChannel:
                    return "Wartość nie obslugiwana";

                default:
                    return "Wartość nie obslugiwana";
            }
        }

        public static bool CanSubscribe(BacnetPropertyType type)
        {
            switch (type)
            {
                case BacnetPropertyType.ObjectAnalogInput:
                case BacnetPropertyType.ObjectBinaryInput:
                case BacnetPropertyType.ObjectMultiStateInput:
                    return true;

                case BacnetPropertyType.ObjectAnalogOutput:
                case BacnetPropertyType.ObjectBinaryOutput:
                case BacnetPropertyType.ObjectMultiStateOutput:
                case BacnetPropertyType.ObjectLightingOutput:
                case BacnetPropertyType.ObjectBinaryLightingOutput:
                    return true;

                case BacnetPropertyType.ObjectAnalogValue:
                case BacnetPropertyType.ObjectBinaryValue:
                case BacnetPropertyType.ObjectBitstringValue:
                case BacnetPropertyType.ObjectCharacterstringValue:
                case BacnetPropertyType.ObjectDatePatternValue:
                case BacnetPropertyType.ObjectDateValue:
                case BacnetPropertyType.ObjectDatetimePatternValue:
                case BacnetPropertyType.ObjectDatetimeValue:
                case BacnetPropertyType.ObjectIntegerValue:
                case BacnetPropertyType.ObjectLargeAnalogValue:
                case BacnetPropertyType.ObjectOctetstringValue:
                case BacnetPropertyType.ObjectPositiveIntegerValue:
                case BacnetPropertyType.ObjectTimePatternValue:
                case BacnetPropertyType.ObjectTimeValue:
                case BacnetPropertyType.ObjectMultiStateValue:
                    return true;

                case BacnetPropertyType.ObjectCalendar:
                case BacnetPropertyType.ObjectCommand:
                case BacnetPropertyType.ObjectDevice:
                case BacnetPropertyType.ObjectEventEnrollment:
                case BacnetPropertyType.ObjectFile:
                case BacnetPropertyType.ObjectGroup:
                case BacnetPropertyType.ObjectLoop:

                case BacnetPropertyType.ObjectProgram:
                case BacnetPropertyType.ObjectSchedule:
                case BacnetPropertyType.ObjectAveraging:
                case BacnetPropertyType.ObjectTrendlog:
                case BacnetPropertyType.ObjectLifeSafetyPoint:
                case BacnetPropertyType.ObjectLifeSafetyZone:
                case BacnetPropertyType.ObjectAccumulator:
                case BacnetPropertyType.ObjectPulseConverter:
                case BacnetPropertyType.ObjectEventLog:
                case BacnetPropertyType.ObjectGlobalGroup:
                case BacnetPropertyType.ObjectTrendLogMultiple:
                case BacnetPropertyType.ObjectLoadControl:
                case BacnetPropertyType.ObjectStructuredView:
                case BacnetPropertyType.ObjectAccessDoor:
                case BacnetPropertyType.ObjectTimer:
                case BacnetPropertyType.ObjectAccessCredential:
                case BacnetPropertyType.ObjectAccessPoint:
                case BacnetPropertyType.ObjectAccessRights:
                case BacnetPropertyType.ObjectAccessUser:
                case BacnetPropertyType.ObjectAccessZone:
                case BacnetPropertyType.ObjectCredentialDataInput:
                case BacnetPropertyType.ObjectNetworkSecurity:
                case BacnetPropertyType.ObjectNotificationForwarder:
                case BacnetPropertyType.ObjectAlertEnrollment:
                case BacnetPropertyType.ObjectChannel:
                    return true;

                case BacnetPropertyType.ObjectNotificationClass:
                    return false;

                default:
                    return false;
            }
        }
    }
}
