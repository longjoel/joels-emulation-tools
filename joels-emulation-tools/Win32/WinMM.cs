using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace joels_emulation_tools.Win32
{
    internal class WinMM
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA")]
        public static extern int MciSendString(string lpstrCommand,
          string lpstrReturnString, int uReturnLength, int hwndCallback);
    }
}
