using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Health.Models
{
    [Serializable()]
    public class HealthStatus
    {
        public const string UP = "UP";
        public const string DOWN = "DOWN";

        public string Status { get; set; }
        public DateTime TimeStamp { get; set; }

        public HealthStatus()
        {
            // TODO: Complete member initialization
        }

        internal void SetStatusUP()
        {
            this.Status = UP;
        }

        internal void SetStatusDOWN()
        {
            this.Status = DOWN;
        }

        internal bool isUP()
        {
            return this.Status.Equals(UP, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}