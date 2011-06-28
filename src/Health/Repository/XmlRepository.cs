using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Castle.Core.Logging;
using Health.Config;
using Health.Models;
using System.Web.Caching;

namespace Health.Repository
{
    public class XmlRepository : IHealthRepository
    {
        private const String Machinexml = "MachineHealth.xml";
        private ILogger _logger = NullLogger.Instance;
        public IEnvironmentConfiguration EnvConfiguration { get; set; }

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        private String GetMachineDir(MachineId macid)
        {
            return Path.Combine(EnvConfiguration.ConfigurationRoot, macid.EnvName, "Machines", macid.MachineName);
        }


        private String GetMachineHealthFileName(MachineId macid)
        {
             return Path.Combine(GetMachineDir(macid), Machinexml);
        }

        private MachineHealth GetMachineHealthFromFile(MachineId macid)
        {
            using (var fileStream = new FileStream(GetMachineHealthFileName(macid), FileMode.Open))
            {
                var xs = new XmlSerializer(typeof(MachineHealth));
                return (MachineHealth)xs.Deserialize(fileStream);
            }            
        }

        public MachineHealth GetMachineHealth(MachineId macid)
        {
            var curcache = HttpContext.Current.Cache;
            String cacheid = macid.ToString();

            if (curcache[cacheid] == null)
            {
                var obj = GetMachineHealthFromFile(macid);

                if (obj.Health.TimeStamp.AddMinutes(5) < DateTime.Now) {
                    obj.Message = String.Format("At {0} : Machine's Last Time Stamp was at {1}", DateTime.Now, obj.Health.TimeStamp);
                    obj.Health.SetStatusDOWN();
                }
                else {

                    if (obj.Probes.Any(probe => !probe.Health.isUP()))
                    {
                        obj.Health.SetStatusDOWN();                        
                    }
                }

                using (var dep = new CacheDependency(GetMachineHealthFileName(macid)))
                {
                    curcache.Insert(cacheid, obj, dep, DateTime.UtcNow.AddMinutes(5.0), Cache.NoSlidingExpiration);
                }
            }

            return (MachineHealth) curcache[cacheid];
        }


        public void Put(MachineHealth machealth)
        {
            var machinedirinfo = new DirectoryInfo(GetMachineDir(machealth.ID));

            if (!machinedirinfo.Exists) {
                if (EnvConfiguration.AllowProbesToCreateEnvironmentDir) {
                    Directory.CreateDirectory(GetMachineDir(machealth.ID));
                }
                else {
                    throw new Exception("Error : The path " + GetMachineDir(machealth.ID) + " does not exist.");
                }
            } 

            var machealthfilename = GetMachineHealthFileName(machealth.ID);

            using (var fileStream = new FileStream(machealthfilename, FileMode.Create))
            {
                var xmlTextWriter = new XmlTextWriter(fileStream, Encoding.UTF8);
                var xs = new XmlSerializer(typeof(MachineHealth));
                xs.Serialize(xmlTextWriter, machealth);
            }
        }

        public List<EnvironmentHealth> GetEnvironmentSummary()
        {
            var curcache = HttpContext.Current.Cache;
            const string cacheid = "EnviromentSummary";

            if (curcache[cacheid] == null)
            {
                var listenv = new List<EnvironmentHealth>();
                var listcachekeys = new List<string>();
                var basedir = new DirectoryInfo(EnvConfiguration.ConfigurationRoot);

                foreach (var envhealth in
                    basedir.GetDirectories()
                        .Select(dirinfo => GetEnvironmentHealth(dirinfo.Name))
                        .Where(envhealth => envhealth != null))
                {
                    listenv.Add(envhealth);
                    listcachekeys.Add(envhealth.Name);
                }

                // need to test what happens with dependency on a directory - seems to work!
                using (var dep = new CacheDependency(new[] {EnvConfiguration.ConfigurationRoot}, listcachekeys.ToArray()))
                {
                    curcache.Insert(cacheid, listenv, dep);                    
                }
            }

            return (List<EnvironmentHealth>) curcache[cacheid];
        }

        public EnvironmentHealth GetEnvironmentHealth(string envname)
        {
            var curcache = HttpContext.Current.Cache;
            var cacheid = envname;

            if (curcache[cacheid] == null)
            {
                var envhealth = new EnvironmentHealth
                 {
                    Name = envname,
                    MachineHealth = new List<MachineHealth>(),
                    Health = new HealthStatus(),
                    Parameters = new List<Parameter>()
                };

                envhealth.Health.SetStatusUP();

                var machinecachekeys = new List<string>();
                var machinesdir = Path.Combine(EnvConfiguration.ConfigurationRoot, envname, "Machines");
                var dirinfo = new DirectoryInfo(machinesdir);

                if (dirinfo.Exists)
                {
                    foreach (var machinehealth in
                        dirinfo.GetDirectories().Select(machinedir => GetMachineHealth(new MachineId(envname, machinedir.Name))))
                    {
                        if (machinehealth == null)
                        {
                            envhealth.Health.SetStatusDOWN();
                        }
                        else
                        {
                            envhealth.MachineHealth.Add(machinehealth);
                            if (!machinehealth.Health.isUP())
                            {
                                envhealth.Health.SetStatusDOWN();
                            }
                            machinecachekeys.Add(machinehealth.ID.ToString());

                            foreach (var parameter in machinehealth.Parameters.Where(parameter => parameter.Level < 2))
                            {
                                envhealth.Parameters.Add(parameter);
                            }
                        }
                    }

                    using (var dep = new CacheDependency(new[] { machinesdir }, machinecachekeys.ToArray()))
                    {
                        curcache.Insert(cacheid, envhealth, dep);                        
                    }
                }
            }

            return (EnvironmentHealth) curcache[cacheid];

        }
    }
}
