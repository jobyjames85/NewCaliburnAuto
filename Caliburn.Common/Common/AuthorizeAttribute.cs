using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleSystem.Common.Common
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
    public class AuthorizeAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AuthorizeAttribute"/> class.
        /// </summary>
        public AuthorizeAttribute()
        {
            Roles = new string[] { };
        }
        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public string[] Roles { get; set; }
    }
}
