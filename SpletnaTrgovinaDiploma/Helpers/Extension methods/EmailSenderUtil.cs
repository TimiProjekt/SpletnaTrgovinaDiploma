using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Identity;
using SpletnaTrgovinaDiploma.Data.Cart;
using SpletnaTrgovinaDiploma.Data.Services.Classes;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public static class EmailSenderUtil
    {
        //TODO: Send email when status changes and NOT when ShippingAndPayment is called
        public static void SendOrderConfirmationEmail(this ShippingAndPaymentViewModel viewModel, ShoppingCart myShoppingCart)
        {
            var messageHtml = $"Hello {viewModel.FullName}! <br/>";
            messageHtml += "The order which includes the following items: <br/> <ul>";

            foreach (var item in myShoppingCart.ShoppingCartItems)
                messageHtml += $"<li> {item.Item.Name} {item.Item.Price} € x ({item.Amount} kom) = {item.Item.Price * item.Amount} € </li>";

            messageHtml += "</ul> has been completed and will be shipped out shortly. <br/>";
            messageHtml += $"Total price: {myShoppingCart.GetShoppingCartTotalAsync()} € <br/>";
            messageHtml += $"Delivery method: {viewModel.ShippingOption}  <br/>";
            messageHtml += $"Delivery address: {viewModel.GetFullAddress}  <br/>";
            messageHtml += $"Payment method: {viewModel.PaymentOption}  <br/>";

            EmailProvider.SendEmail(
                viewModel.EmailAddress,
                "Your order is completed.",
                messageHtml);
        }

        public static async Task SendForgotPasswordEmail(this ApplicationUser appUser, string email, UserManager<ApplicationUser> userManager)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(appUser);
            var callbackUrl = $"{ConfigurationService.PublishedUrl}/Account/ResetPassword?email={email}&token={HttpUtility.UrlEncode(token)}";

            EmailProvider.SendEmail(
                email,
                "Reset password link",
                $"Dear {appUser.FullName}, <br />" +
                "You requested to reset your password." +
                $"If you requested this then follow this <a href=\"{callbackUrl}\"> link </a> to reset your password." +
                "If you did not request this, then we suggest that you immediately change your password.");
        }

        public static void SendRegisterConfirmationEmail(this RegisterViewModel viewModel)
        {
            var messageHtml = $"Hello {viewModel.FullName}! <br/>";
            messageHtml += "You have successfully registered at Gaming svet <br/> ";
            messageHtml += $"You can now <a href='{ConfigurationService.PublishedUrl}'> login! </a>";

            EmailProvider.SendEmail(
                viewModel.EmailAddress,
                "Your registration is completed",
                messageHtml);
        }

        public static void SendNewsletterSubscriptionConfirmationEmail(string email)
        {
            var messageHtml = $"Hello {email}! <br/>";
            messageHtml += "You have successfully subscribed to our newsletter at Gaming svet <br/> ";

            EmailProvider.SendEmail(
                email,
                "Your newsletter subscription",
                messageHtml);
        }

        public static void SendNewsletterUnsubscribeConfirmationEmail(string email)
        {
            var messageHtml = $"Hello {email}! <br/>";
            messageHtml += "You have successfully unsubscribed from our newsletter at Gaming svet <br/> ";

            EmailProvider.SendEmail(
                email,
                "Your newsletter subscription",
                messageHtml);
        }
    }
}
