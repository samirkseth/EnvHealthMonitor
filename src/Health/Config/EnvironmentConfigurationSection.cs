using System;
using System.Configuration;
using System.Web;

namespace Health.Config
{
    public enum LogLevel
    {
        INFO,
        WARN,
        DEBUG
    };

    public interface IEnvironmentConfiguration
    {
        string ConfigurationRoot { get; set; }

        bool AllowProbesToCreateEnvironmentDir { get; set; }
    }

    public class EnvironmentConfigurationSection : ConfigurationSection, IEnvironmentConfiguration
    {
        [ConfigurationProperty("ConfigurationRoot", DefaultValue = @"~/App_Data/Configuration", IsRequired = false)]
        public string ConfigurationRoot
        {
            get
            {
                return HttpContext.Current.Server.MapPath((string)this["ConfigurationRoot"]);
            }
            set
            {
                this["ConfigurationRoot"] = HttpContext.Current.Server.MapPath(value);
            }
        }

        [ConfigurationProperty("AllowProbesToCreateEnvironmentDir", DefaultValue = false, IsRequired = false)]
        public bool AllowProbesToCreateEnvironmentDir
        {
            get
            {
                return (bool)this["AllowProbesToCreateEnvironmentDir"];
            }
            set
            {
                this["AllowProbesToCreateEnvironmentDir"] = value;
            }
        }
    
    }
}