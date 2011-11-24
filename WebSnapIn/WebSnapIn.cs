using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.ComponentModel;

namespace WebSnapIn
{
    [RunInstaller(true)]
    public class WebSnapIn : PSSnapIn
    {
        public override string Description
        {
            get { return "Snap for web related activities"; }
        }

        public override string Name
        {
            get { return "WebSnapIn"; }
        }

        public override string Vendor
        {
            get { return "Brian Hartsock"; }
        }
    }
}
