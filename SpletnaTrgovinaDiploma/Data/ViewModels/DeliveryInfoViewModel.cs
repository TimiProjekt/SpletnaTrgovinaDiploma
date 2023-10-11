namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class DeliveryInfoViewModel
    {
        public bool IsRegistered { get; set; }

        public LoginViewModel RegisteredUser { get; set; }

        public UserInfoViewModel UnregisteredUser { get; set; }
    }
}
