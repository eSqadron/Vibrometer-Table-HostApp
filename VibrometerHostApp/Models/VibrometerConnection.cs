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
        public bool PWMconnectedBUG { set; get; } = true;

        public string? Port {  get; set; }

        private SerialPort? _serialConnection;
        VibrometerConnection()
        {
        }

        private static VibrometerConnection? instance = null;
        private static readonly object padlock = new();

        public static VibrometerConnection Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new VibrometerConnection();
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
            _serialConnection = new SerialPort(Port)
            {
                BaudRate = 9600,
                RtsEnable = true,
                DtrEnable = true,
                ReadTimeout = 100,
                WriteTimeout = 10
            };


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

            return StandardWrite($"scan define yaw {channelDef.Channel} {(int)(channelDef.MinAngle * 100)} {(int)(channelDef.MaxAngle * 100)} {(int)(channelDef.AngleDelta * 100)}\r\n");
        }

        public string DefinePitch(ScannerChannelDefinition channelDef)
        {
            if (channelDef.Channel is null || channelDef.MinAngle is null || channelDef.MaxAngle is null || channelDef.AngleDelta is null)
            {
                throw new VibrometerException("Some params for Pitch undefined!");
            }

            return StandardWrite($"scan define pitch {channelDef.Channel} {(int)(channelDef.MinAngle * 100)} {(int)(channelDef.MaxAngle * 100)} {(int)(channelDef.AngleDelta * 100)}\r\n");
        }

        public string ReadyScanner()
        {
            return StandardWrite($"scan ready\r\n");
        }

        public string StartScan()
        {
            return StandardWrite($"scan start\r\n");
        }

        public string GoToNextPointScan()
        {
            return StandardWrite($"scan next_point\r\n");
        }

        public MeasurementPoint GetPointScan()
        {
            string raw_point = StandardWrite($"scan get_point\r\n");

            MatchCollection matchList = Regex.Matches(raw_point, @"[0-9]+");

            if (matchList.Count != 2)
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
            return StandardWrite($"scan stop\r\n");
        }

        public string GetStatus()
        {
            string raw_status = StandardWrite($"scan status\r\n");

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
            StandardWrite($"mode pos\r\n");
            StandardWrite($"channel {(int)channel}\r\n");

            return StandardWrite($"pos {(int)(pos * 100)}\r\n");
        }

        public double GetPosition(Channel channel)
        {
            StandardWrite($"channel {(int)channel}\r\n");
            string raw_pos = StandardWrite($"pos\r\n");

            MatchCollection matchList = Regex.Matches(raw_pos, @"[0-9]+");

            if (matchList.Count != 1)
            {
                throw new VibrometerException("There are more numbers than one in get poin response");
            }

            return Convert.ToDouble(matchList[0].ToString()) / 100;
        }

        public string ZeroPosition(Channel channel)
        {
            StandardWrite($"channel {(int)channel}\r\n");
            return StandardWrite($"pos zero\r\n");
        }

        private Channel GetOtherChannel(Channel chnl)
        {
            if (chnl == Channel.CH0) return Channel.CH1;
            else return Channel.CH0;
        }

        public string MotorStart(Channel channel)
        {
            if (PWMconnectedBUG)
            {
                if(MotorGetStatus(GetOtherChannel(channel)))
                {
                    throw new VibrometerException("it is illegal to start other motor when first is started when there is PWM hardware bug!");
                }
            }

            StandardWrite($"channel {(int)channel}\r\n");

            return StandardWrite($"motor start\r\n");
        }

        public string MotorStop(Channel channel)
        {
            StandardWrite($"channel {(int)channel}\r\n");

            return StandardWrite($"motor stop\r\n");
        }

        public bool MotorGetStatus(Channel channel)
        {
            StandardWrite($"channel {(int)channel}\r\n");
            string raw_status = StandardWrite($"motor\r\n");

            MatchCollection matchList = Regex.Matches(raw_status, @"on|off!");

            if (matchList.Count != 1)
            {
                throw new VibrometerException("There are more on/off than one in get motor status response");
            }



            return matchList[0].ToString() == "on";
        }


        private string StandardWrite(string to_write)
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
