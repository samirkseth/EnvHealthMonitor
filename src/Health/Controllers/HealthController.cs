using System;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.IO;

// TBD - Should not be a dependency
using Castle.Core.Logging;
using Health.Repository;
using Health.Models;

namespace Health.Controllers
{
    public class HealthController : Controller
    {
        private ILogger _logger = NullLogger.Instance;
        public IHealthRepository Repository { get; set; }

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public ActionResult Index()
        {
            Logger.Info("Hello, World of Logging");

            var envs = Repository.GetEnvironmentSummary();

            return View(envs);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Environment(string envname)
        {
            return View(Repository.GetEnvironmentHealth(envname));
        }

        // GET: /MachineHealth/
        public ViewResult Machine(string envname, string machinename)
        {
            return View(Repository.GetMachineHealth(new MachineId(envname, machinename)));
        }

        // ValidateInput needed to turn off post filtering in .NET 4.0
        [ValidateInput(false)]
        public HttpStatusCodeResult New(string probeXml)
        {
            try
            {
                using (var xmlreader = new StringReader(probeXml))
                {
                    var xs = new XmlSerializer(typeof(MachineHealth));
                    var obj = (MachineHealth)xs.Deserialize(xmlreader);
                    Repository.Put(obj);                    
                }

                return new HttpStatusCodeResult(200);
            }
            catch (Exception e)
            {
                Logger.Error("Error in saving probe", e);
                return new HttpStatusCodeResult(400);
            }
        }

    }
}
