namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class LoginOrRegisterViewModel
    {
        public bool IsRegistered { get; set; }

        public LoginViewModel RegisteredUser { get; set; }

        public UnregisteredViewModel UnregisteredUser { get; set; }
    }
}
