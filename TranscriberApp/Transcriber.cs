using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

namespace TranscriberApp
{
    public class Transcriber : ITranscriber
    {
        private readonly string whisperPath;
        private readonly string modelPath;
        
        public Transcriber()
        {
            // Ścieżki do Whisper i modelu
            whisperPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "whisper", "whisper.exe");
            modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "whisper", "models", "medium.pt");
            
            // Upewnij się, że katalogi istnieją
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "whisper", "models"));
        }
        
        public async Task<string?> TranscribeAsync(string audioFilePath, Action<int, string?>? progressCallback = null)
        {
            if (!File.Exists(audioFilePath))
            {
                throw new FileNotFoundException("Nie znaleziono pliku audio", audioFilePath);
            }
            
            // Utwórz ścieżkę do pliku wyjściowego
            string outputPath = Path.Combine(
                Path.GetDirectoryName(audioFilePath) ?? string.Empty,
                Path.GetFileNameWithoutExtension(audioFilePath) + "_transkrypcja.txt");
            
            // Przygotuj argumenty dla Whisper
            string arguments = $"--model {modelPath} --language pl --output_dir \"{Path.GetDirectoryName(audioFilePath)}\" \"{audioFilePath}\"";
            
            // Utwórz proces Whisper
            using (Process process = new Process())
            {
                process.StartInfo.FileName = whisperPath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                
                // Obsługa wyjścia standardowego dla śledzenia postępu
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        // Przykładowy format wyjścia: "[00:00.000 --> 00:05.000] Tekst transkrypcji"
                        // Można z tego wyciągnąć informacje o postępie
                        
                        // Prosta implementacja - zakładamy, że każda linia to 1% postępu
                        progressCallback?.Invoke(1, null);
                    }
                };
                
                // Uruchom proces
                process.Start();
                process.BeginOutputReadLine();
                
                // Poczekaj na zakończenie procesu
                await Task.Run(() => process.WaitForExit());
                
                // Sprawdź, czy proces zakończył się pomyślnie
                if (process.ExitCode != 0)
                {
                    string error = process.StandardError.ReadToEnd();
                    throw new Exception($"Błąd podczas transkrypcji: {error}");
                }
                
                // Sprawdź, czy plik wyjściowy został utworzony
                if (File.Exists(outputPath))
                {
                    return outputPath;
                }
                else
                {
                    throw new Exception("Nie udało się utworzyć pliku transkrypcji");
                }
            }
        }
        
        public async Task<string?> TranscribeWithOptionsAsync(string audioFilePath, Dictionary<string, object> options, Action<int, string?>? progressCallback = null)
        {
            if (!File.Exists(audioFilePath))
            {
                throw new FileNotFoundException("Nie znaleziono pliku audio", audioFilePath);
            }
            
            // Pobierz opcje transkrypcji
            string language = options.ContainsKey("language") ? options["language"]?.ToString() ?? "pl" : "pl";
            bool addPunctuation = options.ContainsKey("add_punctuation") && options["add_punctuation"] is bool value && value;
            bool highQuality = options.ContainsKey("high_quality") && options["high_quality"] is bool qualityValue && qualityValue;
            
            // Utwórz ścieżkę do pliku wyjściowego
            string outputPath = Path.Combine(
                Path.GetDirectoryName(audioFilePath) ?? string.Empty,
                Path.GetFileNameWithoutExtension(audioFilePath) + "_transkrypcja.txt");
            
            // Przygotuj argumenty dla Whisper
            string arguments = $"--model {modelPath} --language {language} --output_dir \"{Path.GetDirectoryName(audioFilePath)}\" ";
            
            // Dodaj opcje jakości
            if (highQuality)
            {
                arguments += "--best_of 5 --beam_size 5 ";
            }
            
            // Dodaj opcję interpunkcji
            if (addPunctuation)
            {
                arguments += "--punctuation ";
            }
            
            // Dodaj ścieżkę do pliku audio
            arguments += $"\"{audioFilePath}\"";
            
            // Utwórz proces Whisper
            using (Process process = new Process())
            {
                process.StartInfo.FileName = whisperPath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                
                // Obsługa wyjścia standardowego dla śledzenia postępu
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        // Przykładowy format wyjścia: "[00:00.000 --> 00:05.000] Tekst transkrypcji"
                        // Można z tego wyciągnąć informacje o postępie
                        
                        // Prosta implementacja - zakładamy, że każda linia to 1% postępu
                        progressCallback?.Invoke(1, null);
                    }
                };
                
                // Uruchom proces
                process.Start();
                process.BeginOutputReadLine();
                
                // Poczekaj na zakończenie procesu
                await Task.Run(() => process.WaitForExit());
                
                // Sprawdź, czy proces zakończył się pomyślnie
                if (process.ExitCode != 0)
                {
                    string error = process.StandardError.ReadToEnd();
                    throw new Exception($"Błąd podczas transkrypcji: {error}");
                }
                
                // Sprawdź, czy plik wyjściowy został utworzony
                if (File.Exists(outputPath))
                {
                    return outputPath;
                }
                else
                {
                    throw new Exception("Nie udało się utworzyć pliku transkrypcji");
                }
            }
        }
        
        public bool CheckWhisperInstallation()
        {
            return File.Exists(whisperPath);
        }
        
        public bool CheckModelInstallation()
        {
            return File.Exists(modelPath);
        }
    }
} 