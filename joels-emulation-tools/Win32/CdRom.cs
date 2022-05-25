using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

/*
 * Code has been borrowed from multiple locations.
 * https://www.codeproject.com/articles/11361/how-to-access-the-cd-rom
 */
namespace joels_emulation_tools.Win32
{
    static internal class CdRom
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA")]
        public static extern int mciSendString(string lpstrCommand,
           string lpstrReturnString, int uReturnLength, int hwndCallback);

        [DllImport("kernel32.dll", EntryPoint = "GetVolumeInformationA")]
        public static extern int GetVolumeInformation(string lpRootPathName,
           StringBuilder lpVolumeNameBuffer, int nVolumeNameSize,
           int lpVolumeSerialNumber, int lpMaximumComponentLength,
           int lpFileSystemFlags, string lpFileSystemNameBuffer,
           int nFileSystemNameSize);

        [DllImport("kernel32.dll", EntryPoint = "GetDriveTypeA")]
        public static extern int GetDriveType(string nDrive);


    }
}
