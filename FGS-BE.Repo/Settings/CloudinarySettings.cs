using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.Settings
{
    public class CloudinarySettings
    {
        public const string SectionName = "CloudinarySettings";
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
