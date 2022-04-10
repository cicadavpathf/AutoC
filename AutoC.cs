using System;
using System.Runtime.InteropServices;
using System.Timers;

namespace AutoC
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        public Int32 X;
        public Int32 Y;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public int mouseData;
        public int dwFlags;
        public int time;
        public IntPtr dwExtraInfo;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public short wVk;
        public short wScan;
        public int dwFlags;
        public int time;
        public IntPtr dwExtraInfo;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT_UNION
    {
        [FieldOffset(0)] public MOUSEINPUT mouse;
        [FieldOffset(0)] public KEYBDINPUT keyboard;
        [FieldOffset(0)] public HARDWAREINPUT hardware;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public int type;
        public INPUT_UNION ui;
    };

    public static class AutoCM
    {
        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_HARDWARE = 2;
        
        public const int MOUSEEVENTF_MOVE = 0x1;
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public const int MOUSEEVENTF_LEFTDOWN = 0x2;
        public const int MOUSEEVENTF_LEFTUP = 0x4;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        public const int MOUSEEVENTF_MIDDLEUP = 0x40;
        public const int MOUSEEVENTF_WHEEL = 0x800;
        public const int WHEEL_DELTA = 120;
        
        public const int KEYEVENTF_KEYDOWN = 0x0;
        public const int KEYEVENTF_KEYUP = 0x2;
        public const int KEYEVENTF_EXTENDEDKEY = 0x1;
        
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Win32Point pt);
        
        [DllImport ("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        
        [DllImport("user32.dll")]
        public static extern void SendInput(int nInputs, ref INPUT pInputs, int cbsize);
    }
    
    public class AutoC
    {
        public void Start()
        {
            var timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(Proc);
            timer.Interval  = 120000;
            timer.AutoReset = true;
            timer.Enabled   = true;
            
            Console.ReadLine();
            timer.Enabled = false;
            timer.Dispose();
        }
        
        void Proc(object sender, ElapsedEventArgs e)
        {
            var mousePosition = new Win32Point
            {
                X = 0,
                Y = 0
            };
            
            AutoCM.GetCursorPos(ref mousePosition);
            
            AutoCM.SetCursorPos(0, 0);
            AutoCM.SetCursorPos(mousePosition.X, mousePosition.Y);
            
            var inpMouseLeftDown = new INPUT
            {
                type = AutoCM.INPUT_MOUSE,
                ui = new INPUT_UNION
                {
                    mouse = new MOUSEINPUT
                    {
                        dwFlags = AutoCM.MOUSEEVENTF_LEFTDOWN,
                        dx = mousePosition.X,
                        dy = mousePosition.Y,
                        mouseData = 0,
                        dwExtraInfo = IntPtr.Zero,
                        time = 0
                    }
                }
            };
            
            var inpMouseLeftUp = new INPUT
            {
                type = AutoCM.INPUT_MOUSE,
                ui = new INPUT_UNION
                {
                    mouse = new MOUSEINPUT
                    {
                        dwFlags = AutoCM.MOUSEEVENTF_LEFTUP,
                        dx = mousePosition.X,
                        dy = mousePosition.Y,
                        mouseData = 0,
                        dwExtraInfo = IntPtr.Zero,
                        time = 0
                    }
                }
            };
            
            var inputs = new INPUT[]
            {
                inpMouseLeftDown,
                inpMouseLeftUp
            };
            
            AutoCM.SendInput(2, ref inputs[0], Marshal.SizeOf(inputs[0]));
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var p = new AutoC();
            p.Start();
        }
    }
}
