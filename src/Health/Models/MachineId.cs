using System;

namespace Health.Models
{
    [Serializable]
    public class MachineId
    {
        public string EnvName { get; set; }
        public string MachineName { get; set; }

        public MachineId(string envName, string machineName)
        {
            EnvName = envName;
            MachineName = machineName;
        }

        public MachineId()
        {
        }

        public override String ToString()
        {
            return EnvName + "." + MachineName;
        }
    }
}