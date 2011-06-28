using System;

namespace Health.Models
{
    [Serializable]
    public class Probe
    {
        public String Name {get; set; }
        public HealthStatus Health { get; set; }
        public String Note { get; set; }
    }
}