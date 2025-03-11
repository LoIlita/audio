namespace TranscriberApp
{
    partial class RecordingOptionsForm
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
            this.grpQuality = new System.Windows.Forms.GroupBox();
            this.lblSampleRate = new System.Windows.Forms.Label();
            this.cboSampleRate = new System.Windows.Forms.ComboBox();
            this.lblQualityInfo = new System.Windows.Forms.Label();
            this.grpAudioSource = new System.Windows.Forms.GroupBox();
            this.lblDevice = new System.Windows.Forms.Label();
            this.cboDevice = new System.Windows.Forms.ComboBox();
            this.progressBarLevel = new System.Windows.Forms.ProgressBar();
            this.lblLevel = new System.Windows.Forms.Label();
            this.btnTestAudio = new System.Windows.Forms.Button();
            this.grpAdvancedOptions = new System.Windows.Forms.GroupBox();
            this.chkAlternativeMethod = new System.Windows.Forms.CheckBox();
            this.chkMixAudio = new System.Windows.Forms.CheckBox();
            this.grpMicrophoneOptions = new System.Windows.Forms.GroupBox();
            this.lblMicrophone = new System.Windows.Forms.Label();
            this.cboMicrophone = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRestoreDefaults = new System.Windows.Forms.Button();
            this.grpQuality.SuspendLayout();
            this.grpAudioSource.SuspendLayout();
            this.grpAdvancedOptions.SuspendLayout();
            this.grpMicrophoneOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpQuality
            // 
            this.grpQuality.Controls.Add(this.lblSampleRate);
            this.grpQuality.Controls.Add(this.cboSampleRate);
            this.grpQuality.Controls.Add(this.lblQualityInfo);
            this.grpQuality.Location = new System.Drawing.Point(12, 12);
            this.grpQuality.Name = "grpQuality";
            this.grpQuality.Size = new System.Drawing.Size(460, 100);
            this.grpQuality.TabIndex = 0;
            this.grpQuality.TabStop = false;
            this.grpQuality.Text = "Opcje jakości (dla lepszej transkrypcji)";
            // 
            // lblSampleRate
            // 
            this.lblSampleRate.AutoSize = true;
            this.lblSampleRate.Location = new System.Drawing.Point(10, 30);
            this.lblSampleRate.Name = "lblSampleRate";
            this.lblSampleRate.Size = new System.Drawing.Size(164, 20);
            this.lblSampleRate.TabIndex = 0;
            this.lblSampleRate.Text = "Częstotliwość próbkowania:";
            // 
            // cboSampleRate
            // 
            this.cboSampleRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSampleRate.FormattingEnabled = true;
            this.cboSampleRate.Items.AddRange(new object[] {
            "44.1 kHz (standard)",
            "48 kHz (wysoka jakość)",
            "96 kHz (najwyższa jakość)"});
            this.cboSampleRate.Location = new System.Drawing.Point(200, 27);
            this.cboSampleRate.Name = "cboSampleRate";
            this.cboSampleRate.Size = new System.Drawing.Size(250, 28);
            this.cboSampleRate.TabIndex = 1;
            this.cboSampleRate.SelectedIndexChanged += new System.EventHandler(this.cboSampleRate_SelectedIndexChanged);
            // 
            // lblQualityInfo
            // 
            this.lblQualityInfo.AutoSize = true;
            this.lblQualityInfo.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblQualityInfo.Location = new System.Drawing.Point(10, 65);
            this.lblQualityInfo.Name = "lblQualityInfo";
            this.lblQualityInfo.Size = new System.Drawing.Size(373, 20);
            this.lblQualityInfo.TabIndex = 2;
            this.lblQualityInfo.Text = "Wyższa jakość nagrania = lepsza transkrypcja, ale większe pliki";
            // 
            // grpAudioSource
            // 
            this.grpAudioSource.Controls.Add(this.lblDevice);
            this.grpAudioSource.Controls.Add(this.cboDevice);
            this.grpAudioSource.Controls.Add(this.progressBarLevel);
            this.grpAudioSource.Controls.Add(this.lblLevel);
            this.grpAudioSource.Controls.Add(this.btnTestAudio);
            this.grpAudioSource.Location = new System.Drawing.Point(12, 118);
            this.grpAudioSource.Name = "grpAudioSource";
            this.grpAudioSource.Size = new System.Drawing.Size(460, 110);
            this.grpAudioSource.TabIndex = 7;
            this.grpAudioSource.TabStop = false;
            this.grpAudioSource.Text = "Źródło dźwięku i poziom";
            // 
            // lblDevice
            // 
            this.lblDevice.AutoSize = true;
            this.lblDevice.Location = new System.Drawing.Point(10, 25);
            this.lblDevice.Name = "lblDevice";
            this.lblDevice.Size = new System.Drawing.Size(110, 20);
            this.lblDevice.TabIndex = 0;
            this.lblDevice.Text = "Źródło dźwięku:";
            // 
            // cboDevice
            // 
            this.cboDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDevice.FormattingEnabled = true;
            this.cboDevice.Location = new System.Drawing.Point(130, 22);
            this.cboDevice.Name = "cboDevice";
            this.cboDevice.Size = new System.Drawing.Size(320, 28);
            this.cboDevice.TabIndex = 1;
            this.cboDevice.SelectedIndexChanged += new System.EventHandler(this.cboDevice_SelectedIndexChanged);
            // 
            // progressBarLevel
            // 
            this.progressBarLevel.Location = new System.Drawing.Point(130, 60);
            this.progressBarLevel.Name = "progressBarLevel";
            this.progressBarLevel.Size = new System.Drawing.Size(200, 29);
            this.progressBarLevel.TabIndex = 2;
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(10, 65);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(108, 20);
            this.lblLevel.TabIndex = 3;
            this.lblLevel.Text = "Poziom dźwięku:";
            // 
            // btnTestAudio
            // 
            this.btnTestAudio.Location = new System.Drawing.Point(340, 60);
            this.btnTestAudio.Name = "btnTestAudio";
            this.btnTestAudio.Size = new System.Drawing.Size(110, 29);
            this.btnTestAudio.TabIndex = 8;
            this.btnTestAudio.Text = "Testuj dźwięk";
            this.btnTestAudio.UseVisualStyleBackColor = true;
            this.btnTestAudio.Click += new System.EventHandler(this.btnTestAudio_Click);
            // 
            // grpAdvancedOptions
            // 
            this.grpAdvancedOptions.Controls.Add(this.chkAlternativeMethod);
            this.grpAdvancedOptions.Controls.Add(this.chkMixAudio);
            this.grpAdvancedOptions.Location = new System.Drawing.Point(12, 234);
            this.grpAdvancedOptions.Name = "grpAdvancedOptions";
            this.grpAdvancedOptions.Size = new System.Drawing.Size(460, 85);
            this.grpAdvancedOptions.TabIndex = 8;
            this.grpAdvancedOptions.TabStop = false;
            this.grpAdvancedOptions.Text = "Opcje zaawansowane";
            // 
            // chkAlternativeMethod
            // 
            this.chkAlternativeMethod.AutoSize = true;
            this.chkAlternativeMethod.Location = new System.Drawing.Point(14, 26);
            this.chkAlternativeMethod.Name = "chkAlternativeMethod";
            this.chkAlternativeMethod.Size = new System.Drawing.Size(384, 24);
            this.chkAlternativeMethod.TabIndex = 1;
            this.chkAlternativeMethod.Text = "Użyj alternatywnej metody nagrywania (jeśli standardowa nie działa)";
            this.chkAlternativeMethod.UseVisualStyleBackColor = true;
            this.chkAlternativeMethod.CheckedChanged += new System.EventHandler(this.chkAlternativeMethod_CheckedChanged);
            // 
            // chkMixAudio
            // 
            this.chkMixAudio.AutoSize = true;
            this.chkMixAudio.Location = new System.Drawing.Point(14, 56);
            this.chkMixAudio.Name = "chkMixAudio";
            this.chkMixAudio.Size = new System.Drawing.Size(283, 24);
            this.chkMixAudio.TabIndex = 2;
            this.chkMixAudio.Text = "Nagrywaj jednocześnie mikrofon i dźwięk systemowy";
            this.chkMixAudio.UseVisualStyleBackColor = true;
            this.chkMixAudio.CheckedChanged += new System.EventHandler(this.chkMixAudio_CheckedChanged);
            // 
            // grpMicrophoneOptions
            // 
            this.grpMicrophoneOptions.Controls.Add(this.lblMicrophone);
            this.grpMicrophoneOptions.Controls.Add(this.cboMicrophone);
            this.grpMicrophoneOptions.Location = new System.Drawing.Point(12, 325);
            this.grpMicrophoneOptions.Name = "grpMicrophoneOptions";
            this.grpMicrophoneOptions.Size = new System.Drawing.Size(460, 75);
            this.grpMicrophoneOptions.TabIndex = 9;
            this.grpMicrophoneOptions.TabStop = false;
            this.grpMicrophoneOptions.Text = "Opcje mikrofonu";
            // 
            // lblMicrophone
            // 
            this.lblMicrophone.AutoSize = true;
            this.lblMicrophone.Location = new System.Drawing.Point(14, 30);
            this.lblMicrophone.Name = "lblMicrophone";
            this.lblMicrophone.Size = new System.Drawing.Size(122, 20);
            this.lblMicrophone.TabIndex = 3;
            this.lblMicrophone.Text = "Wybierz mikrofon:";
            // 
            // cboMicrophone
            // 
            this.cboMicrophone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMicrophone.FormattingEnabled = true;
            this.cboMicrophone.Location = new System.Drawing.Point(142, 27);
            this.cboMicrophone.Name = "cboMicrophone";
            this.cboMicrophone.Size = new System.Drawing.Size(300, 28);
            this.cboMicrophone.TabIndex = 4;
            this.cboMicrophone.SelectedIndexChanged += new System.EventHandler(this.cboMicrophone_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(122, 470);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(110, 35);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(252, 470);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 35);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Anuluj";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRestoreDefaults
            // 
            this.btnRestoreDefaults.Location = new System.Drawing.Point(20, 420);
            this.btnRestoreDefaults.Name = "btnRestoreDefaults";
            this.btnRestoreDefaults.Size = new System.Drawing.Size(200, 29);
            this.btnRestoreDefaults.TabIndex = 9;
            this.btnRestoreDefaults.Text = "Przywróć ustawienia fabryczne";
            this.btnRestoreDefaults.UseVisualStyleBackColor = true;
            this.btnRestoreDefaults.Click += new System.EventHandler(this.btnRestoreDefaults_Click);
            // 
            // RecordingOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 520);
            this.Controls.Add(this.btnRestoreDefaults);
            this.Controls.Add(this.grpMicrophoneOptions);
            this.Controls.Add(this.grpAdvancedOptions);
            this.Controls.Add(this.grpAudioSource);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpQuality);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecordingOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Opcje nagrywania";
            this.grpQuality.ResumeLayout(false);
            this.grpQuality.PerformLayout();
            this.grpAudioSource.ResumeLayout(false);
            this.grpAudioSource.PerformLayout();
            this.grpAdvancedOptions.ResumeLayout(false);
            this.grpAdvancedOptions.PerformLayout();
            this.grpMicrophoneOptions.ResumeLayout(false);
            this.grpMicrophoneOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpQuality;
        private System.Windows.Forms.Label lblSampleRate;
        private System.Windows.Forms.ComboBox cboSampleRate;
        private System.Windows.Forms.Label lblQualityInfo;
        private System.Windows.Forms.GroupBox grpAudioSource;
        private System.Windows.Forms.Label lblDevice;
        private System.Windows.Forms.ComboBox cboDevice;
        private System.Windows.Forms.ProgressBar progressBarLevel;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.GroupBox grpAdvancedOptions;
        private System.Windows.Forms.CheckBox chkAlternativeMethod;
        private System.Windows.Forms.CheckBox chkMixAudio;
        private System.Windows.Forms.GroupBox grpMicrophoneOptions;
        private System.Windows.Forms.Label lblMicrophone;
        private System.Windows.Forms.ComboBox cboMicrophone;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnTestAudio;
        private System.Windows.Forms.Button btnRestoreDefaults;
    }
} 