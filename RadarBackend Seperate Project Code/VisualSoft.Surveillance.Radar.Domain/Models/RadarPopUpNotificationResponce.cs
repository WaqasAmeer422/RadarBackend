using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualSoft.Surveillance.Radar.Domain.Models
{
    public class RadarPopUpNotificationResponce
    {
        public string Location { get; set; } = string.Empty;
        public string RadarSourceMac { get; set; } = string.Empty;
        public DateTime DeviceTimestamp { get; set; }
    }
}
