using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace VibrometerHostApp.Models
{
    public struct ScannerChannelDefinition
    {
        public ScannerChannelDefinition(uint? Channel, double? MinAngle, double? MaxAngle, double? AngleDelta)
        {
            this.Channel = Channel;
            this.MinAngle = MinAngle;
            this.MaxAngle = MaxAngle;
            this.AngleDelta = AngleDelta;
        }
        public uint? Channel { get; set; }
        public double? MinAngle { get; set; }
        public double? MaxAngle { get; set; }
        public double? AngleDelta { get; set; }
    }

    public struct ScannerAxes
    {
        public ScannerChannelDefinition Yaw { get; set; }
        public ScannerChannelDefinition Pitch { get; set; }
    }
}
