using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;

using MeasurementPoint = (double yaw, double pitch);

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

        public bool IsConnected
        {
            get
            {
                return _serialConnection?.IsOpen ?? false;
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

            try {
                _serialConnection.Open();
            } catch(System.UnauthorizedAccessException e) {
                throw new VibrometerException(e.Message);
            }
        }

        public void Disconnect()
        {
            if(_serialConnection is null)
            {
                throw new VibrometerException("Closing unopened connection!");
            }
            _serialConnection.Close();
            _serialConnection = null;
            Port = "";
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
            if (channelDef.Channel is null || channelDef.MinAngle is null || channelDef.MaxAngle is null || channelDef.AngleDelta is null)
            {
                throw new VibrometerException("Some params for Yaw undefined!");
            }

            return standardWrite($"scan define yaw {channelDef.Channel} {(int)(channelDef.MinAngle * 100)} {(int)(channelDef.MaxAngle * 100)} {(int)(channelDef.AngleDelta * 100)}\r\n");
        }

        public string DefinePitch(ScannerChannelDefinition channelDef)
        {
            if (channelDef.Channel is null || channelDef.MinAngle is null || channelDef.MaxAngle is null || channelDef.AngleDelta is null)
            {
                throw new VibrometerException("Some params for Pitch undefined!");
            }

            return standardWrite($"scan define pitch {channelDef.Channel} {(int)(channelDef.MinAngle * 100)} {(int)(channelDef.MaxAngle * 100)} {(int)(channelDef.AngleDelta * 100)}\r\n");
        }

        public string ReadyScanner()
        {
            return standardWrite($"scan ready\r\n");
        }

        public string StartScan()
        {
            return standardWrite($"scan start\r\n");
        }

        public string GoToNextPointScan()
        {
            return standardWrite($"scan next_point\r\n");
        }

        public MeasurementPoint GetPointScan()
        {
            string raw_point = standardWrite($"scan get_point\r\n");

            MatchCollection matchList = Regex.Matches(raw_point, @"[0-9]+");

            if (matchList.Count() != 2)
            {
                throw new VibrometerException("There are more numbers than two in get poin response");
            }

            MeasurementPoint point;

            point.yaw = Convert.ToDouble(matchList[0].ToString()) / 100;
            point.pitch = Convert.ToDouble(matchList[1].ToString()) / 100;

            return point;

        }

        public string StopScan()
        {
            return standardWrite($"scan stop\r\n");
        }

        public string GetStatus()
        {
            string raw_status = standardWrite($"scan status\r\n");

            var prefix = "status: ";
            int index = raw_status.IndexOf(prefix);
            return ((index < 0)
                    ? raw_status
                    : raw_status.Remove(index, prefix.Length)).Trim();
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


        public string SetPosition(Channel channel, double pos)
        {
            standardWrite($"mode pos\r\n");
            standardWrite($"channel {(int)channel}\r\n");

            return standardWrite($"pos {(int)(pos * 100)}\r\n");
        }

        public double GetPosition(Channel channel)
        {
            standardWrite($"channel {(int)channel}\r\n");
            string raw_pos = standardWrite($"pos\r\n");

            MatchCollection matchList = Regex.Matches(raw_pos, @"[0-9]+");

            if (matchList.Count() != 1)
            {
                throw new VibrometerException("There are more numbers than one in get poin response");
            }

            return Convert.ToDouble(matchList[0].ToString()) / 100;
        }

        public string MotorStart(Channel channel)
        {
            standardWrite($"channel {(int)channel}\r\n");

            return standardWrite($"motor start\r\n");
        }

        public string MotorStop(Channel channel)
        {
            standardWrite($"channel {(int)channel}\r\n");

            return standardWrite($"motor stop\r\n");
        }

        public string MotorGetStatus(Channel channel)
        {
            standardWrite($"channel {(int)channel}\r\n");

            return standardWrite($"motor\r\n");
        }


        private string standardWrite(string to_write)
        {
            if (_serialConnection is null)
            {
                throw new VibrometerException("Serial Connection is null!");
            }

            _serialConnection.WriteLine(to_write);

            try
            {
                _serialConnection.ReadLine();
                return _serialConnection.ReadLine();
            }
            catch (TimeoutException)
            {
                throw new VibrometerException("Couldn't read from Vibrometer - timeout!");
            }
        }

    }
}
