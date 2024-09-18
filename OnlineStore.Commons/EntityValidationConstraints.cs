namespace OnlineStore.Commons;

public class EntityValidationConstraints
{
    public class Products
    {
        public const int ProductNameMinLength = 3;
        public const int ProductNameMaxLength = 30;

        public const int ProductDescriptionMinLength = 15;
        public const int ProductDescriptionMaxLength = 500;

        public const int ProductStockQuantityMinValue = 1;
        public const int ProductStockQuantityMaxValue = 1000;

        public const string ProductPriceMinValue = "0";
        public const string ProductPriceMaxValue = "1000000";

        public const int ProductRejectionReasonMinLength = 15;
        public const int ProductRejectionReasonMaxLength = 500;
    }

    public class OrderItems
    {
        public const int OrderItemQuantityMaxLength = int.MaxValue;
        public const int OrderItemQuantityMinLength = 1;
    }

    public class Orders
    {
        public const int OrderAdressMinLength = 10;
        public const int OrderAdressMaxLength = 200;

        public const int UserPhoneNumberMinLenght = 7;
        public const int UserPhoneNumberMaxLenght = 15;
    }

    public class ApplicationUsers
    {
        public const int UserFirstNameMinLength = 3;
        public const int UserFirstNameMaxLength = 20;

        public const int UserLastNameMinLength = 5;
        public const int UserLastNameMaxLength = 40;
    }

    public class Reviews
    {
        public const int ReviewContentMinLength = 3;
        public const int ReviewContentMaxLength = 500;

        public const int ReviewRatingMinValue = 1;
        public const int ReviewRatingMaxValue = 5;
    }

    public class Categoryes
    {
        public const int CategoryNameMinLength = 3;
        public const int CategoryNameMaxLength = 40;

    }

    public class Brands
    {
        public const int BrandNameMinLength = 3;
        public const int BrandNameMaxLength = 40;
    }
    public class Sellers
    {
        public const int SellrFirstNameMinLength = 3;
        public const int SellerFirstNameMaxLength = 15;

        public const int SellerLastNameMinLength = 5;
        public const int SellerLastNameMaxLength = 15;

        public const int SellerDescriptionMinLength = 15;
        public const int SellerDescriptionMaxLength = 500;

        public const int SellerPhoneNumberMinLenght = 7;
        public const int SellerPhoneNumberMaxLenght = 15;

        public const int SellerEgnMinLength = 10;
        public const int SellerEgnMaxLength = 10;

        public const int SellerRejectionReasonMinLength = 15;
        public const int SellerRejectionReasonMaxLength = 500;
    }

    public class Colors
    {
        public const int ColorNameMinLength = 3;
        public const int ColorNameMaxLength = 15;
    }

    public class Sizes
    {
        public const int SizeNameMinLength = 3;
        public const int SizeNameMaxLength = 15;
    }

    public class Notifications
    {
        public const int NotificationsMessegerMinLength = 10;
        public const int NotificationsMessegerMaxLength = 700;

        public const int NotificationResponseMinLength = 10;
        public const int NotificationResponseMaxLength = 700;
    }
}
