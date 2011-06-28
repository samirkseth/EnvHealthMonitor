using System.Collections.Generic;
using Health.Models;

namespace Health.Repository
{
    public interface IHealthRepository
    {
        MachineHealth GetMachineHealth(MachineId macid);
        void Put(MachineHealth machealth);
        List<EnvironmentHealth> GetEnvironmentSummary();
        EnvironmentHealth GetEnvironmentHealth(string envname);
    }
}