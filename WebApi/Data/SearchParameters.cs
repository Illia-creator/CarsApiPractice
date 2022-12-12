namespace WebApi.Data
{
    public record SearchParameters(double price, string name)
    {
        public static bool TryParse(string input, out SearchParameters? searchParameters)
        {
            searchParameters = default;
            var splitArray = input.Split(',', 2);
            if (splitArray.Length != 2) return false;
            if (!double.TryParse(splitArray[0], out var pric)) return false;
            searchParameters = new(pric, splitArray[1].ToString());
            return true;
        }
    }
}
