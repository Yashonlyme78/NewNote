using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace newnotec_
{
    public partial class Form1 : Form
    {
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private WidgetForm widgetForm;

        public Form1()
        {
            InitializeComponent();
            this.Resize += new EventHandler(Form1_Resize);
            this.richTextBox1.TextChanged += new EventHandler(RichTextBox1_TextChanged);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            widgetForm = new WidgetForm(this);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                widgetForm.UpdateText(GetText());
                widgetForm.Show();
            }
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (widgetForm != null && widgetForm.Visible)
            {
                widgetForm.UpdateText(GetText());
            }
        }

        public string GetText()
        {
            return richTextBox1.Text;
        }

        public void RestoreFromWidget()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            widgetForm.Hide();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Your code here
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog yash = new OpenFileDialog();
            yash.Title = "Open";
            yash.Filter = "Text Document(*.txt)|*.txt|All Files(*.*)|*.*";
            if (yash.ShowDialog() == DialogResult.OK)
                richTextBox1.LoadFile(yash.FileName, RichTextBoxStreamType.PlainText);
            this.Text = yash.FileName;
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog yash = new SaveFileDialog();
            yash.Title = "Save";
            yash.Filter = "Text Document(*.txt)|*.txt|All Files(*.*)|*.*";
            if (yash.ShowDialog() == DialogResult.OK)
                richTextBox1.SaveFile(yash.FileName, RichTextBoxStreamType.PlainText);
            this.Text = yash.FileName;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void newToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (widgetForm != null && widgetForm.Visible)
            {
                widgetForm.UpdateText(GetText());
            }
        }

        private void notifyIcon2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon2.Visible = false;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
