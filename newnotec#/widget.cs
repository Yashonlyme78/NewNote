using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace newnotec_
{
    public partial class WidgetForm : Form
    {
        private RichTextBox richTextBox1;
        private Form1 mainForm;
        private IntPtr targetHandle;
        private Timer followTimer;
        private Control button1;

        public WidgetForm(IntPtr handle, Form1 form)
        {
            // Store references to the main form and target handle
            mainForm = form;
            targetHandle = handle;

            // Initialize the follow timer
            followTimer = new Timer
            {
                Interval = 100 // Update position every 100 milliseconds
            };
            followTimer.Tick += FollowTimer_Tick;

            // Set fixed dialog properties
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Adjust the widget size as needed
            this.Size = new Size(250, 400); // Fixed size; adjust as per requirements
            this.StartPosition = FormStartPosition.Manual; // Manual positioning for precise placement
            this.TopMost = true; // Ensure the widget stays on top of all windows

            // Initialize the components (RichTextBox and Save button)
            InitializeComponent();

            // Start the follow timer
            followTimer.Start();
        }

        private void WidgetForm_Load(object sender, EventArgs e)
        {
            UpdateText(mainForm.GetText());
        }

        public void UpdateText(string rtf)
        {
            // Update the content in the RichTextBox
            richTextBox1.Rtf = rtf;
        }

        private void FollowTimer_Tick(object sender, EventArgs e)
        {
            if (targetHandle == IntPtr.Zero)
            {
                followTimer.Stop();
                return;
            }

            // Check if the target window is minimized
            if (IsIconic(targetHandle))
            {
                if (this.WindowState != FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Minimized;
                }
            }
            else
            {
                // Restore to normal state if not minimized
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Open SaveFileDialog for saving content
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Rich Text Format (*.rtf)|*.rtf|Plain Text (*.txt)|*.txt";
                saveFileDialog.DefaultExt = ".txt";
                saveFileDialog.Title = "Save Note";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Save content based on selected file type
                        if (saveFileDialog.FilterIndex == 1) // Rich Text Format
                        {
                            richTextBox1.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.RichText);
                        }
                        else if (saveFileDialog.FilterIndex == 2) // Plain Text
                        {
                            richTextBox1.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
                        }

                        MessageBox.Show("Note saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Windows API functions
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            BringToFront(); // Ensure the widget stays on top
        }

        private void InitializeComponent()
        {
            this.richTextBox1 = new RichTextBox();
            this.button1 = new Button();
            this.SuspendLayout();

            // RichTextBox
            this.richTextBox1.BackColor = SystemColors.WindowFrame;
            this.richTextBox1.BorderStyle = BorderStyle.None;
            this.richTextBox1.Dock = DockStyle.Top;
            this.richTextBox1.ForeColor = SystemColors.Window;
            this.richTextBox1.Location = new Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new Size(250, 350);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";

            // Save Button (button1)
            this.button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.button1.Location = new Point(85, 360);
            this.button1.Name = "button1";
            this.button1.Size = new Size(80, 30);
            this.button1.TabIndex = 1;
            this.button1.Text = "Save";
            //this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);

            // WidgetForm
            this.ClientSize = new Size(250, 400);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox1);
            this.Name = "WidgetForm";
            this.Text = "WidgetForm";
            this.Load += new EventHandler(this.WidgetForm_Load);
            this.ResumeLayout(false);
        }
    }
}
