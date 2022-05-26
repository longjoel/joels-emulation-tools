using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

/*
 * Code has been borrowed from multiple locations.
 * https://www.codeproject.com/articles/11361/how-to-access-the-cd-rom
 * https://www.codeproject.com/Articles/5458/C-Sharp-Ripper
 */

//
//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
//
//  Email:  yetiicb@hotmail.com
//
//  Copyright (C) 2002-2003 Idael Cardoso. 
//

namespace joels_emulation_tools.Win32
{
    public class DataReadEventArgs : EventArgs
    {
        private byte[] m_Data;
        private uint m_DataSize;
        public DataReadEventArgs(byte[] data, uint size)
        {
            m_Data = data;
            m_DataSize = size;
        }
        public byte[] Data
        {
            get
            {
                return m_Data;
            }
        }
        public uint DataSize
        {
            get
            {
                return m_DataSize;
            }
        }
    }

    public class ReadProgressEventArgs : EventArgs
    {
        private uint m_Bytes2Read;
        private uint m_BytesRead;
        private bool m_CancelRead = false;
        public ReadProgressEventArgs(uint bytes2read, uint bytesread)
        {
            m_Bytes2Read = bytes2read;
            m_BytesRead = bytesread;
        }
        public uint Bytes2Read
        {
            get
            {
                return m_Bytes2Read;
            }
        }
        public uint BytesRead
        {
            get
            {
                return m_BytesRead;
            }
        }
        public bool CancelRead
        {
            get
            {
                return m_CancelRead;
            }
            set
            {
                m_CancelRead = value;
            }
        }
    }

    internal enum DeviceChangeEventType { DeviceInserted, DeviceRemoved };
    internal class DeviceChangeEventArgs : EventArgs
    {
        private DeviceChangeEventType m_Type;
        private char m_Drive;
        public DeviceChangeEventArgs(char drive, DeviceChangeEventType type)
        {
            m_Drive = drive;
            m_Type = type;
        }
        public char Drive
        {
            get
            {
                return m_Drive;
            }
        }
        public DeviceChangeEventType ChangeType
        {
            get
            {
                return m_Type;
            }
        }
    }
    public delegate void CdDataReadEventHandler(object sender, DataReadEventArgs ea);
    public delegate void CdReadProgressEventHandler(object sender, ReadProgressEventArgs ea);
    internal delegate void DeviceChangeEventHandler(object sender, DeviceChangeEventArgs ea);

    internal enum DeviceType : uint
    {
        DBT_DEVTYP_OEM = 0x00000000,      // oem-defined device type
        DBT_DEVTYP_DEVNODE = 0x00000001,  // devnode number
        DBT_DEVTYP_VOLUME = 0x00000002,   // logical volume
        DBT_DEVTYP_PORT = 0x00000003,     // serial, parallel
        DBT_DEVTYP_NET = 0x00000004       // network resource
    }

    internal enum VolumeChangeFlags : ushort
    {
        DBTF_MEDIA = 0x0001,          // media comings and goings
        DBTF_NET = 0x0002           // network volume
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DEV_BROADCAST_HDR
    {
        public uint dbch_size;
        public DeviceType dbch_devicetype;
        uint dbch_reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DEV_BROADCAST_VOLUME
    {
        public uint dbcv_size;
        public DeviceType dbcv_devicetype;
        uint dbcv_reserved;
        uint dbcv_unitmask;
        public char[] Drives
        {
            get
            {
                string drvs = "";
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    if ((dbcv_unitmask & (1 << (c - 'A'))) != 0)
                    {
                        drvs += c;
                    }
                }
                return drvs.ToCharArray();
            }
        }
        public VolumeChangeFlags dbcv_flags;
    }

    internal class DeviceChangeNotificationWindow : NativeWindow
    {
        public event DeviceChangeEventHandler DeviceChange;

        const int WS_EX_TOOLWINDOW = 0x80;
        const int WS_POPUP = unchecked((int)0x80000000);

        const int WM_DEVICECHANGE = 0x0219;

        const int DBT_APPYBEGIN = 0x0000;
        const int DBT_APPYEND = 0x0001;
        const int DBT_DEVNODES_CHANGED = 0x0007;
        const int DBT_QUERYCHANGECONFIG = 0x0017;
        const int DBT_CONFIGCHANGED = 0x0018;
        const int DBT_CONFIGCHANGECANCELED = 0x0019;
        const int DBT_MONITORCHANGE = 0x001B;
        const int DBT_SHELLLOGGEDON = 0x0020;
        const int DBT_CONFIGMGAPI32 = 0x0022;
        const int DBT_VXDINITCOMPLETE = 0x0023;
        const int DBT_VOLLOCKQUERYLOCK = 0x8041;
        const int DBT_VOLLOCKLOCKTAKEN = 0x8042;
        const int DBT_VOLLOCKLOCKFAILED = 0x8043;
        const int DBT_VOLLOCKQUERYUNLOCK = 0x8044;
        const int DBT_VOLLOCKLOCKRELEASED = 0x8045;
        const int DBT_VOLLOCKUNLOCKFAILED = 0x8046;
        const int DBT_DEVICEARRIVAL = 0x8000;
        const int DBT_DEVICEQUERYREMOVE = 0x8001;
        const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        const int DBT_DEVICEREMOVEPENDING = 0x8003;
        const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        const int DBT_DEVICETYPESPECIFIC = 0x8005;

        public DeviceChangeNotificationWindow()
        {
            CreateParams Params = new CreateParams();
            Params.ExStyle = WS_EX_TOOLWINDOW;
            Params.Style = WS_POPUP;
            CreateHandle(Params);
        }

        private void OnCDChange(DeviceChangeEventArgs ea)
        {
            if (DeviceChange != null)
            {
                DeviceChange(this, ea);
            }
        }
        private void OnDeviceChange(DEV_BROADCAST_VOLUME DevDesc, DeviceChangeEventType EventType)
        {
            if (DeviceChange != null)
            {
                foreach (char ch in DevDesc.Drives)
                {
                    DeviceChangeEventArgs a = new DeviceChangeEventArgs(ch, EventType);
                    DeviceChange(this, a);
                }
            }
        }

        protected override void WndProc(Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                DEV_BROADCAST_HDR head;
                switch (m.WParam.ToInt32())
                {
                    /*case DBT_DEVNODES_CHANGED :
                      break;
                    case DBT_CONFIGCHANGED :
                      break;*/
                    case DBT_DEVICEARRIVAL:
                        head = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                        if (head.dbch_devicetype == DeviceType.DBT_DEVTYP_VOLUME)
                        {
                            DEV_BROADCAST_VOLUME DevDesc = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                            if (DevDesc.dbcv_flags == VolumeChangeFlags.DBTF_MEDIA)
                            {
                                OnDeviceChange(DevDesc, DeviceChangeEventType.DeviceInserted);
                            }
                        }
                        break;
                    /*case DBT_DEVICEQUERYREMOVE :
                      break;
                    case DBT_DEVICEQUERYREMOVEFAILED :
                      break;
                    case DBT_DEVICEREMOVEPENDING :
                      break;*/
                    case DBT_DEVICEREMOVECOMPLETE:
                        head = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                        if (head.dbch_devicetype == DeviceType.DBT_DEVTYP_VOLUME)
                        {
                            DEV_BROADCAST_VOLUME DevDesc = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                            if (DevDesc.dbcv_flags == VolumeChangeFlags.DBTF_MEDIA)
                            {
                                OnDeviceChange(DevDesc, DeviceChangeEventType.DeviceRemoved);
                            }
                        }
                        break;
                        /*case DBT_DEVICETYPESPECIFIC :
                          break;*/
                }
            }
            base.WndProc(ref m);
        }
    }



    internal class CdRomEntry
    {
        public int DriveIndex { get; set; }
        public string VolumeName { get; set; }
        public string DiscName { get; set; }

        public CdRomEntry(int index, string volumeName, string discName)
        {
            DriveIndex = index;
            VolumeName = volumeName;
            DiscName = discName;
        }
    }

    public enum DriveTypes : uint
    {
        DRIVE_UNKNOWN = 0,
        DRIVE_NO_ROOT_DIR,
        DRIVE_REMOVABLE,
        DRIVE_FIXED,
        DRIVE_REMOTE,
        DRIVE_CDROM,
        DRIVE_RAMDISK
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct TRACK_DATA
    {
        public byte Reserved;
        private byte BitMapped;
        public byte Control
        {
            get
            {
                return (byte)(BitMapped & 0x0F);
            }
            set
            {
                BitMapped = (byte)((BitMapped & 0xF0) | (value & (byte)0x0F));
            }
        }
        public byte Adr
        {
            get
            {
                return (byte)((BitMapped & (byte)0xF0) >> 4);
            }
            set
            {
                BitMapped = (byte)((BitMapped & (byte)0x0F) | (value << 4));
            }
        }
        public byte TrackNumber;
        public byte Reserved1;
        /// <summary>
        /// Don't use array to avoid array creation
        /// </summary>
        public byte Address_0;
        public byte Address_1;
        public byte Address_2;
        public byte Address_3;
    }

    public class CdRipProgress {
        public int CurrentTrack;
        public int MaxTracks;
        public int TrackProgress;
        public int TotalProgress;
    }

    class CDBufferFiller
    {
        byte[] BufferArray;
        int WritePosition = 0;

        public CDBufferFiller(byte[] aBuffer)
        {
            BufferArray = aBuffer;
        }
        public void OnCdDataRead(object sender, DataReadEventArgs ea)
        {
            Buffer.BlockCopy(ea.Data, 0, BufferArray, WritePosition, (int)ea.DataSize);
            WritePosition += (int)ea.DataSize;
        }

    }

    class CDReadSession { 
    
    }

    static internal class CdRom
    {
        //DesiredAccess values
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint GENERIC_EXECUTE = 0x20000000;
        public const uint GENERIC_ALL = 0x10000000;

        //Share constants
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint FILE_SHARE_DELETE = 0x00000004;

        //CreationDisposition constants
        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint OPEN_ALWAYS = 4;
        public const uint TRUNCATE_EXISTING = 5;

        public const uint IOCTL_CDROM_READ_TOC = 0x00024000;
        public const uint IOCTL_STORAGE_CHECK_VERIFY = 0x002D4800;
        public const uint IOCTL_CDROM_RAW_READ = 0x0002403E;
        public const uint IOCTL_STORAGE_MEDIA_REMOVAL = 0x002D4804;
        public const uint IOCTL_STORAGE_EJECT_MEDIA = 0x002D4808;
        public const uint IOCTL_STORAGE_LOAD_MEDIA = 0x002D480C;

        public static async Task< List<CdRomEntry>> GetCDRomDrivesAsync()
        {
            var results = new List<CdRomEntry>();

            var drives = System.IO.Directory.GetLogicalDrives();

            await Task.Run(() => {
                if (drives != null)
                {
                    for (int driveIndex = 0; driveIndex < drives.Length; driveIndex++)
                    {
                        var drive = drives[driveIndex];
                        if (drive != null)
                        {
                            if (Kernel32.GetDriveType(drive) == 5)
                            {
                                StringBuilder volumeName = new StringBuilder(256);
                                int srNum = new int();
                                int comLen = new int();
                                string sysName = "";
                                int sysFlags = new int();
                                int result;

                                result = Kernel32.GetVolumeInformation(drive, volumeName, 256, srNum, comLen, sysFlags, sysName, 256);

                                if (result != 0)
                                {
                                    results.Add(new CdRomEntry(driveIndex, drive, volumeName.ToString()));
                                }
                            }
                        }
                    }

                }
            });
           
            return results;
        }

        public async static Task RipCdToFolderAsync(CdRomEntry cdRomDrive, string folder, Action<CdRipProgress> onUpdate ) { 

        
        }


    }
}
