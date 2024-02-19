namespace SpletnaTrgovinaDiploma.Helpers
{
    public static class ActiveLink
    {
        private static bool IsActiveCategory(string link, int id)
        {
            var splitLink = link.Split('/');

            if (splitLink.Length == 4 && splitLink[1].ToLower() == "category")
            {
                var isParsed = int.TryParse(splitLink[3], out var number);
                if (isParsed && number == id)
                    return true;
            }

            return false;
        }

        public static string GetCssClassIfActiveCategory(string link, int id) => IsActiveCategory(link, id) ? "active-link" : "";

        public static string GetChevronIfActiveCategory(string link, int id) => IsActiveCategory(link, id) ? ">" : "";

        private static bool IsActiveHomePage(string link)
        {
            var splitLink = link.Split('/');

            return splitLink.Length == 2 && splitLink[0].Length == 0 && splitLink[1].Length == 0;
        }

        public static string GetCurrentCategoryName(string link)
        {
            var splitLink = link.Split('/');

            if (splitLink.Length == 4 && splitLink[1].ToLower() == "category")
            {
                var isParsed = int.TryParse(splitLink[3], out _);
                if (isParsed)
                    return " > ";
            }

            return "";
        }
    }
}
