using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

namespace TranscriberApp;

/// <summary>
/// Implementacja transkrybera wykorzystująca Whisper AI poprzez skrypt Python.
/// </summary>
public class WhisperTranscriber : ITranscriber
{
    private readonly string _pythonPath;
    private readonly string _scriptPath;
    private readonly string _outputDirectory;
    private readonly string _modelSize;

    /// <summary>
    /// Inicjalizuje nową instancję klasy WhisperTranscriber.
    /// </summary>
    /// <param name="pythonPath">Ścieżka do interpretera Python (domyślnie 'python')</param>
    /// <param name="scriptPath">Ścieżka do skryptu Whisper (domyślnie utworzona w katalogu bieżącym)</param>
    /// <param name="outputDirectory">Katalog wyjściowy dla transkrypcji (domyślnie 'transcriptions')</param>
    /// <param name="modelSize">Rozmiar modelu Whisper do użycia (tiny, base, small, medium, large)</param>
    public WhisperTranscriber(
        string pythonPath = "python", 
        string? scriptPath = null, 
        string outputDirectory = "transcriptions",
        string modelSize = "small")
    {
        _pythonPath = pythonPath;
        _scriptPath = scriptPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PythonScripts", "whisper_transcribe.py");
        _outputDirectory = outputDirectory;
        _modelSize = modelSize;
        
        // Upewnij się, że katalog wyjściowy istnieje
        Directory.CreateDirectory(_outputDirectory);
        
        // Upewnij się, że katalog ze skryptem istnieje
        Directory.CreateDirectory(Path.GetDirectoryName(_scriptPath) ?? string.Empty);
    }

    /// <inheritdoc/>
    public async Task<string?> TranscribeAsync(string audioFilePath, Action<int>? progressCallback = null)
    {
        // Sprawdzenie, czy pliki istnieją
        if (!File.Exists(audioFilePath))
        {
            throw new FileNotFoundException("Nie znaleziono pliku audio", audioFilePath);
        }

        if (!File.Exists(_scriptPath))
        {
            throw new FileNotFoundException("Nie znaleziono skryptu Pythona", _scriptPath);
        }

        // Przygotowanie ścieżki wyjściowej
        string fileName = Path.GetFileNameWithoutExtension(audioFilePath);
        string outputPath = Path.Combine(_outputDirectory, $"{fileName}.md");
        
        // Uruchomienie procesu Pythona
        try
        {
            return await RunPythonScriptAsync(audioFilePath, outputPath, progressCallback);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas uruchamiania skryptu Python: {ex.Message}");
            throw new Exception($"Błąd transkrypcji: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Uruchamia skrypt Python do transkrypcji.
    /// </summary>
    /// <param name="audioFilePath">Ścieżka do pliku audio</param>
    /// <param name="outputPath">Ścieżka do pliku wyjściowego</param>
    /// <param name="progressCallback">Funkcja zwrotna do raportowania postępu</param>
    /// <returns>Ścieżka do pliku z transkrypcją lub null</returns>
    private async Task<string?> RunPythonScriptAsync(string audioFilePath, string outputPath, Action<int>? progressCallback = null)
    {
        // Przygotowanie procesu
        using var process = new Process();
        process.StartInfo.FileName = _pythonPath;
        process.StartInfo.Arguments = $"\"{_scriptPath}\" \"{audioFilePath}\" --output \"{outputPath}\" --model {_modelSize}";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
        process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

        // Bufor dla ostatniej linii z wynikiem
        StringBuilder outputBuilder = new StringBuilder();
        
        // Obsługa standardowego wyjścia (do odczytu postępu i wyniku)
        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data == null) return;
            
            try
            {
                // Próba parsowania JSON
                var jsonData = JsonDocument.Parse(args.Data);
                var root = jsonData.RootElement;

                // Sprawdzenie, czy mamy informację o postępie
                if (root.TryGetProperty("progress", out var progressElement))
                {
                    int progress = progressElement.GetInt32();
                    progressCallback?.Invoke(progress);
                }
                
                // Sprawdzenie, czy mamy wynikową ścieżkę pliku
                if (root.TryGetProperty("output_path", out var outputPathElement))
                {
                    outputBuilder.Clear();
                    outputBuilder.Append(outputPathElement.GetString());
                }
                
                // Sprawdzenie, czy wystąpił błąd
                if (root.TryGetProperty("error", out var errorElement))
                {
                    throw new Exception(errorElement.GetString());
                }
            }
            catch (JsonException)
            {
                // Jeśli nie jest to JSON, po prostu zapisujemy wyjście
                Debug.WriteLine($"Python stdout: {args.Data}");
            }
        };

        // Obsługa błędów
        var errorBuilder = new StringBuilder();
        process.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data == null) return;
            errorBuilder.AppendLine(args.Data);
            Debug.WriteLine($"Python stderr: {args.Data}");
        };

        // Uruchomienie procesu
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        
        // Czekamy na zakończenie procesu
        await process.WaitForExitAsync();

        // Sprawdzamy kod wyjścia
        if (process.ExitCode != 0)
        {
            string errorMessage = errorBuilder.ToString();
            throw new Exception($"Skrypt Python zakończył się z błędem (kod {process.ExitCode}): {errorMessage}");
        }

        // Zwracamy ścieżkę z wyniku lub domyślną ścieżkę
        string resultPath = outputBuilder.Length > 0 ? outputBuilder.ToString() : outputPath;
        
        // Sprawdzamy, czy plik transkrypcji istnieje
        if (!File.Exists(resultPath))
        {
            throw new FileNotFoundException("Plik transkrypcji nie został utworzony", resultPath);
        }
        
        return resultPath;
    }

    /// <inheritdoc/>
    public string TranscribeAudioFile(string audioFilePath, Dictionary<string, object> options)
    {
        // Sprawdzenie, czy plik istnieje
        if (!File.Exists(audioFilePath))
        {
            throw new FileNotFoundException("Nie znaleziono pliku audio", audioFilePath);
        }

        if (!File.Exists(_scriptPath))
        {
            throw new FileNotFoundException("Nie znaleziono skryptu Pythona", _scriptPath);
        }

        // Pobierz opcje transkrypcji
        string language = options.ContainsKey("language") ? options["language"].ToString() : "pl";
        bool addPunctuation = options.ContainsKey("add_punctuation") && (bool)options["add_punctuation"];
        bool highQuality = options.ContainsKey("high_quality") && (bool)options["high_quality"];
        
        // Przygotowanie argumentów dla skryptu Python
        string arguments = $"\"{_scriptPath}\" \"{audioFilePath}\" --model {_modelSize} --language {language}";
        
        // Dodaj opcje jakości
        if (highQuality)
        {
            arguments += " --best_of 5 --beam_size 5";
        }
        
        // Dodaj opcję interpunkcji
        if (addPunctuation)
        {
            arguments += " --punctuation";
        }
        
        // Uruchomienie procesu Pythona
        try
        {
            return RunPythonScriptForTranscription(arguments);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas uruchamiania skryptu Python: {ex.Message}");
            throw new Exception($"Błąd transkrypcji: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Uruchamia skrypt Python do transkrypcji i zwraca tekst transkrypcji.
    /// </summary>
    /// <param name="arguments">Argumenty dla procesu Python</param>
    /// <returns>Tekst transkrypcji</returns>
    private string RunPythonScriptForTranscription(string arguments)
    {
        // Przygotowanie procesu
        using var process = new Process();
        process.StartInfo.FileName = _pythonPath;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
        process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

        // Bufor dla wyjścia
        StringBuilder outputBuilder = new StringBuilder();
        
        // Obsługa standardowego wyjścia
        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                outputBuilder.AppendLine(args.Data);
            }
        };

        // Obsługa błędów
        var errorBuilder = new StringBuilder();
        process.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                errorBuilder.AppendLine(args.Data);
                Debug.WriteLine($"Python stderr: {args.Data}");
            }
        };

        // Uruchomienie procesu
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        
        // Czekamy na zakończenie procesu
        process.WaitForExit();

        // Sprawdzamy kod wyjścia
        if (process.ExitCode != 0)
        {
            string errorMessage = errorBuilder.ToString();
            throw new Exception($"Skrypt Python zakończył się z błędem (kod {process.ExitCode}): {errorMessage}");
        }

        // Zwracamy tekst transkrypcji
        return outputBuilder.ToString().Trim();
    }
} 