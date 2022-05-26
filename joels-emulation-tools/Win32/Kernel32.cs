using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace joels_emulation_tools.Win32
{
    internal class Kernel32
    {
        [DllImport("kernel32.dll", EntryPoint = "GetVolumeInformationA")]
        public static extern int GetVolumeInformation(string lpRootPathName,
          StringBuilder lpVolumeNameBuffer, int nVolumeNameSize,
          int lpVolumeSerialNumber, int lpMaximumComponentLength,
          int lpFileSystemFlags, string lpFileSystemNameBuffer,
          int nFileSystemNameSize);

        [DllImport("kernel32.dll", EntryPoint = "GetDriveTypeA")]
        public static extern int GetDriveType(string nDrive);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int CloseHandle(IntPtr hObject);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int DeviceIoControl(IntPtr hDevice, uint IoControlCode,
            IntPtr lpInBuffer, uint InBufferSize,
            IntPtr lpOutBuffer, uint nOutBufferSize,
            ref uint lpBytesReturned,
            IntPtr lpOverlapped);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int DeviceIoControl(IntPtr hDevice, uint IoControlCode,
      IntPtr InBuffer, uint InBufferSize,
      [Out] CDROM_TOC OutTOC, uint OutBufferSize,
      ref uint BytesReturned,
      IntPtr Overlapped);

      
        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int DeviceIoControl(IntPtr hDevice, uint IoControlCode,
          [In] PREVENT_MEDIA_REMOVAL InMediaRemoval, uint InBufferSize,
          IntPtr OutBuffer, uint OutBufferSize,
          ref uint BytesReturned,
          IntPtr Overlapped);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static IntPtr CreateFile(string FileName, uint DesiredAccess,
           uint ShareMode, IntPtr lpSecurityAttributes,
           uint CreationDisposition, uint dwFlagsAndAttributes,
           IntPtr hTemplateFile);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int DeviceIoControl(IntPtr hDevice, uint IoControlCode,
            [In] RAW_READ_INFO rri, uint InBufferSize,
            [In, Out] byte[] OutBuffer, uint OutBufferSize,
            ref uint BytesReturned,
            IntPtr Overlapped);
    }
}
