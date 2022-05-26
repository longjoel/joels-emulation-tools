using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace joels_emulation_tools.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public class TrackDataList
    {
        public const int MAXIMUM_NUMBER_TRACKS = 100;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMUM_NUMBER_TRACKS * 8)]
        private byte[] Data;
        public TRACK_DATA this[int Index]
        {
            get
            {
                if ((Index < 0) | (Index >= MAXIMUM_NUMBER_TRACKS))
                {
                    throw new IndexOutOfRangeException();
                }
                TRACK_DATA res;
                GCHandle handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
                try
                {
                    IntPtr buffer = handle.AddrOfPinnedObject();
                    buffer = (IntPtr)(buffer.ToInt32() + (Index * Marshal.SizeOf(typeof(TRACK_DATA))));
                    res = (TRACK_DATA)Marshal.PtrToStructure(buffer, typeof(TRACK_DATA));
                }
                finally
                {
                    handle.Free();
                }
                return res;
            }
        }
        public TrackDataList()
        {
            Data = new byte[MAXIMUM_NUMBER_TRACKS * Marshal.SizeOf(typeof(TRACK_DATA))];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class CDROM_TOC
    {
        public ushort Length;
        public byte FirstTrack = 0;
        public byte LastTrack = 0;

        public TrackDataList TrackData;

        public CDROM_TOC()
        {
            TrackData = new TrackDataList();
            Length = (ushort)Marshal.SizeOf(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class PREVENT_MEDIA_REMOVAL
    {
        public byte PreventMediaRemoval = 0;
    }

    public enum TRACK_MODE_TYPE { YellowMode2, XAForm2, CDDA }
    [StructLayout(LayoutKind.Sequential)]
    public class RAW_READ_INFO
    {
        public long DiskOffset = 0;
        public uint SectorCount = 0;
        public TRACK_MODE_TYPE TrackMode = TRACK_MODE_TYPE.CDDA;
    }

}
