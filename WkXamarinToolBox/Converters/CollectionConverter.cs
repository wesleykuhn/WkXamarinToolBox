using System.Collections.Generic;

namespace WkXamarinToolBox.Converters
{
    public static class CollectionConverter
    {
        public static string DictionaryToQueryParameters(Dictionary<string, string> keysValues)
        {
            int counter = 1;

            var result = "?";
            foreach (var param in keysValues)
            {
                result += $"{param.Key}={param.Value}";

                if (counter < keysValues.Count) result += "&";

                counter++;
            }

            return result;
        }
    }
}
