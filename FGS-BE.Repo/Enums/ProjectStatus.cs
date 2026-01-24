using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.Enums
{
    public enum ProjectStatus
    {
        Open,      // Recruiting members
        InProcess, // Active/started
        Close,     // Closed (e.g., cancelled)
        Complete   // Finished
    }
}
