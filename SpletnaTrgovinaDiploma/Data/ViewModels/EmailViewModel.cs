namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class EmailViewModel
    {
        public string Header { get; set; }

        public string Body { get; set; }

        public string Footer { get; set; }

        public EmailViewModel()
        {

        }

        public EmailViewModel(string header, string body, string footer = "")
        {
            Header = header;
            Body = body;
            Footer = footer;
        }
    }
}
