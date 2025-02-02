namespace TableCloth2.Spork
{
    partial class StepControl
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel = new TableLayoutPanel();
            stateLabel = new Label();
            stepNameLabel = new Label();
            progressBar = new ProgressBar();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            tableLayoutPanel.Controls.Add(stateLabel, 0, 0);
            tableLayoutPanel.Controls.Add(stepNameLabel, 1, 0);
            tableLayoutPanel.Controls.Add(progressBar, 1, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Size = new Size(400, 50);
            tableLayoutPanel.TabIndex = 0;
            // 
            // stateLabel
            // 
            stateLabel.Anchor = AnchorStyles.Right;
            stateLabel.AutoSize = true;
            stateLabel.Location = new Point(37, 5);
            stateLabel.Name = "stateLabel";
            stateLabel.Size = new Size(40, 15);
            stateLabel.TabIndex = 0;
            stateLabel.Text = "Status";
            stateLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // stepNameLabel
            // 
            stepNameLabel.Anchor = AnchorStyles.Left;
            stepNameLabel.AutoSize = true;
            stepNameLabel.Location = new Point(83, 5);
            stepNameLabel.Name = "stepNameLabel";
            stepNameLabel.Size = new Size(67, 15);
            stepNameLabel.TabIndex = 1;
            stepNameLabel.Text = "Step Name";
            stepNameLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            progressBar.Dock = DockStyle.Fill;
            progressBar.Location = new Point(83, 28);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(314, 19);
            progressBar.TabIndex = 2;
            // 
            // StepControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            DoubleBuffered = true;
            MaximumSize = new Size(400, 50);
            MinimumSize = new Size(400, 50);
            Name = "StepControl";
            Size = new Size(400, 50);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private Label stateLabel;
        private Label stepNameLabel;
        private ProgressBar progressBar;
    }
}
