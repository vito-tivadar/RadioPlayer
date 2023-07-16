
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RadioPlayer
{ 
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static string radioTXT = Properties.Resources.radio;
        string[] radioName = Regex.Split(radioTXT, "\r\n");
        
        static string radioURL = Properties.Resources.radio_url;
        string[] radio = Regex.Split(radioURL, "\r\n");

        //string SongName = "";
        string SongNameFile = "";
        string Separator = ",";



        public static System.Windows.Input.RoutedUICommand Pause { get; }
        private int playing = 0;
        private int position = 0;

        public MainWindow()
        {
            InitializeComponent();
            Player.Source = new Uri(radio[position], UriKind.RelativeOrAbsolute);
            RadioNameBox.Text = radioName[position].ToString();
            
        }


        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;

        //Modifiers:
        private const uint MOD_NONE = 0x0000; //(none)
        private const uint MOD_ALT = 0x0001; //ALT
        private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint MOD_SHIFT = 0x0004; //SHIFT
        private const uint MOD_WIN = 0x0008; //WINDOWS
        //CAPS LOCK:
        private const uint PLAY = 0xB3;
        private const uint PREVIOUS = 0xB1;
        private const uint NEXT = 0xB0;

        


        private IntPtr _windowHandle;
        private HwndSource _source;
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_NONE, PLAY); //CTRL + CAPS_LOCK
            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_NONE, PREVIOUS); //CTRL + CAPS_LOCK
            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_NONE, NEXT); //CTRL + CAPS_LOCK

        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == PLAY)
                            {

                                if (playing < 1)
                                {
                                    Player.Source = new Uri(radio[position], UriKind.RelativeOrAbsolute);
                                    Player.Play();
                                    playing = 1;
                                    RadioNameBox.Text = radioName[position].ToString();
                                    /*
                                    SongNameFile = radio[position] + "/7";
                                    string[] SongName = Regex.Split(SongNameFile, Separator);

                                    SongNameBox.Text = SongName[0];
                                    */
                                }

                                else
                                {
                                    Player.Source = new Uri("", UriKind.RelativeOrAbsolute);
                                    Player.Stop();
                                    playing = 0;
                                    RadioNameBox.Text = radioName[position].ToString();
                                }

                            }
                            handled = true;

                            if (vkey == PREVIOUS)
                            {

                                if (position == 0)
                                {
                                    position = 16;
                                    Player.Source = new Uri(radio[position], UriKind.RelativeOrAbsolute);
                                    Player.Play();
                                    RadioNameBox.Text = radioName[position].ToString();

                                }
                                else
                                {
                                    position -= 1;
                                    Player.Source = new Uri(radio[position], UriKind.RelativeOrAbsolute);
                                    Player.Play();
                                    RadioNameBox.Text = radioName[position].ToString();

                                }

                            }
                            handled = true;

                            if (vkey == NEXT)
                            {

                                if (position == 16)
                                {
                                    position = 0;
                                    Player.Source = new Uri(radio[0], UriKind.RelativeOrAbsolute);
                                    Player.Play();
                                    RadioNameBox.Text = radioName[position].ToString();

                                }
                                else
                                {
                                    position += 1;
                                    Player.Source = new Uri(radio[position], UriKind.RelativeOrAbsolute);
                                    Player.Play();
                                    RadioNameBox.Text = radioName[position].ToString();

                                }

                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            base.OnClosed(e);
        }
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Volume = VolumeSlider.Value;
        }
    }
}
