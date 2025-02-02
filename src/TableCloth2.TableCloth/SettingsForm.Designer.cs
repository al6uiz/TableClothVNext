namespace TableCloth2.TableCloth
{
    partial class SettingsForm
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
            tabControl = new TabControl();
            tableClothSettingsPage = new TabPage();
            folderMountList = new ListBox();
            enableFolderMount = new CheckBox();
            excludeAllFolderButton = new Button();
            excludeFolderButton = new Button();
            includeFolderButton = new Button();
            mountNPKICerts = new CheckBox();
            windowsSandboxSettingsPage = new TabPage();
            label3 = new Label();
            useCloudflareDns = new CheckBox();
            enableVirtualizedGpu = new CheckBox();
            enablePrinterRedirection = new CheckBox();
            enableVideoInput = new CheckBox();
            enableAudioInput = new CheckBox();
            analyticsPage = new TabPage();
            collectAnalytics = new CheckBox();
            collectSentryLog = new CheckBox();
            analyticsInstruction = new Label();
            sentryLogInstruction = new Label();
            tableLayoutPanel = new TableLayoutPanel();
            flowLayoutPanel = new FlowLayoutPanel();
            cancelButton = new Button();
            okayButton = new Button();
            folderBrowserDialog = new FolderBrowserDialog();
            tabControl.SuspendLayout();
            tableClothSettingsPage.SuspendLayout();
            windowsSandboxSettingsPage.SuspendLayout();
            analyticsPage.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            flowLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.Controls.Add(tableClothSettingsPage);
            tabControl.Controls.Add(windowsSandboxSettingsPage);
            tabControl.Controls.Add(analyticsPage);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(3, 3);
            tabControl.Multiline = true;
            tabControl.Name = "tabControl";
            tabControl.Padding = new Point(7, 7);
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(458, 390);
            tabControl.TabIndex = 0;
            // 
            // tableClothSettingsPage
            // 
            tableClothSettingsPage.Controls.Add(folderMountList);
            tableClothSettingsPage.Controls.Add(enableFolderMount);
            tableClothSettingsPage.Controls.Add(excludeAllFolderButton);
            tableClothSettingsPage.Controls.Add(excludeFolderButton);
            tableClothSettingsPage.Controls.Add(includeFolderButton);
            tableClothSettingsPage.Controls.Add(mountNPKICerts);
            tableClothSettingsPage.Location = new Point(4, 35);
            tableClothSettingsPage.Name = "tableClothSettingsPage";
            tableClothSettingsPage.Padding = new Padding(3);
            tableClothSettingsPage.Size = new Size(450, 351);
            tableClothSettingsPage.TabIndex = 0;
            tableClothSettingsPage.Text = "식탁보 동작";
            tableClothSettingsPage.UseVisualStyleBackColor = true;
            // 
            // folderMountList
            // 
            folderMountList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            folderMountList.FormattingEnabled = true;
            folderMountList.Location = new Point(40, 86);
            folderMountList.Name = "folderMountList";
            folderMountList.Size = new Size(401, 90);
            folderMountList.TabIndex = 2;
            // 
            // enableFolderMount
            // 
            enableFolderMount.AutoSize = true;
            enableFolderMount.Location = new Point(17, 52);
            enableFolderMount.Name = "enableFolderMount";
            enableFolderMount.Size = new Size(213, 19);
            enableFolderMount.TabIndex = 1;
            enableFolderMount.Text = "호스트 컴퓨터 폴더와 연결하기(&M)";
            enableFolderMount.UseVisualStyleBackColor = true;
            // 
            // excludeAllFolderButton
            // 
            excludeAllFolderButton.Location = new Point(268, 192);
            excludeAllFolderButton.Name = "excludeAllFolderButton";
            excludeAllFolderButton.Size = new Size(108, 23);
            excludeAllFolderButton.TabIndex = 5;
            excludeAllFolderButton.Text = "모두 제외(&A)";
            excludeAllFolderButton.UseVisualStyleBackColor = true;
            // 
            // excludeFolderButton
            // 
            excludeFolderButton.Location = new Point(154, 192);
            excludeFolderButton.Name = "excludeFolderButton";
            excludeFolderButton.Size = new Size(108, 23);
            excludeFolderButton.TabIndex = 4;
            excludeFolderButton.Text = "폴더 제외(&X)";
            excludeFolderButton.UseVisualStyleBackColor = true;
            // 
            // includeFolderButton
            // 
            includeFolderButton.Location = new Point(40, 192);
            includeFolderButton.Name = "includeFolderButton";
            includeFolderButton.Size = new Size(108, 23);
            includeFolderButton.TabIndex = 3;
            includeFolderButton.Text = "폴더 포함(&I)...";
            includeFolderButton.UseVisualStyleBackColor = true;
            // 
            // mountNPKICerts
            // 
            mountNPKICerts.AutoSize = true;
            mountNPKICerts.Location = new Point(17, 17);
            mountNPKICerts.Name = "mountNPKICerts";
            mountNPKICerts.Size = new Size(210, 19);
            mountNPKICerts.TabIndex = 0;
            mountNPKICerts.Text = "공동 인증서 자동으로 복사하기(&C)";
            mountNPKICerts.UseVisualStyleBackColor = true;
            // 
            // windowsSandboxSettingsPage
            // 
            windowsSandboxSettingsPage.Controls.Add(label3);
            windowsSandboxSettingsPage.Controls.Add(useCloudflareDns);
            windowsSandboxSettingsPage.Controls.Add(enableVirtualizedGpu);
            windowsSandboxSettingsPage.Controls.Add(enablePrinterRedirection);
            windowsSandboxSettingsPage.Controls.Add(enableVideoInput);
            windowsSandboxSettingsPage.Controls.Add(enableAudioInput);
            windowsSandboxSettingsPage.Location = new Point(4, 35);
            windowsSandboxSettingsPage.Name = "windowsSandboxSettingsPage";
            windowsSandboxSettingsPage.Padding = new Padding(3);
            windowsSandboxSettingsPage.Size = new Size(450, 351);
            windowsSandboxSettingsPage.TabIndex = 1;
            windowsSandboxSettingsPage.Text = "Windows Sandbox 동작";
            windowsSandboxSettingsPage.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(17, 161);
            label3.Name = "label3";
            label3.Size = new Size(59, 15);
            label3.TabIndex = 4;
            label3.Text = "문제 해결";
            // 
            // useCloudflareDns
            // 
            useCloudflareDns.AutoSize = true;
            useCloudflareDns.Location = new Point(17, 191);
            useCloudflareDns.Name = "useCloudflareDns";
            useCloudflareDns.Size = new Size(210, 19);
            useCloudflareDns.TabIndex = 5;
            useCloudflareDns.Text = "Cloudflare DNS 서버 대체 지정(&C)";
            useCloudflareDns.UseVisualStyleBackColor = true;
            // 
            // enableVirtualizedGpu
            // 
            enableVirtualizedGpu.AutoSize = true;
            enableVirtualizedGpu.Location = new Point(17, 119);
            enableVirtualizedGpu.Name = "enableVirtualizedGpu";
            enableVirtualizedGpu.Size = new Size(238, 19);
            enableVirtualizedGpu.TabIndex = 3;
            enableVirtualizedGpu.Text = "가상화된 그래픽 처리 장치 사용하기(&G)";
            enableVirtualizedGpu.UseVisualStyleBackColor = true;
            // 
            // enablePrinterRedirection
            // 
            enablePrinterRedirection.AutoSize = true;
            enablePrinterRedirection.Location = new Point(17, 85);
            enablePrinterRedirection.Name = "enablePrinterRedirection";
            enablePrinterRedirection.Size = new Size(157, 19);
            enablePrinterRedirection.TabIndex = 2;
            enablePrinterRedirection.Text = "프린터 같이 사용하기(&P)";
            enablePrinterRedirection.UseVisualStyleBackColor = true;
            // 
            // enableVideoInput
            // 
            enableVideoInput.AutoSize = true;
            enableVideoInput.Location = new Point(17, 51);
            enableVideoInput.Name = "enableVideoInput";
            enableVideoInput.Size = new Size(306, 19);
            enableVideoInput.TabIndex = 1;
            enableVideoInput.Text = "비디오 입력 사용(&V) - 개인 정보 노출에 주의하세요!";
            enableVideoInput.UseVisualStyleBackColor = true;
            // 
            // enableAudioInput
            // 
            enableAudioInput.AutoSize = true;
            enableAudioInput.Location = new Point(17, 17);
            enableAudioInput.Name = "enableAudioInput";
            enableAudioInput.Size = new Size(306, 19);
            enableAudioInput.TabIndex = 0;
            enableAudioInput.Text = "오디오 입력 사용(&A) - 개인 정보 노출에 주의하세요!";
            enableAudioInput.UseVisualStyleBackColor = true;
            // 
            // analyticsPage
            // 
            analyticsPage.Controls.Add(collectAnalytics);
            analyticsPage.Controls.Add(collectSentryLog);
            analyticsPage.Controls.Add(analyticsInstruction);
            analyticsPage.Controls.Add(sentryLogInstruction);
            analyticsPage.Location = new Point(4, 35);
            analyticsPage.Name = "analyticsPage";
            analyticsPage.Padding = new Padding(3);
            analyticsPage.Size = new Size(450, 351);
            analyticsPage.TabIndex = 2;
            analyticsPage.Text = "데이터 수집";
            analyticsPage.UseVisualStyleBackColor = true;
            // 
            // collectAnalytics
            // 
            collectAnalytics.AutoSize = true;
            collectAnalytics.Location = new Point(17, 287);
            collectAnalytics.Name = "collectAnalytics";
            collectAnalytics.Size = new Size(244, 19);
            collectAnalytics.TabIndex = 3;
            collectAnalytics.Text = "식탁보 데이터 및 사용 통계 자동 수집(&L)";
            collectAnalytics.UseVisualStyleBackColor = true;
            // 
            // collectSentryLog
            // 
            collectSentryLog.AutoSize = true;
            collectSentryLog.Location = new Point(17, 98);
            collectSentryLog.Name = "collectSentryLog";
            collectSentryLog.Size = new Size(189, 19);
            collectSentryLog.TabIndex = 1;
            collectSentryLog.Text = "식탁보 오류 로그 자동 수집(&R)";
            collectSentryLog.UseVisualStyleBackColor = true;
            // 
            // analyticsInstruction
            // 
            analyticsInstruction.Location = new Point(17, 169);
            analyticsInstruction.Name = "analyticsInstruction";
            analyticsInstruction.Size = new Size(411, 106);
            analyticsInstruction.TabIndex = 2;
            analyticsInstruction.Text = "식탁보의 기능과 품질을 향상시키기 위해 익명화된 데이터 및 사용 통계를 수집하고 있습니다. 이 정보는 제품 개선 및 사용자 경험 향상을 위한 목적으로만 사용되며, 개인을 식별할 수 있는 데이터는 수집되지 않습니다. 만약 데이터를 보내기를 원하지 않으시면 아래 체크 박스를 선택 해제하신 후 식탁보를 다시 실행해주세요.";
            // 
            // sentryLogInstruction
            // 
            sentryLogInstruction.Location = new Point(17, 17);
            sentryLogInstruction.Name = "sentryLogInstruction";
            sentryLogInstruction.Size = new Size(411, 69);
            sentryLogInstruction.TabIndex = 0;
            sentryLogInstruction.Text = "식탁보는 Sentry.io 서비스를 통하여 식탁보 프로그램 실행 도중 발생한 오류 정보를 익명으로 수집합니다. 만약 데이터 보내기를 원하지 않으시면 아래 체크 박스를 선택 해제하신 후 식탁보를 다시 실행해주세요.";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(tabControl, 0, 0);
            tableLayoutPanel.Controls.Add(flowLayoutPanel, 0, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(10, 10);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.Size = new Size(464, 441);
            tableLayoutPanel.TabIndex = 0;
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.AutoSize = true;
            flowLayoutPanel.Controls.Add(cancelButton);
            flowLayoutPanel.Controls.Add(okayButton);
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel.Location = new Point(3, 399);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Padding = new Padding(5);
            flowLayoutPanel.Size = new Size(458, 39);
            flowLayoutPanel.TabIndex = 1;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(370, 8);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "취소";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // okayButton
            // 
            okayButton.DialogResult = DialogResult.OK;
            okayButton.Location = new Point(289, 8);
            okayButton.Name = "okayButton";
            okayButton.Size = new Size(75, 23);
            okayButton.TabIndex = 0;
            okayButton.Text = "확인";
            okayButton.UseVisualStyleBackColor = true;
            // 
            // folderBrowserDialog
            // 
            folderBrowserDialog.Multiselect = true;
            // 
            // SettingsForm
            // 
            AcceptButton = okayButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(484, 461);
            Controls.Add(tableLayoutPanel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MaximumSize = new Size(500, 500);
            MinimizeBox = false;
            MinimumSize = new Size(500, 500);
            Name = "SettingsForm";
            Padding = new Padding(10);
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "식탁보 2.0 설정";
            tabControl.ResumeLayout(false);
            tableClothSettingsPage.ResumeLayout(false);
            tableClothSettingsPage.PerformLayout();
            windowsSandboxSettingsPage.ResumeLayout(false);
            windowsSandboxSettingsPage.PerformLayout();
            analyticsPage.ResumeLayout(false);
            analyticsPage.PerformLayout();
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            flowLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl;
        private TabPage tableClothSettingsPage;
        private TabPage windowsSandboxSettingsPage;
        private TableLayoutPanel tableLayoutPanel;
        private FlowLayoutPanel flowLayoutPanel;
        private Button cancelButton;
        private Button okayButton;
        private CheckBox mountNPKICerts;
        private Button includeFolderButton;
        private CheckBox enableFolderMount;
        private ListBox folderMountList;
        private Button excludeAllFolderButton;
        private Button excludeFolderButton;
        private TabPage analyticsPage;
        internal FolderBrowserDialog folderBrowserDialog;
        private CheckBox enablePrinterRedirection;
        private CheckBox enableVideoInput;
        private CheckBox enableAudioInput;
        private CheckBox enableVirtualizedGpu;
        private CheckBox collectSentryLog;
        private Label sentryLogInstruction;
        private Label analyticsInstruction;
        private CheckBox collectAnalytics;
        private Label label3;
        private CheckBox useCloudflareDns;
    }
}