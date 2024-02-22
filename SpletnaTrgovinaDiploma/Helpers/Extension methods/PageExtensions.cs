using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public static class PageExtensions
    {
        public static void SetError(this ITempDataDictionary tempData, string errorMessage)
            => tempData["Error"] = errorMessage;

        public static void SetPageDetails(this ViewDataDictionary viewData, string title, string description)
        {
            viewData["Title"] = title;
            viewData["Description"] = description;
        }
    }
}