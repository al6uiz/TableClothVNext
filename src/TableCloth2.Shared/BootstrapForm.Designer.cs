namespace TableCloth2
{
    partial class BootstrapForm
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
            tableLayoutPanel = new TableLayoutPanel();
            statusLabel = new Label();
            progressBar = new ProgressBar();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(statusLabel, 0, 0);
            tableLayoutPanel.Controls.Add(progressBar, 0, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Size = new Size(304, 201);
            tableLayoutPanel.TabIndex = 0;
            // 
            // statusLabel
            // 
            statusLabel.Anchor = AnchorStyles.Bottom;
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(114, 75);
            statusLabel.Margin = new Padding(3, 0, 3, 10);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(75, 15);
            statusLabel.TabIndex = 0;
            statusLabel.Text = "In Progress...";
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Top;
            progressBar.Location = new Point(102, 110);
            progressBar.Margin = new Padding(3, 10, 3, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(100, 10);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 1;
            // 
            // BootstrapForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(304, 201);
            ControlBox = false;
            Controls.Add(tableLayoutPanel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MaximumSize = new Size(320, 240);
            MinimizeBox = false;
            MinimumSize = new Size(320, 240);
            Name = "BootstrapForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Preparing TableCloth App";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private Label statusLabel;
        private ProgressBar progressBar;
    }
}