using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hypermint
{
    interface IHypermintPaths
    {
        string IMPath { get; set; }
        string FEMediaPath { get; set; }
        string FELaunchPath { get; set; }
        string FEParams { get; set; }
    }
}
