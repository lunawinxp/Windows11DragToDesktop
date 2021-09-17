// sorry in advance for the terrible code but it works
// i do not care if it looks bad, it works.
// the mouse position stuff i took from stackoverflow but i forgot the link
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Windows11DragToDesktop
{
    class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        public static Point GetCursorPosition()
        {
            GetCursorPos(out POINT lpPoint);
            return lpPoint;
        }
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int key);

        static int screenX = 0;
        static int screenY = 0;

        private static void MouseListener(object obj)
        {
            while (true)
            {
                try
                {
                    Point cursorPointRn = GetCursorPosition();
                    Console.WriteLine("Cursor X: " + cursorPointRn.X + ", Y: " + cursorPointRn.Y);
                    if (cursorPointRn.X >= (screenX - 10) && cursorPointRn.Y >= (screenY - 50) && GetAsyncKeyState(1) != 0)
                    {
                        Console.WriteLine("Mouse is in bounds! Waiting 1 seconds to check again...");
                        Thread.Sleep(1000);
                        cursorPointRn = GetCursorPosition();
                        if (cursorPointRn.X >= (screenX - 10) && cursorPointRn.Y >= (screenY - 50) && GetAsyncKeyState(1) != 0)
                        {
                            Console.WriteLine("Mouse has been held! Showing desktop...");
                            Shell32.ShellClass objShel = new Shell32.ShellClass();
                            objShel.MinimizeAll();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                Thread.Sleep(100);
            }
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        static void Main()
        {
            Console.WriteLine("Windows 11 Corner Drag thingy");
            Console.WriteLine("Hiding console window in 5 seconds!");
            Thread.Sleep(5000);
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            screenX = resolution.Width;
            screenY = resolution.Height;
           
            Thread thread = new Thread(MouseListener);
            thread.SetApartmentState(ApartmentState.STA); // sta for shell32
            thread.Start();
            thread.Join();
        }
    }
}
