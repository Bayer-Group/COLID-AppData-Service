namespace COLID.AppDataService.Common.Enums
{
    /// <summary>
    /// Interval for sending messages via email.
    /// int-values will be used to determine invalid combinations with <see cref="DeleteInterval"/>.
    ///</summary>
    public enum SendInterval
    {
        Never = 0,
        Immediately = 1,
        Daily = 2,
        Weekly = 3,
        Monthly = 4
    }
}
