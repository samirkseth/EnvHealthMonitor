using System;
using System.Collections.Generic;

namespace Health.Models
{
    public class EnvironmentHealth
    {
        public String Name;
        public HealthStatus Health;
        public List<MachineHealth> MachineHealth;
        public List<Parameter> Parameters { get; set; }
    }
}