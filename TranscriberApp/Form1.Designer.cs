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
        components = new System.ComponentModel.Container();
        btnSelectFile = new System.Windows.Forms.Button();
        btnTranscribe = new System.Windows.Forms.Button();
        lblStatus = new System.Windows.Forms.Label();
        groupBoxExternalApps = new System.Windows.Forms.GroupBox();
        lblRecordingStatus = new System.Windows.Forms.Label();
        btnRecord = new System.Windows.Forms.Button();
        txtTranscription = new System.Windows.Forms.TextBox();
        btnClearTranscription = new System.Windows.Forms.Button();
        btnOpenTranscriptionsFolder = new System.Windows.Forms.Button();
        btnOptions = new System.Windows.Forms.Button();
        groupBoxExternalApps.SuspendLayout();
        SuspendLayout();
        
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
        
        // btnOptions - Nowy przycisk opcji
        this.btnOptions.Location = new System.Drawing.Point(370, 30);
        this.btnOptions.Name = "btnOptions";
        this.btnOptions.Size = new System.Drawing.Size(150, 30);
        this.btnOptions.TabIndex = 2;
        this.btnOptions.Text = "Opcje";
        this.btnOptions.UseVisualStyleBackColor = true;
        this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
        
        // Grupa opcji zewnętrznych aplikacji
        this.groupBoxExternalApps.Controls.Add(this.lblRecordingStatus);
        this.groupBoxExternalApps.Controls.Add(this.btnRecord);
        this.groupBoxExternalApps.Location = new System.Drawing.Point(30, 70);
        this.groupBoxExternalApps.Name = "groupBoxExternalApps";
        this.groupBoxExternalApps.Size = new System.Drawing.Size(520, 120);
        this.groupBoxExternalApps.TabIndex = 4;
        this.groupBoxExternalApps.TabStop = false;
        this.groupBoxExternalApps.Text = "Nagrywanie audio";
        
        // lblRecordingStatus
        this.lblRecordingStatus.Location = new System.Drawing.Point(20, 80);
        this.lblRecordingStatus.Name = "lblRecordingStatus";
        this.lblRecordingStatus.Size = new System.Drawing.Size(480, 20);
        this.lblRecordingStatus.TabIndex = 1;
        this.lblRecordingStatus.Text = "Status: Gotowy do nagrywania";
        
        // btnRecord
        this.btnRecord.Location = new System.Drawing.Point(20, 30);
        this.btnRecord.Name = "btnRecord";
        this.btnRecord.Size = new System.Drawing.Size(150, 30);
        this.btnRecord.TabIndex = 0;
        this.btnRecord.Text = "Nagraj dźwięk";
        this.btnRecord.UseVisualStyleBackColor = true;
        this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
        
        // lblStatus - przesuń pod grupę nagrywania
        this.lblStatus.AutoSize = true;
        this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.lblStatus.Location = new System.Drawing.Point(30, 200);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new System.Drawing.Size(520, 20);
        this.lblStatus.TabIndex = 5;
        this.lblStatus.Text = "Status: Gotowy";
        
        // Pole na wynik transkrypcji - przesuń wyżej, tam gdzie była grupa opcji
        this.txtTranscription.Multiline = true;
        this.txtTranscription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtTranscription.Location = new System.Drawing.Point(30, 230);
        this.txtTranscription.Size = new System.Drawing.Size(520, 300);
        this.txtTranscription.Name = "txtTranscription";
        this.txtTranscription.TabIndex = 7;
        this.txtTranscription.TextChanged += new System.EventHandler(this.txtTranscription_TextChanged);
        
        // Przycisk czyszczenia transkrypcji
        this.btnClearTranscription.Text = "Wyczyść transkrypcję";
        this.btnClearTranscription.Location = new System.Drawing.Point(30, 540);
        this.btnClearTranscription.Size = new System.Drawing.Size(150, 30);
        this.btnClearTranscription.Enabled = false;
        this.btnClearTranscription.Name = "btnClearTranscription";
        this.btnClearTranscription.TabIndex = 9;
        this.btnClearTranscription.Click += new System.EventHandler(this.btnClearTranscription_Click);
        
        // Przycisk otwierania folderu z transkrypcjami
        this.btnOpenTranscriptionsFolder.Text = "Pokaż w folderze";
        this.btnOpenTranscriptionsFolder.Location = new System.Drawing.Point(190, 540);
        this.btnOpenTranscriptionsFolder.Size = new System.Drawing.Size(150, 30);
        this.btnOpenTranscriptionsFolder.Name = "btnOpenTranscriptionsFolder";
        this.btnOpenTranscriptionsFolder.TabIndex = 10;
        this.btnOpenTranscriptionsFolder.Click += new System.EventHandler(this.btnOpenTranscriptionsFolder_Click);
        
        // Dodaj kontrolki do formularza
        this.Controls.Add(this.btnOptions);
        this.Controls.Add(this.btnOpenTranscriptionsFolder);
        this.Controls.Add(this.btnClearTranscription);
        this.Controls.Add(this.txtTranscription);
        this.Controls.Add(this.groupBoxExternalApps);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.btnTranscribe);
        this.Controls.Add(this.btnSelectFile);
        
        // Form1
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(584, 590);
        this.Name = "Form1";
        this.Text = "Aplikacja do transkrypcji";
        this.groupBoxExternalApps.ResumeLayout(false);
        this.groupBoxExternalApps.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
    
    private System.Windows.Forms.Button btnSelectFile;
    private System.Windows.Forms.Button btnTranscribe;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.GroupBox groupBoxExternalApps;
    private System.Windows.Forms.Label lblRecordingStatus;
    private System.Windows.Forms.Button btnRecord;
    private System.Windows.Forms.TextBox txtTranscription;
    private System.Windows.Forms.Button btnClearTranscription;
    private System.Windows.Forms.Button btnOpenTranscriptionsFolder;
    private System.Windows.Forms.Button btnOptions;
}
