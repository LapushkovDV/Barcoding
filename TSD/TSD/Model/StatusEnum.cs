namespace TSD.Model
{
    /// <summary>
    /// Перичислитель статусов
    /// </summary>
    public enum StatusEnum
    {
        InWork = 1,
        InSyncToServer = 2,
        SendInERP = 3,
        Complete = 4,
        Dictionary = 5,
        Processed = 6
    }
}