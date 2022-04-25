namespace BACnetAPA 
{
    /// <summary>
    /// Bacnet request interface
    /// </summary>
    public interface IBacnetRequest
    {
        /// <summary>
        /// Is read request
        /// </summary>
        bool ReadRequest { get; }
    }
}
