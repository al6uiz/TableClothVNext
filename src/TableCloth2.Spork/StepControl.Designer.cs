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
            resultLabel = new Label();
            label1 = new Label();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(stateLabel, 0, 0);
            tableLayoutPanel.Controls.Add(stepNameLabel, 1, 0);
            tableLayoutPanel.Controls.Add(resultLabel, 1, 1);
            tableLayoutPanel.Controls.Add(label1, 0, 2);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 3;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.Size = new Size(200, 50);
            tableLayoutPanel.TabIndex = 0;
            // 
            // stateLabel
            // 
            stateLabel.Anchor = AnchorStyles.Right;
            stateLabel.AutoSize = true;
            stateLabel.Location = new Point(3, 16);
            stateLabel.Name = "stateLabel";
            tableLayoutPanel.SetRowSpan(stateLabel, 2);
            stateLabel.Size = new Size(40, 15);
            stateLabel.TabIndex = 0;
            stateLabel.Text = "Status";
            stateLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // stepNameLabel
            // 
            stepNameLabel.Anchor = AnchorStyles.Left;
            stepNameLabel.AutoSize = true;
            stepNameLabel.Location = new Point(49, 4);
            stepNameLabel.Name = "stepNameLabel";
            stepNameLabel.Size = new Size(67, 15);
            stepNameLabel.TabIndex = 1;
            stepNameLabel.Text = "Step Name";
            stepNameLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // resultLabel
            // 
            resultLabel.Anchor = AnchorStyles.Right;
            resultLabel.AutoSize = true;
            resultLabel.Location = new Point(158, 28);
            resultLabel.Name = "resultLabel";
            resultLabel.Size = new Size(39, 15);
            resultLabel.TabIndex = 2;
            resultLabel.Text = "Result";
            resultLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            label1.BorderStyle = BorderStyle.Fixed3D;
            tableLayoutPanel.SetColumnSpan(label1, 2);
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(3, 48);
            label1.Name = "label1";
            label1.Size = new Size(194, 2);
            label1.TabIndex = 3;
            // 
            // StepControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            DoubleBuffered = true;
            Name = "StepControl";
            Size = new Size(200, 50);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private Label stateLabel;
        private Label stepNameLabel;
        private Label resultLabel;
        private Label label1;
    }
}
