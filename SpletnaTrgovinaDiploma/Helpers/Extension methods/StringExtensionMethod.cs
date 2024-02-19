namespace SpletnaTrgovinaDiploma.Helpers
{
    public static class StringExtensionMethod
    {
        public static bool ContainsCaseInsensitive(this string text, string contains)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            if (string.IsNullOrEmpty(contains)) 
                return false;

            return text.ToUpper().Contains(contains.ToUpper());
        }
    }
}