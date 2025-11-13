using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotorisLib
{
    /// <summary>
    /// Configuration for a temporary scroll mode action.
    /// </summary>
    public struct TemporaryScrollConfig
    {
        public bool Enabled { set; get; }
        public int IdleTimeoutInSeconds { set; get; }
        public string ClickModuleName { set; get; }
        public string ClockwiseModuleName { set; get; }
        public string CounterclockwiseModuleName { set; get; }
        public string TimeoutModuleName { set; get; }
    }
}
