namespace TableCloth2.TableCloth
{
    partial class TableClothForm
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
            detailInstructLabel = new Label();
            settingsButton = new Button();
            launchButton = new Button();
            instructLabel = new Label();
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // detailInstructLabel
            // 
            tableLayoutPanel.SetColumnSpan(detailInstructLabel, 2);
            detailInstructLabel.Dock = DockStyle.Fill;
            detailInstructLabel.Location = new Point(11, 35);
            detailInstructLabel.Name = "detailInstructLabel";
            detailInstructLabel.Padding = new Padding(2);
            detailInstructLabel.Size = new Size(282, 85);
            detailInstructLabel.TabIndex = 1;
            detailInstructLabel.Text = "시작 버튼을 눌러 가상 환경을 시작하거나, 설정 버튼을 눌러 필요한 설정을 변경할 수 있습니다.";
            // 
            // settingsButton
            // 
            settingsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            settingsButton.Location = new Point(13, 125);
            settingsButton.Margin = new Padding(5);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(80, 23);
            settingsButton.TabIndex = 2;
            settingsButton.Text = "설정(&S)...";
            settingsButton.UseVisualStyleBackColor = true;
            // 
            // launchButton
            // 
            launchButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            launchButton.Location = new Point(216, 125);
            launchButton.Margin = new Padding(5);
            launchButton.Name = "launchButton";
            launchButton.Size = new Size(75, 23);
            launchButton.TabIndex = 3;
            launchButton.Text = "시작(&L)...";
            launchButton.UseVisualStyleBackColor = true;
            // 
            // instructLabel
            // 
            tableLayoutPanel.SetColumnSpan(instructLabel, 2);
            instructLabel.Dock = DockStyle.Fill;
            instructLabel.Location = new Point(11, 8);
            instructLabel.Name = "instructLabel";
            instructLabel.Padding = new Padding(2);
            instructLabel.Size = new Size(282, 27);
            instructLabel.TabIndex = 0;
            instructLabel.Text = "안녕하세요!";
            instructLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(instructLabel, 0, 0);
            tableLayoutPanel.Controls.Add(launchButton, 1, 2);
            tableLayoutPanel.Controls.Add(detailInstructLabel, 0, 1);
            tableLayoutPanel.Controls.Add(settingsButton, 0, 2);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.Padding = new Padding(8);
            tableLayoutPanel.RowCount = 3;
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.Size = new Size(304, 161);
            tableLayoutPanel.TabIndex = 4;
            // 
            // TableClothForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(304, 161);
            Controls.Add(tableLayoutPanel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MaximumSize = new Size(320, 200);
            MinimizeBox = false;
            MinimumSize = new Size(320, 200);
            Name = "TableClothForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "식탁보 2.0";
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label detailInstructLabel;
        private Button settingsButton;
        private Button launchButton;
        private Label instructLabel;
        private TableLayoutPanel tableLayoutPanel;
    }
}