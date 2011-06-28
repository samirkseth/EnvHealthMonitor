using System;
using System.Collections.Generic;

namespace Health.Models
{
    [Serializable]
    public class MachineHealth
    {
        public MachineId ID {get; set; }
        public HealthStatus Health { get; set; }
        public string Message { get; set; }
        public List<Probe> Probes { get; set; }
        public List<Parameter> Parameters { get; set; }
    }
}