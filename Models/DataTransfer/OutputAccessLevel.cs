using System.Collections.Generic;
using System.Linq;

namespace OOL_API.Models.DataTransfer
{
    public class OutputAccessLevel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OutputAccessLevelHandler
    {
        public IEnumerable<OutputAccessLevel> ListFromEnum()
        {
            return AccessLevelInfo.Values
                .Select(pair => new OutputAccessLevel
                {
                    Id = (int) pair.Value,
                    Name = pair.Key
                });
        }
    }
}
