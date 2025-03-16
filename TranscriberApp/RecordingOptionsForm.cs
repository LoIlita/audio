using System;
using System.Drawing;
using System.Windows.Forms;
using NAudio.Wave;
using System.Collections.Generic;

namespace TranscriberApp
{
    public partial class RecordingOptionsForm : Form
    {
        private RecorderForm parentForm;
        private WaveInEvent? waveIn;
        private WasapiLoopbackCapture? loopbackCapture;
        private System.Windows.Forms.Timer levelUpdateTimer;
        
        // Właściwości do przechowywania wybranych opcji
        public bool UseAlternativeMethod { get; private set; } = false;
        public bool MixAudioSources { get; private set; } = true;
        public int SelectedMicrophoneIndex { get; private set; } = -1;
        public int SampleRate { get; private set; } = 44100;
        public int SelectedDeviceIndex { get; private set; } = 0;
        public bool IsTestingAudio { get; private set; } = false;
        
        public RecordingOptionsForm(RecorderForm parent, bool useAlternativeMethod, bool mixAudioSources, 
                                   int selectedMicrophoneIndex, int sampleRate)
        {
            InitializeComponent();
            parentForm = parent;
            
            // Ustaw początkowe wartości
            UseAlternativeMethod = useAlternativeMethod;
            MixAudioSources = mixAudioSources;
            SelectedMicrophoneIndex = selectedMicrophoneIndex;
            SampleRate = sampleRate;
            
            // Ustawienie wartości kontrolek
            chkAlternativeMethod.Checked = UseAlternativeMethod;
            chkMixAudio.Checked = MixAudioSources;
            
            // Inicjalizacja listy urządzeń audio
            LoadAudioDevices();
            
            // Inicjalizacja listy mikrofonów - zawsze ładujemy mikrofony
            LoadMicrophones();
            
            // Ustaw wybraną częstotliwość próbkowania
            switch (SampleRate)
            {
                case 44100: cboSampleRate.SelectedIndex = 0; break;
                case 48000: cboSampleRate.SelectedIndex = 1; break;
                case 96000: cboSampleRate.SelectedIndex = 2; break;
                default: cboSampleRate.SelectedIndex = 0; break;
            }
            
            // Aktualizuj widoczność kontrolek mikrofonu
            UpdateMicrophoneControlsVisibility();
            
            // Inicjalizacja timera do aktualizacji poziomu dźwięku
            levelUpdateTimer = new System.Windows.Forms.Timer();
            levelUpdateTimer.Interval = 50; // 50ms
            levelUpdateTimer.Tick += LevelUpdateTimer_Tick;
        }
        
        // Nowa metoda do ładowania mikrofonów
        private void LoadMicrophones()
        {
            try
            {
                // Wyczyść listę mikrofonów
                cboMicrophone.Items.Clear();
                
                // Dodaj dostępne mikrofony
                for (int i = 0; i < WaveInEvent.DeviceCount; i++)
                {
                    var capabilities = WaveInEvent.GetCapabilities(i);
                    cboMicrophone.Items.Add($"{capabilities.ProductName}");
                }
                
                // Jeśli jest wybrany mikrofon, zaznacz go
                if (SelectedMicrophoneIndex >= 0 && SelectedMicrophoneIndex < cboMicrophone.Items.Count)
                {
                    cboMicrophone.SelectedIndex = SelectedMicrophoneIndex;
                }
                else if (cboMicrophone.Items.Count > 0)
                {
                    cboMicrophone.SelectedIndex = 0;
                    SelectedMicrophoneIndex = 0;
                }
                
                // Jeśli nie ma mikrofonów, poinformuj użytkownika
                if (cboMicrophone.Items.Count == 0)
                {
                    MessageBox.Show("Nie znaleziono żadnych mikrofonów. Upewnij się, że mikrofon jest podłączony.", 
                        "Brak mikrofonów", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania mikrofonów: {ex.Message}", 
                    "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Ładowanie urządzeń audio
        private void LoadAudioDevices()
        {
            cboDevice.Items.Clear();
            
            try
            {
                // Dodaj opcję nagrywania dźwięku systemowego
                cboDevice.Items.Add("Dźwięk systemowy");
                
                // Dodaj urządzenia wejściowe (mikrofony)
                for (int i = 0; i < WaveInEvent.DeviceCount; i++)
                {
                    var capabilities = WaveInEvent.GetCapabilities(i);
                    cboDevice.Items.Add($"Mikrofon: {capabilities.ProductName}");
                }

                if (cboDevice.Items.Count > 0)
                {
                    cboDevice.SelectedIndex = SelectedDeviceIndex; // Domyślnie wybierz dźwięk systemowy
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania urządzeń audio: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Obsługa zmian w wyborze urządzenia audio
        private void cboDevice_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SelectedDeviceIndex = cboDevice.SelectedIndex;
        }
        
        // Obsługa przycisku testowania dźwięku
        private void btnTestAudio_Click(object? sender, EventArgs e)
        {
            if (IsTestingAudio)
            {
                StopAudioTest();
                btnTestAudio.Text = "Testuj dźwięk";
            }
            else
            {
                StartAudioTest();
                btnTestAudio.Text = "Zatrzymaj test";
            }
        }
        
        // Rozpoczęcie testu dźwięku
        private void StartAudioTest()
        {
            try
            {
                // Sprawdź, czy wybrano dźwięk systemowy (loopback)
                bool isLoopbackMode = cboDevice.SelectedIndex == 0;
                
                if (isLoopbackMode)
                {
                    // Nagrywanie dźwięku systemowego
                    loopbackCapture = new WasapiLoopbackCapture();
                    loopbackCapture.DataAvailable += LoopbackCapture_DataAvailable;
                    loopbackCapture.StartRecording();
                }
                else
                {
                    // Nagrywanie z mikrofonu
                    waveIn = new WaveInEvent();
                    waveIn.DeviceNumber = cboDevice.SelectedIndex - 1; // Odejmujemy 1, bo pierwszy element to loopback
                    waveIn.WaveFormat = new WaveFormat(44100, 1); // 44.1kHz, mono
                    waveIn.DataAvailable += WaveIn_DataAvailable;
                    waveIn.StartRecording();
                }
                
                IsTestingAudio = true;
                levelUpdateTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas testowania dźwięku: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Zatrzymanie testu dźwięku
        private void StopAudioTest()
        {
            try
            {
                levelUpdateTimer.Stop();
                
                if (waveIn != null)
                {
                    waveIn.StopRecording();
                    waveIn.Dispose();
                    waveIn = null;
                }
                
                if (loopbackCapture != null)
                {
                    loopbackCapture.StopRecording();
                    loopbackCapture.Dispose();
                    loopbackCapture = null;
                }
                
                IsTestingAudio = false;
                progressBarLevel.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zatrzymywania testu dźwięku: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Obsługa danych z mikrofonu
        private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            try
            {
                if (e.BytesRecorded > 0)
                {
                    // Aktualizacja wskaźnika poziomu dźwięku
                    int max = 0;
                    for (int i = 0; i < e.BytesRecorded; i += 2)
                    {
                        int sample = Math.Abs(BitConverter.ToInt16(e.Buffer, i));
                        if (sample > max) max = sample;
                    }
                    
                    // Konwersja na skalę 0-100 dla ProgressBar
                    int level = max * 100 / 32768;
                    
                    // Zapisz wartość do wykorzystania przez timer
                    currentLevel = level;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w WaveIn_DataAvailable: {ex.Message}");
                // Nie wyświetlamy MessageBox, bo ta metoda jest wywoływana często
            }
        }
        
        // Obsługa danych z dźwięku systemowego
        private void LoopbackCapture_DataAvailable(object? sender, WaveInEventArgs e)
        {
            try
            {
                if (e.BytesRecorded > 0)
                {
                    // Aktualizacja wskaźnika poziomu dźwięku
                    int max = 0;
                    for (int i = 0; i < e.BytesRecorded; i += 2)
                    {
                        int sample = Math.Abs(BitConverter.ToInt16(e.Buffer, i));
                        if (sample > max) max = sample;
                    }
                    
                    // Konwersja na skalę 0-100 dla ProgressBar
                    int level = max * 100 / 32768;
                    
                    // Zapisz wartość do wykorzystania przez timer
                    currentLevel = level;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w LoopbackCapture_DataAvailable: {ex.Message}");
            }
        }
        
        // Zmienna do przechowywania aktualnego poziomu dźwięku
        private volatile int currentLevel = 0;
        
        // Timer do aktualizacji wskaźnika poziomu dźwięku w UI
        private void LevelUpdateTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                progressBarLevel.Value = Math.Min(currentLevel, 100);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w LevelUpdateTimer_Tick: {ex.Message}");
            }
        }
        
        private void UpdateMicrophoneControlsVisibility()
        {
            // Pokazuj lub ukrywaj całą grupę opcji mikrofonu zależnie od tego czy wybrano miksowanie audio
            grpMicrophoneOptions.Visible = chkMixAudio.Checked;
        }
        
        private void chkMixAudio_CheckedChanged(object? sender, EventArgs e)
        {
            MixAudioSources = chkMixAudio.Checked;
            
            // Zapisz ustawienie do AppSettings
            var settings = AppSettings.Load();
            settings.MixAudioSources = MixAudioSources;
            settings.Save();
            
            // Jeśli opcja miksowania została włączona, załaduj listę mikrofonów
            if (MixAudioSources)
            {
                LoadMicrophones();
            }
            
            UpdateMicrophoneControlsVisibility();
        }
        
        private void chkAlternativeMethod_CheckedChanged(object? sender, EventArgs e)
        {
            UseAlternativeMethod = chkAlternativeMethod.Checked;
        }
        
        private void cboMicrophone_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SelectedMicrophoneIndex = cboMicrophone.SelectedIndex;
        }
        
        private void cboSampleRate_SelectedIndexChanged(object? sender, EventArgs e)
        {
            switch (cboSampleRate.SelectedIndex)
            {
                case 0: SampleRate = 44100; break;
                case 1: SampleRate = 48000; break;
                case 2: SampleRate = 96000; break;
            }
        }
        
        private void btnRestoreDefaults_Click(object? sender, EventArgs e)
        {
            // Przywracanie domyślnych ustawień
            UseAlternativeMethod = false;
            
            // Wczytaj domyślne ustawienie MixAudioSources z AppSettings
            MixAudioSources = AppSettings.DefaultMixAudioSources;
            
            SelectedMicrophoneIndex = 0;
            SampleRate = 44100;
            SelectedDeviceIndex = 0;
            
            // Aktualizacja kontrolek
            chkAlternativeMethod.Checked = false;
            chkMixAudio.Checked = MixAudioSources; // Użyj wartości z AppSettings
            cboSampleRate.SelectedIndex = 0;
            
            if (cboDevice.Items.Count > 0)
                cboDevice.SelectedIndex = 0;
                
            if (cboMicrophone.Items.Count > 0)
                cboMicrophone.SelectedIndex = 0;
                
            // Aktualizacja widoczności
            UpdateMicrophoneControlsVisibility();
            
            MessageBox.Show("Przywrócono domyślne ustawienia", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void btnOK_Click(object? sender, EventArgs e)
        {
            // Zatrzymaj test dźwięku, jeśli trwa
            if (IsTestingAudio)
            {
                StopAudioTest();
            }
            
            // Przekaż ustawienia do formularza nagrywania
            parentForm.UpdateOptions(
                UseAlternativeMethod,
                MixAudioSources,
                SelectedMicrophoneIndex,
                SampleRate,
                SelectedDeviceIndex
            );
            
            DialogResult = DialogResult.OK;
            Close();
        }
        
        private void btnCancel_Click(object? sender, EventArgs e)
        {
            // Zatrzymaj test dźwięku, jeśli trwa
            if (IsTestingAudio)
            {
                StopAudioTest();
            }
            
            DialogResult = DialogResult.Cancel;
            Close();
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Zatrzymaj test dźwięku, jeśli trwa
            if (IsTestingAudio)
            {
                StopAudioTest();
            }
            
            base.OnFormClosing(e);
        }
    }
} 