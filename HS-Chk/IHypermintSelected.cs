using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hypermint
{
    interface IHypermintSelected
    {
        DatabaseGame SelectedGame { get; set; }
        string SelectedColumn { get; set; }
        string LastSelectedColumn { get; set; }
        string LastSelectedRomName { get; set; }
        string ViewerFile { get; set; }
        string InfoMessage { get; set; }
    }

    interface IHyperminSelected2
    {
        string FullPath { get; set; }
        bool IsHyperspin { get; set; }
    }
}
