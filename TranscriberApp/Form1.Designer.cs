namespace TranscriberApp;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.btnSelectFile = new System.Windows.Forms.Button();
        this.btnTranscribe = new System.Windows.Forms.Button();
        this.lblStatus = new System.Windows.Forms.Label();
        this.btnStartObsidian = new System.Windows.Forms.Button();
        this.lblRecordingStatus = new System.Windows.Forms.Label();
        this.groupBoxExternalApps = new System.Windows.Forms.GroupBox();
        this.btnRecord = new System.Windows.Forms.Button();
        this.grpTranscriptionOptions = new System.Windows.Forms.GroupBox();
        this.lblLanguage = new System.Windows.Forms.Label();
        this.cboLanguage = new System.Windows.Forms.ComboBox();
        this.chkHighQualityTranscription = new System.Windows.Forms.CheckBox();
        this.chkAddPunctuation = new System.Windows.Forms.CheckBox();
        this.txtTranscription = new System.Windows.Forms.TextBox();
        this.btnSaveTranscription = new System.Windows.Forms.Button();
        this.btnClearTranscription = new System.Windows.Forms.Button();
        this.progressBarTranscription = new System.Windows.Forms.ProgressBar();
        this.groupBoxExternalApps.SuspendLayout();
        this.grpTranscriptionOptions.SuspendLayout();
        this.SuspendLayout();
        
        // btnSelectFile
        this.btnSelectFile.Location = new System.Drawing.Point(30, 30);
        this.btnSelectFile.Name = "btnSelectFile";
        this.btnSelectFile.Size = new System.Drawing.Size(150, 30);
        this.btnSelectFile.TabIndex = 0;
        this.btnSelectFile.Text = "Wybierz plik";
        this.btnSelectFile.UseVisualStyleBackColor = true;
        this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
        
        // btnTranscribe
        this.btnTranscribe.Enabled = false;
        this.btnTranscribe.Location = new System.Drawing.Point(200, 30);
        this.btnTranscribe.Name = "btnTranscribe";
        this.btnTranscribe.Size = new System.Drawing.Size(150, 30);
        this.btnTranscribe.TabIndex = 1;
        this.btnTranscribe.Text = "Transkrybuj";
        this.btnTranscribe.UseVisualStyleBackColor = true;
        this.btnTranscribe.Click += new System.EventHandler(this.btnTranscribe_Click);
        
        // lblStatus
        this.lblStatus.AutoSize = true;
        this.lblStatus.Location = new System.Drawing.Point(30, 70);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new System.Drawing.Size(350, 20);
        this.lblStatus.TabIndex = 2;
        this.lblStatus.Text = "Status: Gotowy";
        
        // Grupa opcji zewnętrznych aplikacji
        this.groupBoxExternalApps.Controls.Add(this.btnRecord);
        this.groupBoxExternalApps.Controls.Add(this.lblRecordingStatus);
        this.groupBoxExternalApps.Controls.Add(this.btnStartObsidian);
        this.groupBoxExternalApps.Location = new System.Drawing.Point(30, 100);
        this.groupBoxExternalApps.Name = "groupBoxExternalApps";
        this.groupBoxExternalApps.Size = new System.Drawing.Size(520, 120);
        this.groupBoxExternalApps.TabIndex = 5;
        this.groupBoxExternalApps.TabStop = false;
        this.groupBoxExternalApps.Text = "Zewnętrzne aplikacje";
        
        // btnStartObsidian
        this.btnStartObsidian.Location = new System.Drawing.Point(20, 30);
        this.btnStartObsidian.Name = "btnStartObsidian";
        this.btnStartObsidian.Size = new System.Drawing.Size(150, 30);
        this.btnStartObsidian.TabIndex = 0;
        this.btnStartObsidian.Text = "Uruchom Obsidian";
        this.btnStartObsidian.UseVisualStyleBackColor = true;
        this.btnStartObsidian.Click += new System.EventHandler(this.btnStartObsidian_Click);
        
        // btnRecord
        this.btnRecord.Location = new System.Drawing.Point(200, 30);
        this.btnRecord.Name = "btnRecord";
        this.btnRecord.Size = new System.Drawing.Size(150, 30);
        this.btnRecord.TabIndex = 1;
        this.btnRecord.Text = "Nagraj dźwięk";
        this.btnRecord.UseVisualStyleBackColor = true;
        this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
        
        // lblRecordingStatus
        this.lblRecordingStatus.AutoSize = true;
        this.lblRecordingStatus.Location = new System.Drawing.Point(20, 80);
        this.lblRecordingStatus.Name = "lblRecordingStatus";
        this.lblRecordingStatus.Size = new System.Drawing.Size(350, 20);
        this.lblRecordingStatus.TabIndex = 4;
        this.lblRecordingStatus.Text = "Status: Obsidian nie jest uruchomiony";
        
        // Grupa opcji transkrypcji
        this.grpTranscriptionOptions.Text = "Opcje transkrypcji";
        this.grpTranscriptionOptions.Location = new System.Drawing.Point(30, 230);
        this.grpTranscriptionOptions.Size = new System.Drawing.Size(520, 120);
        this.grpTranscriptionOptions.Name = "grpTranscriptionOptions";
        this.grpTranscriptionOptions.TabIndex = 6;
        
        // lblLanguage
        this.lblLanguage.Text = "Język:";
        this.lblLanguage.AutoSize = true;
        this.lblLanguage.Location = new System.Drawing.Point(15, 25);
        this.lblLanguage.Name = "lblLanguage";
        this.lblLanguage.TabIndex = 0;
        
        // cboLanguage
        this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cboLanguage.Location = new System.Drawing.Point(150, 22);
        this.cboLanguage.Size = new System.Drawing.Size(200, 23);
        this.cboLanguage.Name = "cboLanguage";
        this.cboLanguage.TabIndex = 1;
        
        // chkHighQualityTranscription
        this.chkHighQualityTranscription.Text = "Wysoka jakość transkrypcji (wolniejsza)";
        this.chkHighQualityTranscription.AutoSize = true;
        this.chkHighQualityTranscription.Location = new System.Drawing.Point(15, 55);
        this.chkHighQualityTranscription.Name = "chkHighQualityTranscription";
        this.chkHighQualityTranscription.TabIndex = 2;
        
        // chkAddPunctuation
        this.chkAddPunctuation.Text = "Dodaj automatycznie interpunkcję";
        this.chkAddPunctuation.AutoSize = true;
        this.chkAddPunctuation.Location = new System.Drawing.Point(15, 85);
        this.chkAddPunctuation.Name = "chkAddPunctuation";
        this.chkAddPunctuation.TabIndex = 3;
        
        // Pole na wynik transkrypcji
        this.txtTranscription.Multiline = true;
        this.txtTranscription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtTranscription.Location = new System.Drawing.Point(30, 360);
        this.txtTranscription.Size = new System.Drawing.Size(520, 200);
        this.txtTranscription.Name = "txtTranscription";
        this.txtTranscription.TabIndex = 7;
        this.txtTranscription.TextChanged += new System.EventHandler(this.txtTranscription_TextChanged);
        
        // Przycisk zapisywania transkrypcji
        this.btnSaveTranscription.Text = "Zapisz transkrypcję";
        this.btnSaveTranscription.Location = new System.Drawing.Point(30, 570);
        this.btnSaveTranscription.Size = new System.Drawing.Size(150, 30);
        this.btnSaveTranscription.Enabled = false;
        this.btnSaveTranscription.Name = "btnSaveTranscription";
        this.btnSaveTranscription.TabIndex = 8;
        this.btnSaveTranscription.Click += new System.EventHandler(this.btnSaveTranscription_Click);
        
        // Przycisk czyszczenia transkrypcji
        this.btnClearTranscription = new System.Windows.Forms.Button();
        this.btnClearTranscription.Text = "Wyczyść transkrypcję";
        this.btnClearTranscription.Location = new System.Drawing.Point(190, 570);
        this.btnClearTranscription.Size = new System.Drawing.Size(150, 30);
        this.btnClearTranscription.Enabled = false;
        this.btnClearTranscription.Name = "btnClearTranscription";
        this.btnClearTranscription.TabIndex = 9;
        this.btnClearTranscription.Click += new System.EventHandler(this.btnClearTranscription_Click);
        
        // progressBarTranscription
        this.progressBarTranscription.Location = new System.Drawing.Point(370, 70);
        this.progressBarTranscription.Name = "progressBarTranscription";
        this.progressBarTranscription.Size = new System.Drawing.Size(180, 20);
        this.progressBarTranscription.TabIndex = 10;
        this.progressBarTranscription.Visible = false;
        
        // Dodaj kontrolki do formularza
        this.grpTranscriptionOptions.Controls.Add(this.lblLanguage);
        this.grpTranscriptionOptions.Controls.Add(this.cboLanguage);
        this.grpTranscriptionOptions.Controls.Add(this.chkHighQualityTranscription);
        this.grpTranscriptionOptions.Controls.Add(this.chkAddPunctuation);
        
        this.Controls.Add(this.btnSelectFile);
        this.Controls.Add(this.btnTranscribe);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.groupBoxExternalApps);
        this.Controls.Add(this.grpTranscriptionOptions);
        this.Controls.Add(this.txtTranscription);
        this.Controls.Add(this.btnSaveTranscription);
        this.Controls.Add(this.btnClearTranscription);
        this.Controls.Add(this.progressBarTranscription);
        
        // Form1
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(584, 620);
        this.Name = "Form1";
        this.Text = "Aplikacja do transkrypcji";
        this.groupBoxExternalApps.ResumeLayout(false);
        this.groupBoxExternalApps.PerformLayout();
        this.grpTranscriptionOptions.ResumeLayout(false);
        this.grpTranscriptionOptions.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
    
    private System.Windows.Forms.Button btnSelectFile;
    private System.Windows.Forms.Button btnTranscribe;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Button btnStartObsidian;
    private System.Windows.Forms.Label lblRecordingStatus;
    private System.Windows.Forms.GroupBox groupBoxExternalApps;
    private System.Windows.Forms.Button btnRecord;
    private System.Windows.Forms.GroupBox grpTranscriptionOptions;
    private System.Windows.Forms.Label lblLanguage;
    private System.Windows.Forms.ComboBox cboLanguage;
    private System.Windows.Forms.CheckBox chkHighQualityTranscription;
    private System.Windows.Forms.CheckBox chkAddPunctuation;
    private System.Windows.Forms.TextBox txtTranscription;
    private System.Windows.Forms.Button btnSaveTranscription;
    private System.Windows.Forms.Button btnClearTranscription;
    private System.Windows.Forms.ProgressBar progressBarTranscription;
}
