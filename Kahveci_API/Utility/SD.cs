namespace Kahveci_API.Utility
{
    public static class SD
    {
        public const string SD_STORAGE_CONTAINER = "kahveci";
        public const string Role_Admin = "admin";
        public const string Role_Customer = "customer";

        public const string status_pending = "Onay Bekliyor"; //enum yada struct olarak sonra değiştir
        public const string status_confirmed = "Onaylandı";
        public const string status_making = "Hazırlanıyor";
        public const string status_ready = "Hazır";
        public const string status_completed = "Tamamlandı";
        public const string status_cancelled = "İptal edildi";
    }
}
