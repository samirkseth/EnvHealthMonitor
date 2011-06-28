using System;
using System.Web.Mvc;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.IO;

using Health.Models;

namespace Health.Controllers
{
    public class MachineCommandController : Controller
    {
        //
        // GET: /Script/

        [AcceptVerbs("GET")]
        public ActionResult Index()
        {
            var model = new MachineCommand { Command = "", Result = "" };
            return View(model);
        }

        [AcceptVerbs("POST")]
        public ActionResult Index(string command, string result)
        {
            var model = new MachineCommand { Command = command, Result = "" };

            using (var shell = PowerShell.Create())
            {
                shell.Commands.AddScript(model.Command);

                var results = shell.Invoke();

                if (results.Count > 0)
                {
                    // We use a string builder ton create our result text
                    var builder = new StringBuilder();

                    foreach (var psObject in results)
                    {
                        // Convert the Base Object to a string and append it to the string builder.
                        // Add \r\n for line breaks
                        builder.Append(psObject.BaseObject + "\r\n");
                    }

                    // Encode the string in HTML (prevent security issue with 'dangerous' caracters like < >
                    model.Result = Server.HtmlEncode(builder.ToString());
                }
                
            }


            return View(model);
        }

        public ActionResult ExecuteScript(string scriptname, string env, string machine)
        {

            var model = new MachineCommand {Command = scriptname, Result = ""};
            var scriptfilename = Path.Combine(Server.MapPath("~/App_Data/Scripts/"), scriptname);

            using (var runspace = RunspaceFactory.CreateRunspace()) {
                runspace.Open();
                var pipeline = runspace.CreatePipeline();
                var newCommand = new Command(scriptfilename);

                newCommand.Parameters.Add(new CommandParameter("env", env));
                newCommand.Parameters.Add(new CommandParameter("machine", machine));

                pipeline.Commands.Add(newCommand);

                var results = pipeline.Invoke();

                // convert the script result into a single string

                var stringBuilder = new StringBuilder();
                foreach (var obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }

                model.Result = stringBuilder.ToString();
            }

            return View(model);
        }
    }
}
