using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReakKK
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        const int WH_KEYBOARD_LL = 13; // Номер глобального LowLevel-хука на клавиатуру
        const int WM_KEYDOWN = 0x100; // Сообщения нажатия клавиши

        private LowLevelKeyboardProc _proc = hookProc;

        private static IntPtr hhook = IntPtr.Zero;

        public void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }

        public static void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }

        static char[] realkk = { 'ㄹ', 'ㅇ', 'ㅋ', 'ㅋ' };
        static int index = 0;
        static bool flag = false;
        public static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (!flag)
            {
                if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    if (char.IsLetter((char)vkCode))
                    {
                        char letter = realkk[index];

                        index = (index + 1) % 4;

                        flag = true;
                        SendKeys.Send(letter.ToString());

                        return (IntPtr)1;
                    }

                    return CallNextHookEx(hhook, code, (int)wParam, lParam);
                }
                else
                {
                    return CallNextHookEx(hhook, code, (int)wParam, lParam);
                }
            }
            else
            {
                flag = false;
                return CallNextHookEx(hhook, code, (int)wParam, lParam);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetHook();
        }
    }
}
