using System;

namespace Health.Models
{
    [Serializable]
    public class Parameter
    {
        // TBD - Make an Enum
        public const int Env = 1;
        public const int Machine = 2;

        public string   Name { get; set; }
        public string   Value { get; set; }
        public int      Level { get; set; }
    }
}
