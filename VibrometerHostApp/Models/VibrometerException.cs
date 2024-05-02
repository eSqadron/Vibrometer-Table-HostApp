using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibrometerHostApp.Models
{
    public class VibrometerException : Exception
    {
        public VibrometerException(string message) : base(message)
        {
        }
    }
}
