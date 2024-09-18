namespace OnlineStore.Commons;

public static class GeneralApplicationConstants
{
    public const int DefaultPage = 1;
    public const int EntitiesPerPage = 3;

    public const string AdminAreaName = "Admin";
    public const string AdminRoleName = "Administrator";
    public const string DevelopmentAdminEmail = "admin@onlineStore.abv.bg";

    public const string UsersCacheKey = "UsersCache";
    public const int UsersCacheDurationMinutes = 5;

    public const string OnlineUsersCookieName = "IsOnline";
    public const int LastActivityBeforeOfflineMinutes = 10;
}
