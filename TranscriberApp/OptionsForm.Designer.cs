namespace TranscriberApp;

partial class OptionsForm
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
        this.grpTranscriptionOptions = new System.Windows.Forms.GroupBox();
        this.lblLanguage = new System.Windows.Forms.Label();
        this.cboLanguage = new System.Windows.Forms.ComboBox();
        this.lblModelSize = new System.Windows.Forms.Label();
        this.cboModelSize = new System.Windows.Forms.ComboBox();
        this.chkHighQualityTranscription = new System.Windows.Forms.CheckBox();
        this.chkAddPunctuation = new System.Windows.Forms.CheckBox();
        this.btnOK = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        this.btnRestoreDefaults = new System.Windows.Forms.Button();
        this.grpTranscriptionOptions.SuspendLayout();
        this.SuspendLayout();
        
        // grpTranscriptionOptions
        this.grpTranscriptionOptions.Controls.Add(this.lblLanguage);
        this.grpTranscriptionOptions.Controls.Add(this.cboLanguage);
        this.grpTranscriptionOptions.Controls.Add(this.lblModelSize);
        this.grpTranscriptionOptions.Controls.Add(this.cboModelSize);
        this.grpTranscriptionOptions.Controls.Add(this.chkHighQualityTranscription);
        this.grpTranscriptionOptions.Controls.Add(this.chkAddPunctuation);
        this.grpTranscriptionOptions.Location = new System.Drawing.Point(20, 20);
        this.grpTranscriptionOptions.Name = "grpTranscriptionOptions";
        this.grpTranscriptionOptions.Size = new System.Drawing.Size(520, 180);
        this.grpTranscriptionOptions.TabIndex = 0;
        this.grpTranscriptionOptions.TabStop = false;
        this.grpTranscriptionOptions.Text = "Opcje transkrypcji";
        
        // lblLanguage
        this.lblLanguage.AutoSize = true;
        this.lblLanguage.Location = new System.Drawing.Point(20, 30);
        this.lblLanguage.Name = "lblLanguage";
        this.lblLanguage.Size = new System.Drawing.Size(45, 20);
        this.lblLanguage.TabIndex = 0;
        this.lblLanguage.Text = "Język:";
        
        // cboLanguage
        this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cboLanguage.FormattingEnabled = true;
        this.cboLanguage.Location = new System.Drawing.Point(180, 30);
        this.cboLanguage.Name = "cboLanguage";
        this.cboLanguage.Size = new System.Drawing.Size(320, 28);
        this.cboLanguage.TabIndex = 1;
        
        // lblModelSize
        this.lblModelSize.AutoSize = true;
        this.lblModelSize.Location = new System.Drawing.Point(20, 70);
        this.lblModelSize.Name = "lblModelSize";
        this.lblModelSize.Size = new System.Drawing.Size(116, 20);
        this.lblModelSize.TabIndex = 2;
        this.lblModelSize.Text = "Rozmiar modelu:";
        
        // cboModelSize
        this.cboModelSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cboModelSize.FormattingEnabled = true;
        this.cboModelSize.Location = new System.Drawing.Point(180, 70);
        this.cboModelSize.Name = "cboModelSize";
        this.cboModelSize.Size = new System.Drawing.Size(320, 28);
        this.cboModelSize.TabIndex = 3;
        
        // chkHighQualityTranscription
        this.chkHighQualityTranscription.AutoSize = true;
        this.chkHighQualityTranscription.Location = new System.Drawing.Point(20, 110);
        this.chkHighQualityTranscription.Name = "chkHighQualityTranscription";
        this.chkHighQualityTranscription.Size = new System.Drawing.Size(249, 24);
        this.chkHighQualityTranscription.TabIndex = 4;
        this.chkHighQualityTranscription.Text = "Wysoka jakość transkrypcji (wolniejsza)";
        this.chkHighQualityTranscription.UseVisualStyleBackColor = true;
        
        // chkAddPunctuation
        this.chkAddPunctuation.AutoSize = true;
        this.chkAddPunctuation.Checked = true;
        this.chkAddPunctuation.CheckState = System.Windows.Forms.CheckState.Checked;
        this.chkAddPunctuation.Location = new System.Drawing.Point(20, 140);
        this.chkAddPunctuation.Name = "chkAddPunctuation";
        this.chkAddPunctuation.Size = new System.Drawing.Size(229, 24);
        this.chkAddPunctuation.TabIndex = 5;
        this.chkAddPunctuation.Text = "Dodaj automatycznie interpunkcję";
        this.chkAddPunctuation.UseVisualStyleBackColor = true;
        
        // btnOK
        this.btnOK.Location = new System.Drawing.Point(340, 210);
        this.btnOK.Name = "btnOK";
        this.btnOK.Size = new System.Drawing.Size(94, 29);
        this.btnOK.TabIndex = 1;
        this.btnOK.Text = "OK";
        this.btnOK.UseVisualStyleBackColor = true;
        this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
        
        // btnCancel
        this.btnCancel.Location = new System.Drawing.Point(440, 210);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(94, 29);
        this.btnCancel.TabIndex = 2;
        this.btnCancel.Text = "Anuluj";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
        
        // btnRestoreDefaults - Przycisk przywracania ustawień fabrycznych
        this.btnRestoreDefaults.Location = new System.Drawing.Point(20, 210);
        this.btnRestoreDefaults.Name = "btnRestoreDefaults";
        this.btnRestoreDefaults.Size = new System.Drawing.Size(200, 29);
        this.btnRestoreDefaults.TabIndex = 3;
        this.btnRestoreDefaults.Text = "Przywróć ustawienia fabryczne";
        this.btnRestoreDefaults.UseVisualStyleBackColor = true;
        this.btnRestoreDefaults.Click += new System.EventHandler(this.btnRestoreDefaults_Click);
        
        // OptionsForm
        this.AcceptButton = this.btnOK;
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.btnCancel;
        this.ClientSize = new System.Drawing.Size(560, 250);
        this.Controls.Add(this.btnRestoreDefaults);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnOK);
        this.Controls.Add(this.grpTranscriptionOptions);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "OptionsForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Opcje transkrypcji";
        this.grpTranscriptionOptions.ResumeLayout(false);
        this.grpTranscriptionOptions.PerformLayout();
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.GroupBox grpTranscriptionOptions;
    private System.Windows.Forms.Label lblLanguage;
    private System.Windows.Forms.ComboBox cboLanguage;
    private System.Windows.Forms.Label lblModelSize;
    private System.Windows.Forms.ComboBox cboModelSize;
    private System.Windows.Forms.CheckBox chkHighQualityTranscription;
    private System.Windows.Forms.CheckBox chkAddPunctuation;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnRestoreDefaults;
} 