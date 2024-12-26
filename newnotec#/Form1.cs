using newnotec_;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace newnotec_
{
    public partial class Form1 : Form
    {
        private WidgetForm widgetForm;
        private ContextMenuStrip contextMenuStrip;
        private string notesDirectory;
        private Timer refreshTimer;

        private WidgetForm widget;

        public Form1()
        {
            InitializeComponent();
            PopulateOpenWindows();
            InitializeRefreshTimer();

            this.Resize += new EventHandler(Form1_Resize);
            this.richTextBox1.TextChanged += new EventHandler(richTextBox1_TextChanged);
            InitializeContextMenu();
            listBox2.SelectedIndexChanged += new EventHandler(listBox2_SelectedIndexChanged);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            IntPtr handle = this.Handle; // Get the window handle for the current form
            widgetForm = new WidgetForm(handle, this); // Pass the handle and the current Form1 instance
            ApplyDarkMode();
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //    this.Hide();
                widgetForm.UpdateText(richTextBox1.Rtf);
                //widgetForm.Show();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (widgetForm != null && widgetForm.Visible)
            {
                widgetForm.UpdateText(richTextBox1.Rtf);
            }
        }

        public string GetText()
        {
            return richTextBox1.Rtf;
        }

        public void RestoreFromWidget()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            widgetForm.Hide();

            // Optional: Unpin the note from the application
            if (pinnedNotes.ContainsKey(this.Text))
            {
                pinnedNotes.Remove(this.Text);
                UpdatePinnedNotesList();
            }
        }


        private void newToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog yash = new OpenFileDialog();
            yash.Title = "Open";
            yash.Filter = "Text Document(*.txt)|*.txt|All Files(*.*)|*.*";
            if (yash.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.LoadFile(yash.FileName, RichTextBoxStreamType.PlainText);
                this.Text = Path.GetFileName(yash.FileName); // Update form title to just the file name
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveCurrentNote();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleFontStyle(FontStyle.Italic);
        }

        private void ToggleFontStyle(FontStyle style)
        {
            if (richTextBox1.SelectionFont != null)
            {
                Font currentFont = richTextBox1.SelectionFont;
                FontStyle newFontStyle;
                if (currentFont.Style.HasFlag(style))
                {
                    newFontStyle = currentFont.Style & ~style;
                }
                else
                {
                    newFontStyle = currentFont.Style | style;
                }
                richTextBox1.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
            }
        }

        private void boldToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ToggleFontStyle(FontStyle.Bold);
        }

        private void bulletsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionBullet = !richTextBox1.SelectionBullet;
        }

        private void strikethroughToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ToggleFontStyle(FontStyle.Strikeout);
        }

        private void underlineToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ToggleFontStyle(FontStyle.Underline);
        }


        private void insertImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertImage();
        }

        private void InsertImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Insert Image";
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image image = Image.FromFile(openFileDialog.FileName);
                    Clipboard.SetImage(image);
                    richTextBox1.Paste();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InitializeContextMenu()
        {
            contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("Copy", null, copyToolStripMenuItem_Click);
            ToolStripMenuItem pasteMenuItem = new ToolStripMenuItem("Paste", null, pasteToolStripMenuItem_Click);
            ToolStripMenuItem cutMenuItem = new ToolStripMenuItem("Cut", null, cutToolStripMenuItem_Click);
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { copyMenuItem, pasteMenuItem, cutMenuItem });
            richTextBox1.ContextMenuStrip = contextMenuStrip;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText != string.Empty)
            {
                richTextBox1.Copy();
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText() || Clipboard.ContainsImage())
            {
                richTextBox1.Paste();
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText != string.Empty)
            {
                richTextBox1.Cut();
            }
        }

        private void darkModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyDarkMode();
        }

        private void lightModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyLightMode();
        }

        private void ApplyDarkMode()
        {
            this.BackColor = Color.Black;
            this.ForeColor = Color.White;
            this.richTextBox1.BackColor = Color.FromArgb(31, 31, 31);
            this.richTextBox1.ForeColor = Color.White;
            this.menuStrip1.BackColor = Color.FromArgb(31, 31, 31);
            this.menuStrip1.ForeColor = Color.Transparent;
            this.listBox1.BackColor = Color.FromArgb(46, 46, 46);
            this.listBox1.ForeColor = Color.White;
            this.Newnote.BackColor = Color.FromArgb(31, 31, 31);
            this.Newnote.ForeColor = Color.White;
            this.button1.BackColor = Color.FromArgb(31, 31, 31);
            this.button1.ForeColor = Color.White;
            this.listBox2.BackColor = Color.FromArgb(46, 46, 46);
            this.listBox2.ForeColor = Color.White;
            this.listBox3.BackColor = Color.FromArgb(31, 31, 31);
            this.listBox3.ForeColor = Color.White;

            foreach (ToolStripMenuItem item in this.menuStrip1.Items)
            {
                item.BackColor = Color.FromArgb(31, 31, 31);
                item.ForeColor = Color.FromArgb(105, 190, 255);
            }
        }

        private void ApplyLightMode()
        {
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            this.richTextBox1.BackColor = Color.White;
            this.richTextBox1.ForeColor = Color.Black;
            this.menuStrip1.BackColor = Color.LightGray;
            this.menuStrip1.ForeColor = Color.Black;

            foreach (ToolStripMenuItem item in this.menuStrip1.Items)
            {
                item.BackColor = Color.LightGray;
                item.ForeColor = Color.Black;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedFile = listBox1.SelectedItem.ToString();
                string filePath = Path.Combine(notesDirectory, selectedFile);
                if (File.Exists(filePath))
                {
                    richTextBox1.LoadFile(filePath, RichTextBoxStreamType.PlainText);
                    this.Text = selectedFile; // Update form title to match the file name
                }
            }
        }

        private void folderPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                notesDirectory = folderDialog.SelectedPath;
                LoadFilesIntoListBox(notesDirectory);
            }
        }

        private void LoadFilesIntoListBox(string folderPath)
        {
            listBox1.Items.Clear();
            try
            {
                string[] files = Directory.GetFiles(folderPath, "*.txt");
                foreach (string file in files)
                {
                    listBox1.Items.Add(Path.GetFileName(file));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading files: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Newnote_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(notesDirectory))
            {
                MessageBox.Show("Please select a folder first to create a new note.", "No Folder Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string newNoteName = "NewNote_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
                string newNotePath = Path.Combine(notesDirectory, newNoteName);
                File.WriteAllText(newNotePath, string.Empty);
                listBox1.Items.Add(newNoteName);
                richTextBox1.Clear();
                this.Text = newNoteName;
                MessageBox.Show("New note created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating new note: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCurrentNote()
        {
            if (string.IsNullOrEmpty(this.Text) || this.Text == "Untitled")
            {
                SaveAsCurrentNote();
            }
            else
            {
                try
                {
                    string filePath = Path.Combine(notesDirectory, this.Text);
                    richTextBox1.SaveFile(filePath, RichTextBoxStreamType.PlainText);
                    MessageBox.Show("File saved successfully!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveAsCurrentNote()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = saveFileDialog.FileName;
                    richTextBox1.SaveFile(filePath, RichTextBoxStreamType.PlainText);
                    this.Text = Path.GetFileName(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private Dictionary<IntPtr, WidgetForm> widgetForms = new Dictionary<IntPtr, WidgetForm>();



        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex >= 0) // Ensure a valid selection is made
            {
                string windowTitle = listBox2.SelectedItem.ToString();
                IntPtr windowHandle = FindWindowByCaption(windowTitle);

                if (windowHandle != IntPtr.Zero)
                {
                    if (!widgetForms.ContainsKey(windowHandle) || widgetForms[windowHandle].IsDisposed)
                    {
                        // Create a new widget form and associate it with the window handle
                        WidgetForm newWidget = new WidgetForm(windowHandle, this);
                        widgetForms[windowHandle] = newWidget;
                        newWidget.Show();
                    }
                    else
                    {
                        // Bring the existing widget to the front
                        WidgetForm existingWidget = widgetForms[windowHandle];
                        existingWidget.WindowState = FormWindowState.Normal;
                        existingWidget.BringToFront();
                    }

                    // Update pinned notes
                    if (!string.IsNullOrEmpty(this.Text)) // Ensure there's a note to pin
                    {
                        pinnedNotes[this.Text] = windowTitle; // Add or update the pin mapping
                        UpdatePinnedNotesList(); // Refresh the listBox3 display
                    }
                }
            }
        }
      




    private void PopulateOpenWindows()
        {
            listBox2.Items.Clear();
            EnumWindows(new EnumWindowsProc((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd) && GetWindowTextLength(hWnd) > 0)
                {
                    StringBuilder builder = new StringBuilder(GetWindowTextLength(hWnd) + 1);
                    GetWindowText(hWnd, builder, builder.Capacity);
                    listBox2.Items.Add(builder.ToString());
                }
                return true;
            }), IntPtr.Zero);
        }
        private void InitializeRefreshTimer()
        {
            refreshTimer = new Timer();
            refreshTimer.Interval = 5000; // 5 seconds
            refreshTimer.Tick += (sender, e) => PopulateOpenWindows();
            refreshTimer.Start();
        }
        private bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
        {
            int length = GetWindowTextLength(hWnd);
            if (length == 0) return true;

            StringBuilder builder = new StringBuilder(length + 1);
            GetWindowText(hWnd, builder, length + 1);
            listBox2.Items.Add(builder.ToString());

            return true;
        }
        private IntPtr FindWindowByCaption(string caption)
        {
            IntPtr hWnd = IntPtr.Zero;
            EnumWindows((wnd, param) =>
            {
                int length = GetWindowTextLength(wnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length + 1);
                GetWindowText(wnd, builder, length + 1);

                if (builder.ToString().Equals(caption))
                {
                    hWnd = wnd;
                    return false; // Stop enumerating windows
                }
                return true;
            }, IntPtr.Zero);
            return hWnd;
        }
        private Dictionary<string, string> pinnedNotes = new Dictionary<string, string>();

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

      

        [return: MarshalAs(UnmanagedType.Bool)]
        
       
        private void UpdatePinnedNotesList()
        {
            listBox3.Items.Clear();
            foreach (var entry in pinnedNotes)
            {
                listBox3.Items.Add($"{entry.Key} -> {entry.Value}");
            }
        }
        private void UpdateActiveWidgetsList()
        {
            listBox3.Items.Clear();
            foreach (var entry in widgetForms)
            {
                listBox3.Items.Add($"Widget for {pinnedNotes.FirstOrDefault(x => x.Value == entry.Key.ToString()).Key}");
            }
        }

        public void RemoveWidget(IntPtr windowHandle)
        {
            if (widgetForms.ContainsKey(windowHandle))
            {
                widgetForms.Remove(windowHandle);
            }
        }





        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}