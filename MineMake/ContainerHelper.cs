using System.Collections.Generic;

namespace MineMake
{
    internal class ContainerHelper
    {
        internal static Dictionary<string, string> CloneStringDictionary(Dictionary<string, string> dict)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var pair in dict)
                result.Add(pair.Key, pair.Value);
            return result;
        }

        internal static bool ArePropertiesEqual(IReadOnlyDictionary<string, string> dict1, IReadOnlyDictionary<string, string> dict2)
        {
            if (dict1.Count != dict2.Count)
                return false;
            foreach (var pair in dict1)
            {
                if (pair.Value != dict1[pair.Key])
                    return false;
            }
            return true;
        }
    }
}
