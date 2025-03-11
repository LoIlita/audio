using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using NAudio.Wave.SampleProviders;
using NAudio.Wave.Asio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace TranscriberApp
{
    /// <summary>
    /// Implementacja filtra górnoprzepustowego dla dźwięku.
    /// </summary>
    public class HighPassFilterSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly float cutoffFrequency;
        private readonly float q;
        private float[] filterCoeffs = new float[5];
        private float[] xv = Array.Empty<float>();
        private float[] yv = Array.Empty<float>();

        public HighPassFilterSampleProvider(ISampleProvider source, float cutoffFrequency, float q = 1.0f)
        {
            this.source = source;
            this.cutoffFrequency = cutoffFrequency;
            this.q = q;
            this.WaveFormat = source.WaveFormat;
            
            // Inicjalizacja filtra
            InitializeFilter();
        }

        public WaveFormat WaveFormat { get; }

        private void InitializeFilter()
        {
            // Współczynniki dla filtra górnoprzepustowego
            float sampleRate = WaveFormat.SampleRate;
            float w0 = 2 * (float)Math.PI * cutoffFrequency / sampleRate;
            float alpha = (float)Math.Sin(w0) / (2 * q);
            
            // Obliczenie współczynników filtra
            filterCoeffs = new float[5];
            filterCoeffs[0] = (1 + (float)Math.Cos(w0)) / 2;
            filterCoeffs[1] = -(1 + (float)Math.Cos(w0));
            filterCoeffs[2] = (1 + (float)Math.Cos(w0)) / 2;
            filterCoeffs[3] = 1 + alpha;
            filterCoeffs[4] = -2 * (float)Math.Cos(w0);
            
            // Normalizacja współczynników
            float a0 = filterCoeffs[3];
            filterCoeffs[0] /= a0;
            filterCoeffs[1] /= a0;
            filterCoeffs[2] /= a0;
            filterCoeffs[4] /= a0;
            
            // Inicjalizacja buforów
            xv = new float[3 * WaveFormat.Channels];
            yv = new float[3 * WaveFormat.Channels];
        }

        public int Read(float[] buffer, int offset, int count)
        {
            // Odczytaj próbki z źródła
            int samplesRead = source.Read(buffer, offset, count);
            
            // Zastosuj filtr do każdej próbki
            for (int i = 0; i < samplesRead; i++)
            {
                int channelIndex = i % WaveFormat.Channels;
                int channelOffset = channelIndex * 3;
                
                // Przesuń bufory
                xv[channelOffset] = xv[channelOffset + 1];
                xv[channelOffset + 1] = xv[channelOffset + 2];
                xv[channelOffset + 2] = buffer[offset + i];
                
                yv[channelOffset] = yv[channelOffset + 1];
                yv[channelOffset + 1] = yv[channelOffset + 2];
                
                // Zastosuj filtr
                yv[channelOffset + 2] = filterCoeffs[0] * xv[channelOffset + 2] +
                                        filterCoeffs[1] * xv[channelOffset + 1] +
                                        filterCoeffs[2] * xv[channelOffset] -
                                        filterCoeffs[4] * yv[channelOffset + 1] -
                                        yv[channelOffset];
                
                // Zapisz wynik
                buffer[offset + i] = yv[channelOffset + 2];
            }
            
            return samplesRead;
        }
    }

    /// <summary>
    /// Implementacja filtra dolnoprzepustowego dla dźwięku.
    /// </summary>
    public class LowPassFilterSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly float cutoffFrequency;
        private readonly float q;
        private float[] filterCoeffs = new float[5];
        private float[] xv = Array.Empty<float>();
        private float[] yv = Array.Empty<float>();

        public LowPassFilterSampleProvider(ISampleProvider source, float cutoffFrequency, float q = 1.0f)
        {
            this.source = source;
            this.cutoffFrequency = cutoffFrequency;
            this.q = q;
            this.WaveFormat = source.WaveFormat;
            
            // Inicjalizacja filtra
            InitializeFilter();
        }

        public WaveFormat WaveFormat { get; }

        private void InitializeFilter()
        {
            // Współczynniki dla filtra dolnoprzepustowego
            float sampleRate = WaveFormat.SampleRate;
            float w0 = 2 * (float)Math.PI * cutoffFrequency / sampleRate;
            float alpha = (float)Math.Sin(w0) / (2 * q);
            
            // Obliczenie współczynników filtra
            filterCoeffs = new float[5];
            filterCoeffs[0] = (1 - (float)Math.Cos(w0)) / 2;
            filterCoeffs[1] = 1 - (float)Math.Cos(w0);
            filterCoeffs[2] = (1 - (float)Math.Cos(w0)) / 2;
            filterCoeffs[3] = 1 + alpha;
            filterCoeffs[4] = -2 * (float)Math.Cos(w0);
            
            // Normalizacja współczynników
            float a0 = filterCoeffs[3];
            filterCoeffs[0] /= a0;
            filterCoeffs[1] /= a0;
            filterCoeffs[2] /= a0;
            filterCoeffs[4] /= a0;
            
            // Inicjalizacja buforów
            xv = new float[3 * WaveFormat.Channels];
            yv = new float[3 * WaveFormat.Channels];
        }

        public int Read(float[] buffer, int offset, int count)
        {
            // Odczytaj próbki z źródła
            int samplesRead = source.Read(buffer, offset, count);
            
            // Zastosuj filtr do każdej próbki
            for (int i = 0; i < samplesRead; i++)
            {
                int channelIndex = i % WaveFormat.Channels;
                int channelOffset = channelIndex * 3;
                
                // Przesuń bufory
                xv[channelOffset] = xv[channelOffset + 1];
                xv[channelOffset + 1] = xv[channelOffset + 2];
                xv[channelOffset + 2] = buffer[offset + i];
                
                yv[channelOffset] = yv[channelOffset + 1];
                yv[channelOffset + 1] = yv[channelOffset + 2];
                
                // Zastosuj filtr
                yv[channelOffset + 2] = filterCoeffs[0] * xv[channelOffset + 2] +
                                        filterCoeffs[1] * xv[channelOffset + 1] +
                                        filterCoeffs[2] * xv[channelOffset] -
                                        filterCoeffs[4] * yv[channelOffset + 1] -
                                        yv[channelOffset];
                
                // Zapisz wynik
                buffer[offset + i] = yv[channelOffset + 2];
            }
            
            return samplesRead;
        }
    }

    public partial class RecorderForm : Form
    {
        private WaveInEvent? waveIn;
        private WasapiLoopbackCapture? loopbackCapture;
        private WasapiOut? wasapiOut;
        private WaveFileWriter? writer;
        private string outputFilePath = string.Empty;
        private bool isRecording = false;
        private Form1 parentForm;
        private bool isLoopbackMode = false;
        private bool useAlternativeMethod = false;
        private SilenceProvider? silenceProvider;
        
        // Opcje nagrywania
        private bool mixAudioSources = true;
        private WaveInEvent? microphoneWaveIn;
        private WasapiLoopbackCapture? systemAudioCapture;
        private WaveFileWriter? mixedWriter;
        private BufferedWaveProvider? microphoneBuffer;
        private BufferedWaveProvider? systemAudioBuffer;
        private List<byte[]> microphoneChunks = new List<byte[]>();
        private List<byte[]> systemAudioChunks = new List<byte[]>();
        private int selectedMicrophoneIndex = -1;
        private int selectedDeviceIndex = 0;
        private int sampleRate = 44100; // Domyślna częstotliwość próbkowania
        
        // Przechowuje wszystkie urządzenia audio
        private List<string> audioDevices = new List<string>();

        public RecorderForm(Form1 parent)
        {
            InitializeComponent();
            parentForm = parent;
            LoadAudioDevices();
            
            // Dodaj przycisk Opcje
            Button btnOptions = new Button();
            btnOptions.Text = "Opcje";
            btnOptions.Size = new Size(100, 35);
            btnOptions.Location = new Point(btnStartRecording.Right + 10, btnStartRecording.Top);
            btnOptions.Click += btnOptions_Click;
            this.Controls.Add(btnOptions);
        }
        
        // Metoda wczytująca urządzenia audio do pamięci
        private void LoadAudioDevices()
        {
            audioDevices.Clear();
            
            try
            {
                // Dodaj opcję nagrywania dźwięku systemowego
                audioDevices.Add("Dźwięk systemowy");
                
                // Dodaj urządzenia wejściowe (mikrofony)
                for (int i = 0; i < WaveInEvent.DeviceCount; i++)
                {
                    var capabilities = WaveInEvent.GetCapabilities(i);
                    audioDevices.Add($"Mikrofon: {capabilities.ProductName}");
                }
                
                selectedDeviceIndex = 0; // Domyślnie wybierz dźwięk systemowy
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania urządzeń audio: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblRecordingStatus.Text = $"Błąd: {ex.Message}";
            }
        }

        // Nowa metoda obsługi przycisku Opcje
        private void btnOptions_Click(object? sender, EventArgs e)
        {
            try
            {
                // Otwórz formularz opcji z bieżącymi ustawieniami
                using (var optionsForm = new RecordingOptionsForm(
                    this, 
                    useAlternativeMethod, 
                    mixAudioSources, 
                    selectedMicrophoneIndex, 
                    sampleRate))
                {
                    // Jeśli użytkownik zatwierdził zmiany, zostaną one zapisane w metodzie UpdateOptions
                    optionsForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas otwierania okna opcji: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Nowa metoda do aktualizacji opcji z formularza opcji
        public void UpdateOptions(bool alternativeMethod, bool mixAudio, int microphoneIndex, int samplingRate, int deviceIndex)
        {
            useAlternativeMethod = alternativeMethod;
            mixAudioSources = mixAudio;
            selectedMicrophoneIndex = microphoneIndex;
            sampleRate = samplingRate;
            selectedDeviceIndex = deviceIndex;
            
            // Aktualizuj status
            lblRecordingStatus.Text = $"Opcje zaktualizowane.";
        }

        private void btnBrowse_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Pliki WAV (*.wav)|*.wav";
                saveFileDialog.Title = "Wybierz lokalizację do zapisania nagrania";
                saveFileDialog.DefaultExt = "wav";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtOutputPath.Text = saveFileDialog.FileName;
                    btnStartRecording.Enabled = true;
                }
            }
        }

        private void btnStartRecording_Click(object? sender, EventArgs e)
        {
            if (isRecording)
            {
                StopRecording();
            }
            else
            {
                StartRecording();
            }
        }

        private void StartRecording()
        {
            if (string.IsNullOrEmpty(txtOutputPath.Text))
            {
                MessageBox.Show("Proszę wybrać lokalizację do zapisania nagrania.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                outputFilePath = txtOutputPath.Text;
                
                if (mixAudioSources)
                {
                    StartMixedRecording();
                }
                else
                {
                    // Sprawdź, czy wybrano dźwięk systemowy (loopback)
                    isLoopbackMode = selectedDeviceIndex == 0;
                    
                    if (isLoopbackMode)
                    {
                        if (useAlternativeMethod)
                        {
                            StartAlternativeLoopbackRecording();
                        }
                        else
                        {
                            // Standardowa metoda nagrywania dźwięku systemowego
                            lblRecordingStatus.Text = "Inicjalizacja nagrywania dźwięku systemowego...";
                            Application.DoEvents(); // Odświeżenie UI
                            
                            try
                            {
                                loopbackCapture = new WasapiLoopbackCapture();
                                
                                // Sprawdź, czy format audio jest poprawny
                                Debug.WriteLine($"Format audio: {loopbackCapture.WaveFormat}");
                                lblRecordingStatus.Text = $"Format audio: {loopbackCapture.WaveFormat}";
                                Application.DoEvents(); // Odświeżenie UI
                                
                                loopbackCapture.DataAvailable += LoopbackCapture_DataAvailable;
                                loopbackCapture.RecordingStopped += LoopbackCapture_RecordingStopped;
                                
                                // Dodaj obsługę wyjątków
                                writer = new WaveFileWriter(outputFilePath, loopbackCapture.WaveFormat);
                                
                                lblRecordingStatus.Text = "Rozpoczynam nagrywanie dźwięku systemowego...";
                                Application.DoEvents(); // Odświeżenie UI
                                
                                loopbackCapture.StartRecording();
                                Debug.WriteLine("Nagrywanie dźwięku systemowego rozpoczęte");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Błąd podczas inicjalizacji nagrywania dźwięku systemowego: {ex.Message}\n\nSpróbuj użyć alternatywnej metody nagrywania.", 
                                    "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                                CleanupRecording();
                                return;
                            }
                        }
                    }
                    else
                    {
                        // Nagrywanie z mikrofonu
                        lblRecordingStatus.Text = "Inicjalizacja nagrywania z mikrofonu...";
                        Application.DoEvents(); // Odświeżenie UI
                        
                        try
                        {
                            waveIn = new WaveInEvent();
                            waveIn.DeviceNumber = selectedDeviceIndex - 1; // Odejmujemy 1, bo pierwszy element to loopback
                            waveIn.WaveFormat = new WaveFormat(44100, 1); // 44.1kHz, mono
                            waveIn.DataAvailable += WaveIn_DataAvailable;
                            waveIn.RecordingStopped += WaveIn_RecordingStopped;

                            writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
                            
                            lblRecordingStatus.Text = "Rozpoczynam nagrywanie z mikrofonu...";
                            Application.DoEvents(); // Odświeżenie UI
                            
                            waveIn.StartRecording();
                            Debug.WriteLine("Nagrywanie z mikrofonu rozpoczęte");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Błąd podczas inicjalizacji nagrywania z mikrofonu: {ex.Message}", 
                                "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                            CleanupRecording();
                            return;
                        }
                    }
                }
                
                isRecording = true;

                btnStartRecording.Text = "Zatrzymaj nagrywanie";
                lblRecordingStatus.Text = mixAudioSources ? 
                    "Nagrywanie mikrofonu i dźwięku systemowego..." : 
                    (isLoopbackMode ? "Nagrywanie dźwięku systemowego..." : "Nagrywanie z mikrofonu...");
                btnBrowse.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas rozpoczynania nagrywania: {ex.Message}\n\n{ex.StackTrace}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                CleanupRecording();
            }
        }

        private void StartAlternativeLoopbackRecording()
        {
            lblRecordingStatus.Text = "Inicjalizacja alternatywnej metody nagrywania dźwięku systemowego...";
            Application.DoEvents(); // Odświeżenie UI
            
            try
            {
                // Tworzymy obiekt WasapiOut i odtwarzamy ciszę, aby uaktywnić bufor dźwięku
                wasapiOut = new WasapiOut();
                silenceProvider = new SilenceProvider(new WaveFormat(44100, 1));
                wasapiOut.Init(silenceProvider);
                wasapiOut.Play();
                
                // Teraz tworzymy loopbackCapture
                loopbackCapture = new WasapiLoopbackCapture();
                loopbackCapture.DataAvailable += LoopbackCapture_DataAvailable;
                loopbackCapture.RecordingStopped += LoopbackCapture_RecordingStopped;
                
                writer = new WaveFileWriter(outputFilePath, loopbackCapture.WaveFormat);
                
                lblRecordingStatus.Text = "Rozpoczynam nagrywanie dźwięku systemowego (metoda alternatywna)...";
                Application.DoEvents(); // Odświeżenie UI
                
                loopbackCapture.StartRecording();
                Debug.WriteLine("Alternatywne nagrywanie dźwięku systemowego rozpoczęte");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas inicjalizacji alternatywnej metody nagrywania dźwięku systemowego: {ex.Message}", 
                    "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                CleanupRecording();
            }
        }

        private void StartMixedRecording()
        {
            try
            {
                lblRecordingStatus.Text = "Inicjalizacja nagrywania z dwóch źródeł...";
                Application.DoEvents(); // Odświeżenie UI
                
                // Tworzymy dwa osobne pliki tymczasowe - jeden dla mikrofonu, drugi dla dźwięku systemowego
                string tempMicFile = Path.Combine(Path.GetDirectoryName(outputFilePath) ?? "", 
                    Path.GetFileNameWithoutExtension(outputFilePath) + "_mic_temp.wav");
                string tempSystemFile = Path.Combine(Path.GetDirectoryName(outputFilePath) ?? "", 
                    Path.GetFileNameWithoutExtension(outputFilePath) + "_system_temp.wav");
                
                Debug.WriteLine($"Plik tymczasowy mikrofonu: {tempMicFile}");
                Debug.WriteLine($"Plik tymczasowy dźwięku systemowego: {tempSystemFile}");
                
                // Inicjalizacja przechwytywania dźwięku systemowego
                try
                {
                    systemAudioCapture = new WasapiLoopbackCapture();
                    Debug.WriteLine($"Format dźwięku systemowego: {systemAudioCapture.WaveFormat}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas inicjalizacji przechwytywania dźwięku systemowego: {ex.Message}\n\n" +
                        "Spróbuj uruchomić aplikację jako administrator lub sprawdź ustawienia dźwięku w systemie.", 
                        "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                    return;
                }
                
                // Inicjalizacja przechwytywania mikrofonu
                try
                {
                    microphoneWaveIn = new WaveInEvent();
                    microphoneWaveIn.DeviceNumber = selectedMicrophoneIndex;
                    microphoneWaveIn.WaveFormat = new WaveFormat(sampleRate, 1); // Używamy wybranej częstotliwości próbkowania
                    Debug.WriteLine($"Format dźwięku z mikrofonu: {microphoneWaveIn.WaveFormat}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas inicjalizacji przechwytywania mikrofonu: {ex.Message}\n\n" +
                        "Sprawdź, czy mikrofon jest podłączony i działa poprawnie.", 
                        "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                    
                    if (systemAudioCapture != null)
                    {
                        systemAudioCapture.Dispose();
                        systemAudioCapture = null;
                    }
                    
                    return;
                }
                
                // Tworzymy dwa osobne writery dla każdego źródła
                WaveFileWriter? micWriter = null;
                WaveFileWriter? systemWriter = null;
                
                try
                {
                    // Zapisujemy każde źródło w jego natywnym formacie
                    micWriter = new WaveFileWriter(tempMicFile, microphoneWaveIn.WaveFormat);
                    systemWriter = new WaveFileWriter(tempSystemFile, systemAudioCapture.WaveFormat);
                    
                    Debug.WriteLine($"Utworzono writer dla mikrofonu: {microphoneWaveIn.WaveFormat}");
                    Debug.WriteLine($"Utworzono writer dla dźwięku systemowego: {systemAudioCapture.WaveFormat}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas tworzenia plików tymczasowych: {ex.Message}", 
                        "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                    
                    if (micWriter != null) micWriter.Dispose();
                    if (systemWriter != null) systemWriter.Dispose();
                    
                    if (microphoneWaveIn != null)
                    {
                        microphoneWaveIn.Dispose();
                        microphoneWaveIn = null;
                    }
                    
                    if (systemAudioCapture != null)
                    {
                        systemAudioCapture.Dispose();
                        systemAudioCapture = null;
                    }
                    
                    return;
                }
                
                // Zapisujemy ścieżki do plików tymczasowych
                microphoneChunks.Clear();
                microphoneChunks.Add(Encoding.UTF8.GetBytes(tempMicFile));
                
                systemAudioChunks.Clear();
                systemAudioChunks.Add(Encoding.UTF8.GetBytes(tempSystemFile));
                
                // Zmienne finalne dla użycia w zdarzeniach
                WaveFileWriter finalMicWriter = micWriter;
                WaveFileWriter finalSystemWriter = systemWriter;
                
                // Dodajemy obsługę zdarzeń dla obu źródeł
                microphoneWaveIn.DataAvailable += (s, e) => 
                {
                    try
                    {
                        if (finalMicWriter != null && e.BytesRecorded > 0)
                        {
                            finalMicWriter.Write(e.Buffer, 0, e.BytesRecorded);
                            finalMicWriter.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Błąd w obsłudze danych mikrofonu: {ex.Message}");
                    }
                };
                
                systemAudioCapture.DataAvailable += (s, e) => 
                {
                    try
                    {
                        if (finalSystemWriter != null && e.BytesRecorded > 0)
                        {
                            finalSystemWriter.Write(e.Buffer, 0, e.BytesRecorded);
                            finalSystemWriter.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Błąd w obsłudze danych systemowych: {ex.Message}");
                    }
                };
                
                // Obsługa zatrzymania nagrywania
                bool microphoneStopped = false;
                bool systemAudioStopped = false;
                
                microphoneWaveIn.RecordingStopped += (s, e) => 
                {
                    try
                    {
                        Debug.WriteLine("Zatrzymano nagrywanie z mikrofonu");
                        finalMicWriter.Dispose();
                        microphoneStopped = true;
                        
                        // Sprawdź, czy oba źródła zostały zatrzymane
                        if (systemAudioStopped)
                        {
                            // Sprawdź, czy formularz jest jeszcze aktywny
                            if (this.IsHandleCreated && !this.IsDisposed)
                            {
                                this.Invoke(new Action(() => 
                                {
                                    if (!this.IsDisposed)
                                    {
                                        FinalizeMixedRecording(tempMicFile, tempSystemFile);
                                    }
                                }));
                            }
                            else
                            {
                                Debug.WriteLine("Formularz nie jest aktywny, pomijam finalizację nagrywania");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Błąd przy zatrzymaniu mikrofonu: {ex.Message}");
                    }
                };
                
                systemAudioCapture.RecordingStopped += (s, e) => 
                {
                    try
                    {
                        Debug.WriteLine("Zatrzymano nagrywanie dźwięku systemowego");
                        finalSystemWriter.Dispose();
                        systemAudioStopped = true;
                        
                        // Sprawdź, czy oba źródła zostały zatrzymane
                        if (microphoneStopped)
                        {
                            // Sprawdź, czy formularz jest jeszcze aktywny
                            if (this.IsHandleCreated && !this.IsDisposed)
                            {
                                this.Invoke(new Action(() => 
                                {
                                    if (!this.IsDisposed)
                                    {
                                        FinalizeMixedRecording(tempMicFile, tempSystemFile);
                                    }
                                }));
                            }
                            else
                            {
                                Debug.WriteLine("Formularz nie jest aktywny, pomijam finalizację nagrywania");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Błąd przy zatrzymaniu dźwięku systemowego: {ex.Message}");
                    }
                };
                
                // Rozpocznij nagrywanie z obu źródeł
                try
                {
                    systemAudioCapture.StartRecording();
                    microphoneWaveIn.StartRecording();
                    
                    Debug.WriteLine("Rozpoczęto nagrywanie z dwóch źródeł");
                    
                    lblRecordingStatus.Text = "Nagrywanie z mikrofonu i dźwięku systemowego...";
                    Application.DoEvents(); // Odświeżenie UI
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas rozpoczynania nagrywania: {ex.Message}", 
                        "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                    
                    // Zatrzymaj nagrywanie, jeśli zostało rozpoczęte
                    try
                    {
                        if (microphoneWaveIn != null)
                        {
                            microphoneWaveIn.StopRecording();
                        }
                        
                        if (systemAudioCapture != null && systemAudioCapture.CaptureState == CaptureState.Capturing)
                        {
                            systemAudioCapture.StopRecording();
                        }
                    }
                    catch
                    {
                        // Ignoruj błędy podczas zatrzymywania
                    }
                    
                    // Wyczyść zasoby
                    finalMicWriter.Dispose();
                    finalSystemWriter.Dispose();
                    
                    if (microphoneWaveIn != null)
                    {
                        microphoneWaveIn.Dispose();
                        microphoneWaveIn = null;
                    }
                    
                    if (systemAudioCapture != null)
                    {
                        systemAudioCapture.Dispose();
                        systemAudioCapture = null;
                    }
                    
                    // Usuń pliki tymczasowe
                    try
                    {
                        if (File.Exists(tempMicFile)) File.Delete(tempMicFile);
                        if (File.Exists(tempSystemFile)) File.Delete(tempSystemFile);
                    }
                    catch
                    {
                        // Ignoruj błędy podczas usuwania plików
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas inicjalizacji nagrywania z dwóch źródeł: {ex.Message}\n\n{ex.StackTrace}", 
                    "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                CleanupRecording();
            }
        }

        private void FinalizeMixedRecording(string tempMicFile, string tempSystemFile)
        {
            try
            {
                MixWavFiles(tempMicFile, tempSystemFile, outputFilePath);
                
                // Usuwamy pliki tymczasowe
                try
                {
                    if (File.Exists(tempMicFile)) File.Delete(tempMicFile);
                    if (File.Exists(tempSystemFile)) File.Delete(tempSystemFile);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Błąd przy usuwaniu plików tymczasowych: {ex.Message}");
                }
                
                // Sprawdzamy czy plik wynikowy istnieje i ma odpowiedni rozmiar
                if (File.Exists(outputFilePath) && new FileInfo(outputFilePath).Length > 0)
                {
                    lblRecordingStatus.Text = "Nagrywanie zakończone.";
                    
                    // Przekaż ścieżkę do pliku do głównego formularza
                    if (parentForm != null && !parentForm.IsDisposed)
                    {
                        try
                        {
                            parentForm.Invoke(new Action(() => 
                            {
                                if (!parentForm.IsDisposed)
                                {
                                    parentForm.SetSelectedFile(outputFilePath);
                                }
                            }));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Błąd przy przekazywaniu ścieżki do głównego formularza: {ex.Message}");
                        }
                    }
                    
                    // Sprawdź, czy formularz jest jeszcze aktywny
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        DialogResult result = MessageBox.Show(
                            "Nagrywanie zakończone. Czy chcesz zamknąć okno nagrywania?",
                            "Nagrywanie zakończone",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                            
                        if (result == DialogResult.Yes)
                        {
                            this.Close();
                        }
                    }
                }
                else
                {
                    lblRecordingStatus.Text = "Nagrywanie anulowane lub plik jest pusty.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas finalizowania nagrywania: {ex.Message}");
                
                // Sprawdź, czy formularz jest jeszcze aktywny
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    MessageBox.Show($"Błąd podczas finalizowania nagrywania: {ex.Message}", 
                        "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                }
            }
            finally
            {
                CleanupRecording();
            }
        }

        private void MixWavFiles(string micFile, string systemFile, string outputFile)
        {
            lblRecordingStatus.Text = "Miksowanie dźwięku...";
            Application.DoEvents();
            
            Debug.WriteLine($"Miksowanie plików: {micFile} i {systemFile} do {outputFile}");
            
            // Sprawdź czy pliki istnieją
            if (!File.Exists(micFile) || !File.Exists(systemFile))
            {
                Debug.WriteLine("Jeden z plików tymczasowych nie istnieje!");
                
                // Jeśli jeden z plików nie istnieje, ale drugi tak, to po prostu skopiuj istniejący
                if (File.Exists(micFile) && !File.Exists(systemFile))
                {
                    Debug.WriteLine("Kopiowanie tylko pliku mikrofonu");
                    File.Copy(micFile, outputFile, true);
                    return;
                }
                else if (!File.Exists(micFile) && File.Exists(systemFile))
                {
                    Debug.WriteLine("Kopiowanie tylko pliku dźwięku systemowego");
                    File.Copy(systemFile, outputFile, true);
                    return;
                }
                else
                {
                    throw new FileNotFoundException("Nie znaleziono plików tymczasowych do miksowania");
                }
            }
            
            // Sprawdź rozmiary plików
            long micSize = new FileInfo(micFile).Length;
            long systemSize = new FileInfo(systemFile).Length;
            
            Debug.WriteLine($"Rozmiar pliku mikrofonu: {micSize} bajtów");
            Debug.WriteLine($"Rozmiar pliku dźwięku systemowego: {systemSize} bajtów");
            
            if (micSize < 100 && systemSize < 100)
            {
                throw new InvalidOperationException("Oba pliki tymczasowe są zbyt małe, prawdopodobnie nie zawierają danych audio");
            }
            
            try
            {
                // Otwórz pliki wejściowe
                using (var micReader = new AudioFileReader(micFile))
                using (var systemReader = new AudioFileReader(systemFile))
                {
                    // Wyświetl informacje o formatach
                    Debug.WriteLine($"Format mikrofonu: {micReader.WaveFormat}");
                    Debug.WriteLine($"Format dźwięku systemowego: {systemReader.WaveFormat}");
                    
                    // Ustalamy wspólny format - stereo, z wybraną częstotliwością próbkowania
                    WaveFormat commonFormat = new WaveFormat(sampleRate, 2);
                    
                    // Konwertuj oba źródła do wspólnego formatu
                    var micSampleProvider = micReader.ToSampleProvider();
                    var systemSampleProvider = systemReader.ToSampleProvider();
                    
                    // Normalizuj głośność
                    var micVolume = new VolumeSampleProvider(micSampleProvider);
                    micVolume.Volume = 0.8f; // 80% głośności dla mikrofonu
                    
                    var systemVolume = new VolumeSampleProvider(systemSampleProvider);
                    systemVolume.Volume = 0.8f; // 80% głośności dla dźwięku systemowego
                    
                    // Konwertuj do wspólnego formatu
                    ISampleProvider micConverted;
                    ISampleProvider systemConverted;
                    
                    // Konwersja mikrofonu (mono -> stereo jeśli potrzeba)
                    if (micVolume.WaveFormat.Channels == 1)
                    {
                        micConverted = new MonoToStereoSampleProvider(micVolume);
                    }
                    else
                    {
                        micConverted = micVolume;
                    }
                    
                    // Konwersja dźwięku systemowego (dostosowanie częstotliwości próbkowania jeśli potrzeba)
                    if (systemVolume.WaveFormat.SampleRate != commonFormat.SampleRate)
                    {
                        systemConverted = new WdlResamplingSampleProvider(systemVolume, commonFormat.SampleRate);
                    }
                    else
                    {
                        systemConverted = systemVolume;
                    }
                    
                    // Upewnij się, że oba źródła mają ten sam format
                    Debug.WriteLine($"Format mikrofonu po konwersji: {micConverted.WaveFormat}");
                    Debug.WriteLine($"Format dźwięku systemowego po konwersji: {systemConverted.WaveFormat}");
                    
                    // Miksuj oba źródła
                    var mixer = new MixingSampleProvider(new[] { micConverted, systemConverted });
                    
                    // Zapisz do pliku wyjściowego
                    WaveFileWriter.CreateWaveFile16(outputFile, mixer);
                    
                    Debug.WriteLine("Miksowanie zakończone");
                    
                    // Sprawdź rozmiar pliku wyjściowego
                    long outputSize = new FileInfo(outputFile).Length;
                    Debug.WriteLine($"Rozmiar pliku wyjściowego: {outputSize} bajtów");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas miksowania: {ex.Message}\n{ex.StackTrace}");
                
                // Jeśli miksowanie się nie powiodło, spróbuj użyć tylko jednego pliku
                try
                {
                    if (micSize > systemSize && micSize > 1000)
                    {
                        Debug.WriteLine("Miksowanie nie powiodło się, używam tylko pliku mikrofonu");
                        File.Copy(micFile, outputFile, true);
                    }
                    else if (systemSize > 1000)
                    {
                        Debug.WriteLine("Miksowanie nie powiodło się, używam tylko pliku dźwięku systemowego");
                        File.Copy(systemFile, outputFile, true);
                    }
                    else
                    {
                        throw new InvalidOperationException("Nie udało się zmiksować plików i żaden z nich nie zawiera wystarczającej ilości danych");
                    }
                }
                catch (Exception innerEx)
                {
                    Debug.WriteLine($"Błąd podczas kopiowania pliku zastępczego: {innerEx.Message}");
                    throw new InvalidOperationException($"Nie udało się zmiksować ani skopiować plików: {ex.Message}", ex);
                }
            }
            
            lblRecordingStatus.Text = "Miksowanie zakończone.";
            Application.DoEvents();
        }

        private void LoopbackCapture_DataAvailable(object? sender, WaveInEventArgs e)
        {
            try
            {
                if (writer != null && e.BytesRecorded > 0)
                {
                    writer.Write(e.Buffer, 0, e.BytesRecorded);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd w LoopbackCapture_DataAvailable: {ex.Message}");
                // Nie wyświetlamy MessageBox, bo ta metoda jest wywoływana często
            }
        }
        
        private void LoopbackCapture_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            try
            {
                Debug.WriteLine("LoopbackCapture_RecordingStopped wywołane");
                
                if (writer != null)
                {
                    writer.Dispose();
                    writer = null;
                    Debug.WriteLine("Writer zamknięty w LoopbackCapture_RecordingStopped");
                }
                
                if (wasapiOut != null)
                {
                    wasapiOut.Dispose();
                    wasapiOut = null;
                    Debug.WriteLine("WasapiOut zamknięty w LoopbackCapture_RecordingStopped");
                }
                
                if (silenceProvider != null)
                {
                    silenceProvider = null;
                    Debug.WriteLine("SilenceProvider zamknięty w LoopbackCapture_RecordingStopped");
                }
                
                // Sprawdź, czy plik wyjściowy istnieje i ma odpowiedni rozmiar
                if (File.Exists(outputFilePath) && new FileInfo(outputFilePath).Length > 0)
                {
                    // Aktualizacja UI musi być wykonana w wątku UI
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        try
                        {
                            this.Invoke(new Action(() =>
                            {
                                if (!this.IsDisposed)
                                {
                                    lblRecordingStatus.Text = "Nagrywanie zakończone.";
                                    
                                    // Przekaż ścieżkę do pliku do głównego formularza
                                    if (parentForm != null && !parentForm.IsDisposed)
                                    {
                                        try
                                        {
                                            parentForm.Invoke(new Action(() => 
                                            {
                                                if (!parentForm.IsDisposed)
                                                {
                                                    parentForm.SetSelectedFile(outputFilePath);
                                                }
                                            }));
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine($"Błąd przy przekazywaniu ścieżki do głównego formularza: {ex.Message}");
                                        }
                                    }
                                    
                                    DialogResult result = MessageBox.Show(
                                        "Nagrywanie zakończone. Czy chcesz zamknąć okno nagrywania?",
                                        "Nagrywanie zakończone",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question);
                                        
                                    if (result == DialogResult.Yes)
                                    {
                                        this.Close();
                                    }
                                }
                            }));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Błąd podczas aktualizacji UI po zatrzymaniu nagrywania: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Plik wyjściowy nie istnieje lub jest pusty!");
                    
                    // Aktualizacja UI musi być wykonana w wątku UI
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        try
                        {
                            this.Invoke(new Action(() =>
                            {
                                if (!this.IsDisposed)
                                {
                                    lblRecordingStatus.Text = "Nagrywanie anulowane lub plik jest pusty.";
                                }
                            }));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Błąd podczas aktualizacji UI po zatrzymaniu nagrywania: {ex.Message}");
                        }
                    }
                }
                
                isRecording = false;
                
                // Aktualizacja UI musi być wykonana w wątku UI
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (!this.IsDisposed)
                            {
                                btnStartRecording.Text = "Rozpocznij nagrywanie";
                                btnBrowse.Enabled = true;
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Błąd podczas aktualizacji UI po zatrzymaniu nagrywania: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd w LoopbackCapture_RecordingStopped: {ex.Message}\n{ex.StackTrace}");
                
                // Aktualizacja UI musi być wykonana w wątku UI
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (!this.IsDisposed)
                            {
                                MessageBox.Show($"Błąd podczas zatrzymywania nagrywania: {ex.Message}", 
                                    "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                            }
                        }));
                    }
                    catch (Exception invokeEx)
                    {
                        Debug.WriteLine($"Błąd podczas wywoływania MessageBox: {invokeEx.Message}");
                    }
                }
            }
        }

        private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            try
            {
                if (writer != null && e.BytesRecorded > 0)
                {
                    writer.Write(e.Buffer, 0, e.BytesRecorded);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd w WaveIn_DataAvailable: {ex.Message}");
                // Nie wyświetlamy MessageBox, bo ta metoda jest wywoływana często
            }
        }

        private void WaveIn_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            try
            {
                Debug.WriteLine("WaveIn_RecordingStopped wywołane");
                
                if (e.Exception != null)
                {
                    Debug.WriteLine($"Wyjątek w WaveIn_RecordingStopped: {e.Exception.Message}");
                    this.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show($"Błąd podczas nagrywania z mikrofonu: {e.Exception.Message}", 
                            "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblRecordingStatus.Text = $"Błąd: {e.Exception.Message}";
                    }));
                }
                
                CleanupRecording();
                
                this.BeginInvoke(new Action(() =>
                {
                    if (File.Exists(outputFilePath) && new FileInfo(outputFilePath).Length > 0)
                    {
                        lblRecordingStatus.Text = "Nagrywanie zakończone.";
                        
                        // Przekaż ścieżkę do pliku do głównego formularza
                        parentForm.SetSelectedFile(outputFilePath);
                        
                        DialogResult result = MessageBox.Show(
                            "Nagrywanie zakończone. Czy chcesz zamknąć okno nagrywania?",
                            "Nagrywanie zakończone",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                            
                        if (result == DialogResult.Yes)
                        {
                            this.Close();
                        }
                    }
                    else
                    {
                        lblRecordingStatus.Text = "Nagrywanie anulowane lub plik jest pusty.";
                    }
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd w WaveIn_RecordingStopped: {ex.Message}");
                this.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show($"Błąd podczas zatrzymywania nagrywania: {ex.Message}", 
                        "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                }));
            }
        }

        private void CleanupRecording()
        {
            try
            {
                Debug.WriteLine("CleanupRecording wywołane");
                
                if (writer != null)
                {
                    writer.Dispose();
                    writer = null;
                    Debug.WriteLine("Writer zamknięty");
                }
                
                if (mixedWriter != null)
                {
                    mixedWriter.Dispose();
                    mixedWriter = null;
                    Debug.WriteLine("MixedWriter zamknięty");
                }
                
                if (waveIn != null)
                {
                    waveIn.Dispose();
                    waveIn = null;
                    Debug.WriteLine("WaveIn zamknięty");
                }
                
                if (loopbackCapture != null)
                {
                    loopbackCapture.Dispose();
                    loopbackCapture = null;
                    Debug.WriteLine("LoopbackCapture zamknięty");
                }
                
                if (wasapiOut != null)
                {
                    wasapiOut.Dispose();
                    wasapiOut = null;
                    Debug.WriteLine("WasapiOut zamknięty");
                }
                
                if (microphoneWaveIn != null)
                {
                    microphoneWaveIn.Dispose();
                    microphoneWaveIn = null;
                    Debug.WriteLine("MicrophoneWaveIn zamknięty");
                }
                
                if (systemAudioCapture != null)
                {
                    systemAudioCapture.Dispose();
                    systemAudioCapture = null;
                    Debug.WriteLine("SystemAudioCapture zamknięty");
                }
                
                if (silenceProvider != null)
                {
                    silenceProvider = null;
                    Debug.WriteLine("SilenceProvider zamknięty");
                }
                
                if (microphoneBuffer != null)
                {
                    microphoneBuffer = null;
                    Debug.WriteLine("MicrophoneBuffer zamknięty");
                }
                
                if (systemAudioBuffer != null)
                {
                    systemAudioBuffer = null;
                    Debug.WriteLine("SystemAudioBuffer zamknięty");
                }

                isRecording = false;
                
                // Sprawdź, czy formularz jest jeszcze aktywny i czy uchwyt okna istnieje
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    // Użyj Invoke zamiast BeginInvoke, aby mieć pewność, że operacja zostanie wykonana
                    this.Invoke(new Action(() =>
                    {
                        if (!this.IsDisposed)
                        {
                            btnStartRecording.Text = "Rozpocznij nagrywanie";
                            btnBrowse.Enabled = true;
                        }
                    }));
                }
                else
                {
                    Debug.WriteLine("Formularz nie jest aktywny, pomijam aktualizację UI");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd w CleanupRecording: {ex.Message}\n{ex.StackTrace}");
                
                // Sprawdź, czy formularz jest jeszcze aktywny i czy uchwyt okna istnieje
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (!this.IsDisposed)
                            {
                                MessageBox.Show($"Błąd podczas czyszczenia zasobów nagrywania: {ex.Message}", 
                                    "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }));
                    }
                    catch (Exception invokeEx)
                    {
                        Debug.WriteLine($"Błąd podczas wywoływania MessageBox: {invokeEx.Message}");
                    }
                }
            }
        }

        private void RecorderForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (isRecording)
            {
                DialogResult result = MessageBox.Show(
                    "Nagrywanie jest w toku. Czy na pewno chcesz zamknąć okno nagrywania?",
                    "Nagrywanie w toku",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                    
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                
                StopRecording();
            }
        }

        private void StopRecording()
        {
            try
            {
                lblRecordingStatus.Text = "Zatrzymywanie nagrywania...";
                Application.DoEvents(); // Odświeżenie UI
                
                Debug.WriteLine("StopRecording wywołane");
                
                if (mixAudioSources)
                {
                    // Zatrzymaj nagrywanie z obu źródeł
                    if (microphoneWaveIn != null)
                    {
                        try
                        {
                            microphoneWaveIn.StopRecording();
                            Debug.WriteLine("Zatrzymano nagrywanie z mikrofonu");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Błąd przy zatrzymywaniu mikrofonu: {ex.Message}");
                        }
                    }
                    
                    if (systemAudioCapture != null && systemAudioCapture.CaptureState == CaptureState.Capturing)
                    {
                        try
                        {
                            systemAudioCapture.StopRecording();
                            Debug.WriteLine("Zatrzymano nagrywanie dźwięku systemowego");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Błąd przy zatrzymywaniu dźwięku systemowego: {ex.Message}");
                        }
                    }
                }
                else
                {
                    if (isLoopbackMode)
                    {
                        if (loopbackCapture != null && loopbackCapture.CaptureState == CaptureState.Capturing)
                        {
                            loopbackCapture.StopRecording();
                            Debug.WriteLine("Zatrzymano nagrywanie dźwięku systemowego");
                        }
                        
                        if (wasapiOut != null && wasapiOut.PlaybackState == PlaybackState.Playing)
                        {
                            wasapiOut.Stop();
                            Debug.WriteLine("Zatrzymano odtwarzanie ciszy");
                        }
                    }
                    else
                    {
                        if (waveIn != null)
                        {
                            try
                            {
                                waveIn.StopRecording();
                                Debug.WriteLine("Zatrzymano nagrywanie z mikrofonu");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Błąd przy zatrzymywaniu mikrofonu: {ex.Message}");
                            }
                        }
                    }
                }
                
                lblRecordingStatus.Text = "Finalizowanie nagrania...";
                Application.DoEvents(); // Odświeżenie UI
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zatrzymywania nagrywania: {ex.Message}", 
                    "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblRecordingStatus.Text = $"Błąd: {ex.Message}";
                CleanupRecording();
            }
        }
    }
} 