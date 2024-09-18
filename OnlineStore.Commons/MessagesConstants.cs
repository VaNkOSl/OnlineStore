namespace OnlineStore.Commons;

public static class MessagesConstants
{
    public const string GeneralErrors = "Unexpected error occurred! Please try again later or contact administrator!";
    public const string GeneralAdminError = "Unexpected error occurred! Please try again later or contact support with the details of this operation!";

     // Seller
    public const string SellerNotFound = "Failed to retrieve the seller ID. Please try again or contact administrator!";
    public const string UserWithTheSameEgnExists = "A user with the same EGN already exists.";
    public const string UserWithPhoneNumberExists = "A user with the same phone number already exists.";
    public const string UserNotASeller = "You must be a become a seller to access this feature!";
    public const string UserNotFound = "The specified user account could not be found. Please try again later or contact with administrator!";
    public const string ApplicationSuccessfully = "Your seller application has been successfully submitted. Please wait for approval from the administrator.";
    public const string SellersCannonMakeOrders = "As a seller, you cannot place orders. Please use a different account to place orders.";
    public const string AdminIsRejected = "The administrator has already rejected your submitted application, please delete it first and then submit a new one";
    public const string UserIsAlreadySeller = "A seller with the name {0} already exists!";
    public const string UnexpectedErrorOccurredCreatingSeller = "An error occurred while processing your seller application! Please try again later or contact administrator!";
    public const string SellerWithPhoneNumberNotExist = "Seller with the provided phone number does not exist.";
    public const string SellerWithEmailNotExists = "Seller with the provided email does not exist.";
    public const string UserEmailNotExists = "User with the provided email does not exist.";
    public const string UserIsSeller = "The provided email is email of the seller.As seller you can't send messages to seller plese register as user no send messeges to a seller";
    public const string InvalidModel = "Please correct the errors in the form and try again!";

    // Order
    public const string SuccessfullyTakeOrder = "You have successfully marked your order as received.";
    public const string ProductAlreadyAddedToCartItem = "The {0} product has already been added to your cart";
    public const string SuccessfullyCompletedOrder = "You have successfully completed your order.Now we start processing it upon shipment";
    public const string SuccessfullyAddedItemToCart = "You have successfully added {0} product to your cart";
    public const string SuccessfullyDeleteProductFromCart = "You have successfully deleted a {0} product from your cart";
    public const string OrderNotFound = "The order could not be found, please try again later!";
    public const string UnexpectedErrorOccurredCompleteOrder = "Unexpected error occurred while trying to complite your order! Please try again later or contact administrator!";
    public const string UnexpectedErrorOccurredSendOrder = "Unexpected error occurred while trying to send order! Please try again later or contact administrator!";
    public const string UnexpectedErrorOccurredTakeOrder = "Unexpected error occurred while trying to take your order! Please try again later or contact administrator!";
    public const string SuccessfullySendOrder = "You have successfully send order!";

    // Cart Item
    public const string UnexpectedErrorOccurredAddToCartItem = "Unexpected error occurred while trying to add a product with name {0} to your cart! Please try again later or contact administrator!";
    public const string UnexpectedErrorOccurredRemoveFromCart = "Unexpected error occurred while trying to remove product with name {0} to your cart! Please try again later or contact administrator!";
    public const string UnexpectedErrorOccurredUpdateColor = "Unexpected error occurred while trying to update color of product with name {0}! Please try again later or contact administrator!";
    public const string UnexpectedErrorOccurredUpdateSize = "Unexpected error occurred while trying to update size of product with name {0}! Please try again later or contact administrator!";
    public const string UnexpectedErrorOccurredUpdateQuantity = "Unexpected error occurred while trying to update quantity of product with name {0}! Please try again later or contact administrator!";
    public const string QuantityMustBePositiveNumber = "The product quantity must be greater than zero";
    public const string UserHasNoItemInCartItem = "There are no items added to your cart. Please add first and then checkout!";

    // Product
    public const string SizeDoesNotExists = "One or more selected sizes do not exist.";
    public const string ColorDoesNotExists = "One or more selected colors do not exist.";
    public const string CategoryDoesNotExists = "Category does not exist!";
    public const string BrandDoesNotExists = "Brand does not exists!";
    public const string ErrorWhileCreatedProduct = "Unexpected error occurred while trying to add your new product! Please try again later or contact administrator!";
    public const string ProductDoesNotExists = "No product with name {0} found! Please try again later or contact administrator!";
    public const string ProductNotFound = "Sorry, we cannot find the product. Please try again later or contact an administrator.";
    public const string ReviewSuccessfullyAdded = "You have successfully added a product review named {0}";
    public const string SuccessfullyEditProduct = "You have successfully edit a prodcut with name {0}!";
    public const string UserDoesNotSelectAColor = "Please choose at least one color";
    public const string UserDoesNotSelectASize = "Plese chose at leats one size";
    public const string SuccessfullyCreatedAProduct = "You have successfully created a product with name {0}.Please wait for approval from the administrator.";
    public const string SuccessfullyDeleteProduct = "You have successfully deleted a {0}";
    public const string ErrorWhileCreatingAReview = "An unexpected error occurred while trying to create a review for a product named {0}! Please try again later or contact the administrator!";
    public const string ColorDoesNotSelected = "Choose at least one color to the product you will add";
    public const string ImageDoesNotSelected = "Chose at least one image to the product you will add";
    public const string SizeDoesNotSelected = "Chose at least one size to the product you will add";
    public const string ProductHasOrders = "You cannot delete the product because it has been ordered, you need to wait for the product to finish shipping and then the product can be deleted";
    public const string ErrorWhileDeleteAProduct = "Unexpected error occurred while trying to delete a product with name {0}! Please try again later or contact administrator!";
    public const string ErrorWhileEditProduct = "Unexpected error occurred while trying to edit a product with name {0}! Please try again later or contact administrator!";

    // Admin
    public const string ColorIdDoesNotExists = "Color with the specified ID does not exist.";
    public const string ColorNameExists = "Color with name {0} already exists!";
    public const string SuccessfullyAddColor = "You have successfully added a color with name {0}.";
    public const string SuccessfullyDeleteColor = "You have successfully delete a color with name {0}.";
    public const string BrandNameExists = "Brand with name {0} already exists!";
    public const string SuccessfullyAddBrand = "You have successfully added a brand with name {0}.";
    public const string SuccessfullyDeleteBrand = "You have successfully delete a brand with name {0}.";
    public const string SizeNameExists = "Size with name {0} already exists!";
    public const string SuccessfullyAddSize = "You have successfully added a size with name {0}";
    public const string SuccessfullyDeleteSize = "You have successfully delete a size with name {0}";
    public const string CategoryNameExists = "Category with name {0} already exists!";
    public const string SizeWithIdDoesNotExists = "Size with the specified ID does not exist.";
    public const string CategoryWithIdDoesNotExists = "Category with the specified ID does not exist.";
    public const string SuccessfullyAddCategory = "You have successfully added a category with name {0}";
    public const string SuccessfullyDeleteCategory = "You have successfully delete a category with name {0}";

    public const string SuccessfullyApprovedAProduct = "You have successfully approved a product with name {0}.";
    public const string SuccessfullyApprovedASeller = "You have successfully approved a seller with name {0}.";
    public const string SuccessfullySentAMessage = "You have successfully sent a message to user with email {0}";
    public const string SuccessfullySentResponse = "You have successfully sent a response to user with email {0}";
    public const string SuccessfullyBlockAUser = "You have successfully blocked a user named {0}";
    public const string UnexpectedErrorWhileBlockinUser = "Unexpected error occurred while trying to block a user with name {0}!.Plese try again later or contact with the owners of the website";

    // Notification
    public const string UnexpectedErrorOccurredCreatingMessege = "Unexpected error occurred while trying to send your messege to {0}! Please try again later or contact administrator!";
    public const string UnexpectedErrorOccurredCreatingResponse = "Unexpected error occurred while trying to send your response to {0}! Please try again later or contact administrator!";
    public const string UnexpectedErrorOccurredDeletingSellerRequest = "Unexpected error occurred while trying to delete your seller request! Please try again later or contact administrator!";
}
