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
        public string? Port {  get; set; }

        private SerialPort? _serialConnection;
        VibrometerConnection()
        {
        }

        private static VibrometerConnection? instance = null;
        private static readonly object padlock = new object();

        public static VibrometerConnection Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new VibrometerConnection();
                    }
                    return instance;
                }
            }
        }

        static public List<string> GetPortNames() => SerialPort.GetPortNames().ToList();

        public void Connect()
        {
            _serialConnection = new SerialPort(Port);

            _serialConnection.BaudRate = 9600;
            _serialConnection.RtsEnable = true;
            _serialConnection.DtrEnable = true;
            _serialConnection.ReadTimeout = 100;
            _serialConnection.WriteTimeout = 100;

            _serialConnection.Open();
        }

        public string GetScanHelp()
        {
            var scan_help = String.Empty;
            _serialConnection?.WriteLine("scan\r\n");
            while (true)
            {
                try
                {
                    scan_help += _serialConnection?.ReadLine();
                } catch (TimeoutException) { break; }
            }
            

            return scan_help;
        }

        public string DefineYaw(ScannerChannelDefinition channelDef)
        {
            if (channelDef.MinAngle is null || channelDef.MaxAngle is null || channelDef.AngleDelta is null)
            {
                throw new ArgumentException();
            }

            var scan_ret = String.Empty;
            _serialConnection?.WriteLine($"scan define yaw {channelDef.Channel} {(int)(channelDef.MinAngle * 100)} {(int)(channelDef.MaxAngle * 100)} {(int)(channelDef.AngleDelta * 100)}\r\n");
            _serialConnection?.ReadLine();
            scan_ret += _serialConnection?.ReadLine();

            return scan_ret;
        }

        public string DefinePitch(ScannerChannelDefinition channelDef)
        {
            if (channelDef.MinAngle is null || channelDef.MaxAngle is null || channelDef.AngleDelta is null)
            {
                throw new ArgumentException();
            }

            var scan_ret = String.Empty;
            _serialConnection?.WriteLine($"scan define pitch {channelDef.Channel} {(int)(channelDef.MinAngle * 100)} {(int)(channelDef.MaxAngle * 100)} {(int)(channelDef.AngleDelta * 100)}\r\n");
            _serialConnection?.ReadLine();
            scan_ret += _serialConnection?.ReadLine();

            return scan_ret;
        }

        public string ReadyScanner()
        {
            var scan_ret = String.Empty;
            _serialConnection?.WriteLine($"scan ready\r\n");
            _serialConnection?.ReadLine();
            scan_ret += _serialConnection?.ReadLine();

            return scan_ret;
        }

        public string StartScan()
        {
            _serialConnection?.WriteLine($"scan start\r\n");
            _serialConnection?.ReadLine();
            return _serialConnection?.ReadLine() ?? throw new Exception();
        }

        public string GoToNextPointScan()
        {
            _serialConnection?.WriteLine($"scan next_point\r\n");
            _serialConnection?.ReadLine();
            return _serialConnection?.ReadLine() ?? throw new Exception();
        }

        public string GetPointScan()
        {
            _serialConnection?.WriteLine($"scan get_point\r\n");
            _serialConnection?.ReadLine();
            return _serialConnection?.ReadLine() ?? throw new Exception();
        }

        public string StopScan()
        {
            _serialConnection?.WriteLine($"scan stop\r\n");
            _serialConnection?.ReadLine();
            return _serialConnection?.ReadLine() ?? throw new Exception();
        }

        public string ResetScan()
        {
            _serialConnection?.WriteLine($"scan reset\r\n");
            _serialConnection?.ReadLine();
            return _serialConnection?.ReadLine() ?? throw new Exception();
        }

        public string DumpPointsFromScan()
        {
            var scan_dump = String.Empty;
            _serialConnection?.WriteLine("scan dump\r\n");
            _serialConnection?.ReadLine();
            while (true)
            {
                try
                {
                    scan_dump += _serialConnection?.ReadLine();
                }
                catch (TimeoutException) { break; }
            }

            return scan_dump;
        }

    }
}
