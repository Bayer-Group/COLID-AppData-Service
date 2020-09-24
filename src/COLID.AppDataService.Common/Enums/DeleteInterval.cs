namespace COLID.AppDataService.Common.Enums
{
    /// <summary>
    /// Interval for deleting messages.
    /// int-values will be used to determine invalid combinations with <see cref="SendInterval"/>.
    ///</summary>
    public enum DeleteInterval
    {
        Weekly = 3,
        Monthly = 4,
        Quarterly = 5
    }
}
