using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibrometerHostApp.Models
{
    public class VibrometerConnection
    {
        public VibrometerConnection() { }

        static public List<string> GetPortNames() => SerialPort.GetPortNames().ToList();
    }
}
