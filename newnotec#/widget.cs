using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace newnotec_
{
    public partial class WidgetForm : Form
    {
        private System.Windows.Forms.Label label1;
        private Form1 mainForm;

        public WidgetForm(Form1 form)
        {
            InitializeComponent();
            mainForm = form;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
            this.Click += new EventHandler(WidgetForm_Click);
        }

        private void WidgetForm_Load(object sender, EventArgs e)
        {
            UpdateText(mainForm.GetText());
        }

        public void UpdateText(string text)
        {
            label1.Text = text;
        }

        private void WidgetForm_Click(object sender, EventArgs e)
        {
            mainForm.RestoreFromWidget();
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(107, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // WidgetForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.label1);
            this.Name = "WidgetForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
