using UnityEngine;
using System.Runtime.InteropServices;

using System;

public class WinApi : MonoBehaviour
{
    [DllImport("User32", EntryPoint = "GetSystemMetrics")]
    public static extern int GetSystemMetrics(SystemMetrics nIndex);

    public enum SystemMetrics : int
    {
        SM_CXSCREEN = 0,
        SM_CYSCREEN = 1,
    };

    [DllImport("Kernel32")]
    public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr pAttributes, uint flProtect,
            uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string pName);

    //[DllImport("Kernel32")]
    //static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

    [DllImport("Kernel32")]
    public static extern Boolean CloseHandle(IntPtr handle);

    [DllImport("Kernel32")]
    public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject,
        UInt32 dwDesiredAccess,
        UInt32 dwFileOffsetHigh, UInt32 dwFileOffsetLow,
        IntPtr dwNumberOfBytesToMap);


    [DllImport("Kernel32")]
    public static extern Boolean UnmapViewOfFile(IntPtr address);

    public readonly IntPtr InvalidHandleValue = new IntPtr(-1);
    public const UInt32 FILE_MAP_WRITE = 2;
    public const UInt32 FILE_MAP_READ = 4;
    public const UInt32 FILE_MAP_ALL_ACCESS = 4;
    public const UInt32 PAGE_READWRITE = 0x04;

    private IntPtr m_hFileMap = IntPtr.Zero;
    //private String m_name;
    private IntPtr m_address = Marshal.AllocHGlobal(sizeof(float)); //float* m_address

    //point cloud data
    const int cameraWidth = 512;
    const int cameraHeight = 424;
    const int ElementSize = cameraWidth * cameraHeight * 6;
    const int m_dwFileSize = sizeof(float) * ElementSize;
    const int FloatSize = sizeof(float);

    public WinApi(string name)
    {
        CreateSharedMemory(name);
    }

    ~WinApi()
    {
        cleanup();
    }

    public void CreateSharedMemory(string name)
    {
        //m_hFileMap = CreateFileMapping(InvalidHandleValue, IntPtr.Zero, PAGE_READWRITE, 0, unchecked((uint)size), name);
        m_hFileMap = CreateFileMapping(InvalidHandleValue, IntPtr.Zero, PAGE_READWRITE, 0, m_dwFileSize, name);

        if (m_hFileMap == IntPtr.Zero)
            throw new Exception("Open/create error: " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());
    }

    public void OpenMapView_Read()
    {
        if (m_address != IntPtr.Zero)
        {
            CloseMapView();
        }
        m_address = MapViewOfFile(m_hFileMap, FILE_MAP_READ, 0, 0, IntPtr.Zero);
        if (m_address == IntPtr.Zero) { throw new Exception("MapViewOfFile error: " + Marshal.GetLastWin32Error()); }
    }

    public unsafe float[] GetData()
    {
        float[] ArrayData = new float[ElementSize];
        byte[] FloatBytes = new byte[FloatSize];

        OpenMapView_Read();

        int index = 0;

        for (int j = 0; j < ElementSize; j++)
        {
            //get byte perfloat
            for (int i = 0; i < FloatSize; i++) { FloatBytes[i] = Address[index + i]; }
            //for (int i = 0; i < FloatSize; i++) { FloatBytes[i] = Address[50 + i]; }
            ArrayData[j] = BitConverter.ToSingle(FloatBytes,0); //convert bytes to float
            //ArrayData[j] = (float)m_address[j];
            index += FloatSize;
        }

        Debug.Log(ArrayData[1]);

        CloseMapView();

        return ArrayData;
    }

    public unsafe Byte* Address
    {
        get { return (byte*)m_address; }
    }
    

    public void CloseMapView()
    {
        if (m_address != IntPtr.Zero)
        {
            UnmapViewOfFile(m_address);
            m_address = IntPtr.Zero;
        }
    }

    public void cleanup()
    {
        if (m_address != IntPtr.Zero)
        {
            UnmapViewOfFile(m_address);
            m_address = IntPtr.Zero;
        }

        if (m_hFileMap != IntPtr.Zero)
        {
            CloseHandle(m_hFileMap);
            m_hFileMap = IntPtr.Zero;
        }
    }

    public void Dispose()
    {
        cleanup();
    }
}