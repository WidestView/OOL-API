using System.Collections.Generic;
using System.Linq;

namespace OOL_API.Models
{
    public enum AccessLevel
    {
        Default,
        Sudo
    }


    public class AccessLevelInfo
    {
        public const string DefaultString = "boring";
        public const string SudoString = "cool";

        public static IReadOnlyDictionary<string, AccessLevel> Values { get; } = new Dictionary<string, AccessLevel>
        {
            [DefaultString] = AccessLevel.Default,
            [SudoString] = AccessLevel.Sudo
        };

        public static IReadOnlyDictionary<AccessLevel, string> Strings { get; } = Values.ToDictionary(
            x => x.Value,
            x => x.Key
        );
    }
}