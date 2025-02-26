using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace TranscriberApp
{
    /// <summary>
    /// Klasa odpowiedzialna za nagrywanie dźwięku z mikrofonu.
    /// </summary>
    public class AudioRecorder : IDisposable
    {
        private WaveInEvent? _waveIn;
        private WaveFileWriter? _writer;
        private string _outputFilePath = string.Empty;
        private bool _isRecording;
        private CancellationTokenSource? _cancellationTokenSource;

        /// <summary>
        /// Zdarzenie wywoływane, gdy poziom dźwięku się zmienia.
        /// </summary>
        public event EventHandler<float>? LevelChanged;

        /// <summary>
        /// Zdarzenie wywoływane, gdy czas nagrywania się zmienia.
        /// </summary>
        public event EventHandler<TimeSpan>? RecordingTimeChanged;

        /// <summary>
        /// Pobiera informację, czy nagrywanie jest aktywne.
        /// </summary>
        public bool IsRecording => _isRecording;

        /// <summary>
        /// Pobiera ścieżkę do pliku wyjściowego.
        /// </summary>
        public string OutputFilePath => _outputFilePath;

        /// <summary>
        /// Inicjalizuje nową instancję klasy AudioRecorder.
        /// </summary>
        public AudioRecorder()
        {
            _isRecording = false;
        }

        /// <summary>
        /// Rozpoczyna nagrywanie dźwięku z mikrofonu.
        /// </summary>
        /// <param name="outputFilePath">Ścieżka do pliku wyjściowego.</param>
        /// <param name="deviceNumber">Numer urządzenia nagrywającego (domyślnie 0 - domyślne urządzenie).</param>
        /// <returns>True, jeśli nagrywanie zostało rozpoczęte pomyślnie.</returns>
        public bool StartRecording(string outputFilePath, int deviceNumber = 0)
        {
            if (_isRecording)
                return false;

            try
            {
                _outputFilePath = outputFilePath;
                _cancellationTokenSource = new CancellationTokenSource();

                // Upewnij się, że katalog docelowy istnieje
                string? directory = Path.GetDirectoryName(_outputFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Inicjalizacja urządzenia nagrywającego
                _waveIn = new WaveInEvent
                {
                    DeviceNumber = deviceNumber,
                    WaveFormat = new WaveFormat(44100, 16, 2), // 44.1kHz, 16-bit, stereo
                    BufferMilliseconds = 50
                };

                _waveIn.DataAvailable += OnDataAvailable;
                _waveIn.RecordingStopped += OnRecordingStopped;

                // Inicjalizacja zapisywania do pliku
                _writer = new WaveFileWriter(_outputFilePath, _waveIn.WaveFormat);

                // Rozpoczęcie nagrywania
                _waveIn.StartRecording();
                _isRecording = true;

                // Uruchomienie zadania do śledzenia czasu nagrywania
                Task.Run(() => TrackRecordingTime(_cancellationTokenSource.Token));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas rozpoczynania nagrywania: {ex.Message}");
                CleanupRecording();
                return false;
            }
        }

        /// <summary>
        /// Zatrzymuje nagrywanie dźwięku.
        /// </summary>
        /// <returns>True, jeśli nagrywanie zostało zatrzymane pomyślnie.</returns>
        public bool StopRecording()
        {
            if (!_isRecording)
                return false;

            try
            {
                _cancellationTokenSource?.Cancel();
                _waveIn?.StopRecording();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zatrzymywania nagrywania: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Pobiera listę dostępnych urządzeń nagrywających.
        /// </summary>
        /// <returns>Tablica nazw urządzeń nagrywających.</returns>
        public static string[] GetRecordingDevices()
        {
            int deviceCount = WaveInEvent.DeviceCount;
            string[] devices = new string[deviceCount];

            for (int i = 0; i < deviceCount; i++)
            {
                var capabilities = WaveInEvent.GetCapabilities(i);
                devices[i] = capabilities.ProductName;
            }

            return devices;
        }

        /// <summary>
        /// Obsługuje zdarzenie dostępności danych audio.
        /// </summary>
        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            if (_writer != null)
            {
                // Zapisz dane do pliku
                _writer.Write(e.Buffer, 0, e.BytesRecorded);

                // Oblicz poziom dźwięku (głośność)
                float level = CalculateLevel(e.Buffer, e.BytesRecorded);
                LevelChanged?.Invoke(this, level);
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie zatrzymania nagrywania.
        /// </summary>
        private void OnRecordingStopped(object? sender, StoppedEventArgs e)
        {
            CleanupRecording();

            if (e.Exception != null)
            {
                Console.WriteLine($"Błąd podczas nagrywania: {e.Exception.Message}");
            }
        }

        /// <summary>
        /// Czyści zasoby używane do nagrywania.
        /// </summary>
        private void CleanupRecording()
        {
            _isRecording = false;

            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }

            if (_waveIn != null)
            {
                _waveIn.Dispose();
                _waveIn = null;
            }
        }

        /// <summary>
        /// Oblicza poziom dźwięku (głośność) na podstawie bufora danych.
        /// </summary>
        private float CalculateLevel(byte[] buffer, int bytesRecorded)
        {
            // Konwersja bajtów na próbki 16-bitowe
            int sampleCount = bytesRecorded / 2;
            float sum = 0;

            for (int i = 0; i < bytesRecorded; i += 2)
            {
                short sample = (short)((buffer[i + 1] << 8) | buffer[i]);
                float normalized = sample / 32768f; // Normalizacja do zakresu -1.0 do 1.0
                sum += Math.Abs(normalized);
            }

            // Średnia wartość bezwzględna jako przybliżenie poziomu dźwięku
            return sum / sampleCount;
        }

        /// <summary>
        /// Śledzi czas nagrywania i wywołuje zdarzenie RecordingTimeChanged.
        /// </summary>
        private async Task TrackRecordingTime(CancellationToken cancellationToken)
        {
            DateTime startTime = DateTime.Now;

            while (!cancellationToken.IsCancellationRequested)
            {
                TimeSpan elapsed = DateTime.Now - startTime;
                RecordingTimeChanged?.Invoke(this, elapsed);
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Zwalnia zasoby używane przez AudioRecorder.
        /// </summary>
        public void Dispose()
        {
            StopRecording();
            _cancellationTokenSource?.Dispose();
        }
    }
} 