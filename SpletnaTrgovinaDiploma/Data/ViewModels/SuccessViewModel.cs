﻿namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class SuccessViewModel
    {
        public string Header { get; set; }

        public string Body { get; set; }

        public string Footer { get; set; }

        public SuccessViewModel()
        {

        }

        public SuccessViewModel(string header, string body, string footer = "")
        {
            Header = header;
            Body = body;
            Footer = footer;
        }
    }
}
