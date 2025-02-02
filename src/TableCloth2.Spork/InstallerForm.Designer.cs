namespace TableCloth2.Spork
{
    partial class InstallerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            cancelButton = new Button();
            tableLayoutPanel = new TableLayoutPanel();
            panel = new Panel();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Right;
            cancelButton.Location = new Point(294, 218);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "취소";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.AutoScroll = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(cancelButton, 0, 1);
            tableLayoutPanel.Controls.Add(panel, 0, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(6, 6);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 85F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel.Size = new Size(372, 249);
            tableLayoutPanel.TabIndex = 2;
            // 
            // panel
            // 
            panel.AutoScroll = true;
            panel.Dock = DockStyle.Fill;
            panel.Location = new Point(3, 3);
            panel.Name = "panel";
            panel.Size = new Size(366, 205);
            panel.TabIndex = 2;
            // 
            // InstallerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(384, 261);
            ControlBox = false;
            Controls.Add(tableLayoutPanel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MaximumSize = new Size(400, 300);
            MinimizeBox = false;
            MinimumSize = new Size(400, 300);
            Name = "InstallerForm";
            Padding = new Padding(6);
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Installing";
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button cancelButton;
        private TableLayoutPanel tableLayoutPanel;
        private Panel panel;
    }
}