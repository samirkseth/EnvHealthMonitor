using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Health.Models
{

    public interface IEnvironmentExtension
    {
        string GetLink(string envname);
    }

    public class EnvironmentExtension
    {
           
    }
}