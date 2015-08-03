using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hypermint
{
    interface IimageMagick
    {
        string IMPath { get; set; }
    }

    interface IRocketLaunch
    {
        string RLPath { get; set; }
        string RLMediaPath { get; set; }
    }

    interface IHyperspin
    {
        string HSPath { get; set; }
    }
}
