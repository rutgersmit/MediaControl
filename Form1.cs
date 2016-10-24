using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MediaControl
{
	public partial class Form1 : Form
	{
		private const int KEYEVENTF_EXTENDEDKEY = 1;
		private const int VK_MEDIA_PLAY_PAUSE = 0xB3;
		private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
		private const int APPCOMMAND_VOLUME_UP = 0xA0000;
		private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
		private const int WM_APPCOMMAND = 0x319;

		readonly KeyboardHook _hook = new KeyboardHook();
		readonly RegistryKey _rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);


		public Form1()
		{
			InitializeComponent();

			if (_rk.GetValue("Audio Control") == null)
			{
				// The value doesn't exist, the application is not set to run at startup
				toolStripMenuItem1.Checked = false;
			}
			else
			{
				// The value exists, the application is set to run at startup
				toolStripMenuItem1.Checked = true;
			}

			_hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
			_hook.RegisterHotKey(MediaControl.ModifierKeys.Win, Keys.NumPad1);
			_hook.RegisterHotKey(MediaControl.ModifierKeys.Win, Keys.NumPad2);
			_hook.RegisterHotKey(MediaControl.ModifierKeys.Win, Keys.NumPad3);
			_hook.RegisterHotKey(MediaControl.ModifierKeys.Win, Keys.NumPad5);
		}

		void hook_KeyPressed(object sender, KeyPressedEventArgs e)
		{
			switch (e.Key)
			{
				case Keys.NumPad1:
					VolDown();
					break;
				case Keys.NumPad2:
					Mute();
					break;
				case Keys.NumPad3:
					VolUp();
					break;
				case Keys.NumPad5:
					Pause(); // Or play
					break;
			}
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
		
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessageW(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		private void Pause()
		{
			keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
		}
		
		private void VolDown()
		{
			SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_DOWN);
		}

		private void VolUp()
		{
			SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_UP);
		}

		private void Mute()
		{
			SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_MUTE);
		}


		private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Pause();
		}

		private void notifyIcon1_DoubleClick(object sender, EventArgs e)
		{
			Mute();
		}

		private void muteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Mute();
		}

		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (toolStripMenuItem1.Checked)
			{
				// Add the value in the registry so that the application runs at startup
				_rk.SetValue("Audio Control", Application.ExecutablePath);
			}
			else
			{
				// Remove the value from the registry so that the application doesn't start
				_rk.DeleteValue("Audio Control", false);
			}
		}
	}
}
