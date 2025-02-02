namespace TableCloth2.Spork
{
    partial class SporkForm
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
            components = new System.ComponentModel.Container();
            smallImageList = new ImageList(components);
            instructionLabel = new Label();
            listView = new ListView();
            largeImageList = new ImageList(components);
            tableLayoutPanel = new TableLayoutPanel();
            closeButton = new Button();
            flowLayoutPanel = new FlowLayoutPanel();
            launchButton = new Button();
            tableLayoutPanel.SuspendLayout();
            flowLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // smallImageList
            // 
            smallImageList.ColorDepth = ColorDepth.Depth32Bit;
            smallImageList.ImageSize = new Size(48, 48);
            smallImageList.TransparentColor = Color.Transparent;
            // 
            // instructionLabel
            // 
            instructionLabel.AutoSize = true;
            instructionLabel.Location = new Point(12, 12);
            instructionLabel.Margin = new Padding(12);
            instructionLabel.Name = "instructionLabel";
            instructionLabel.Size = new Size(425, 15);
            instructionLabel.TabIndex = 0;
            instructionLabel.Text = "접속하려는 웹 사이트를 선택하시면, 필요한 플러그인을 자동으로 설치합니다.";
            // 
            // listView
            // 
            listView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView.BorderStyle = BorderStyle.None;
            listView.FullRowSelect = true;
            listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView.LargeImageList = largeImageList;
            listView.Location = new Point(3, 42);
            listView.Name = "listView";
            listView.Size = new Size(794, 340);
            listView.SmallImageList = smallImageList;
            listView.Sorting = SortOrder.Ascending;
            listView.TabIndex = 1;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details;
            // 
            // largeImageList
            // 
            largeImageList.ColorDepth = ColorDepth.Depth32Bit;
            largeImageList.ImageSize = new Size(48, 48);
            largeImageList.TransparentColor = Color.Transparent;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(flowLayoutPanel, 0, 2);
            tableLayoutPanel.Controls.Add(instructionLabel, 0, 0);
            tableLayoutPanel.Controls.Add(listView, 0, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 3;
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel.Size = new Size(800, 450);
            tableLayoutPanel.TabIndex = 3;
            // 
            // closeButton
            // 
            closeButton.Anchor = AnchorStyles.Right;
            closeButton.Location = new Point(686, 9);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(75, 23);
            closeButton.TabIndex = 2;
            closeButton.Text = "닫기(&C)";
            closeButton.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.AutoSize = true;
            flowLayoutPanel.Controls.Add(closeButton);
            flowLayoutPanel.Controls.Add(launchButton);
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel.Location = new Point(12, 397);
            flowLayoutPanel.Margin = new Padding(12);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Padding = new Padding(6);
            flowLayoutPanel.Size = new Size(776, 41);
            flowLayoutPanel.TabIndex = 5;
            // 
            // launchButton
            // 
            launchButton.Anchor = AnchorStyles.Right;
            launchButton.Location = new Point(605, 9);
            launchButton.Name = "launchButton";
            launchButton.Size = new Size(75, 23);
            launchButton.TabIndex = 3;
            launchButton.Text = "실행(&L)";
            launchButton.UseVisualStyleBackColor = true;
            // 
            // SporkForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel);
            DoubleBuffered = true;
            Name = "SporkForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Spork";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            flowLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ImageList smallImageList;
        private Label instructionLabel;
        private ListView listView;
        private ImageList largeImageList;
        private TableLayoutPanel tableLayoutPanel;
        private FlowLayoutPanel flowLayoutPanel;
        private Button closeButton;
        private Button launchButton;
    }
}