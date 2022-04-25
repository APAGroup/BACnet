using BACnetAPA.Enums;

namespace BACnetAPA.EventArgs
{
    public class ErrorEventArgs : System.EventArgs
    {
        public ErrorEventArgs(string errorText, string logText, ErrorCodes errorCode)
        {
            ErrorText = errorText;
            ErrorCode = errorCode;
            LogText = logText;
        }

        public string LogText{ get; set; }
        public string ErrorText { get; set; }

        public ErrorCodes ErrorCode { get; set; }
    }
}
