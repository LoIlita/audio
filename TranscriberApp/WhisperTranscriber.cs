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
    private string _pythonPath;
    private string _scriptPath;
    private string _outputDirectory;
    private string _modelSize;

    /// <summary>
    /// Pobiera lub ustawia rozmiar modelu Whisper.
    /// </summary>
    public string ModelSize
    {
        get => _modelSize;
        set
        {
            if (value != "tiny" && value != "base" && value != "small" && 
                value != "medium" && value != "large")
            {
                throw new ArgumentException("Nieprawidłowy rozmiar modelu. Dozwolone wartości: tiny, base, small, medium, large");
            }
            _modelSize = value;
        }
    }

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
        string modelSize = "medium")
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
    public async Task<string?> TranscribeAsync(string audioFilePath, Action<int, string?>? progressCallback = null)
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
    /// Wykonuje transkrypcję pliku audio z dodatkowymi opcjami.
    /// </summary>
    /// <param name="audioFilePath">Ścieżka do pliku audio</param>
    /// <param name="options">Opcje transkrypcji, w tym język, jakość itp.</param>
    /// <param name="progressCallback">Funkcja zwrotna do raportowania postępu</param>
    /// <returns>Ścieżka do pliku z transkrypcją lub null w przypadku błędu</returns>
    public async Task<string?> TranscribeWithOptionsAsync(string audioFilePath, Dictionary<string, object> options, Action<int, string?>? progressCallback = null)
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

        // Pobierz opcje transkrypcji
        string language = options.ContainsKey("language") ? options["language"]?.ToString() ?? "pl" : "pl";
        bool addPunctuation = options.ContainsKey("add_punctuation") && (bool)options["add_punctuation"];
        bool highQuality = options.ContainsKey("high_quality") && (bool)options["high_quality"];

        // Przygotowanie ścieżki wyjściowej
        string fileName = Path.GetFileNameWithoutExtension(audioFilePath);
        string outputPath = Path.Combine(_outputDirectory, $"{fileName}.md");
        
        // Uruchomienie procesu Pythona z opcjami
        try
        {
            return await RunPythonScriptWithOptionsAsync(audioFilePath, outputPath, language, addPunctuation, highQuality, options, progressCallback);
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
    private async Task<string?> RunPythonScriptAsync(string audioFilePath, string outputPath, Action<int, string?>? progressCallback = null)
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
                // Jeśli to nie jest JSON, zapisujemy do debug i kontynuujemy
                if (!args.Data.TrimStart().StartsWith("{"))
                {
                    Debug.WriteLine($"Non-JSON output: {args.Data}");
                    return;
                }
                
                // Próba parsowania JSON
                var jsonData = JsonDocument.Parse(args.Data);
                var root = jsonData.RootElement;

                // Sprawdzenie, czy mamy informację o postępie
                if (root.TryGetProperty("progress", out var progressElement))
                {
                    int progress = progressElement.GetInt32();
                    
                    // Sprawdź, czy mamy informację o pozostałym czasie
                    string? statusInfo = null;
                    if (root.TryGetProperty("time_remaining", out var timeRemainingElement))
                    {
                        double timeRemaining = timeRemainingElement.GetDouble();
                        statusInfo = $"(Pozostało: {FormatTimeRemaining(timeRemaining)})";
                    }
                    
                    progressCallback?.Invoke(progress, statusInfo);
                }
                
                // Sprawdzenie, czy mamy informację o statusie lub szacowanym czasie
                if (root.TryGetProperty("info", out var infoElement))
                {
                    string info = infoElement.GetString() ?? "";
                    Debug.WriteLine($"Info: {info}");
                    progressCallback?.Invoke(-1, info); // Użyj -1 jako specjalnego kodu dla informacji
                }
                
                // Sprawdzenie, czy mamy informację o szacowanym czasie
                if (root.TryGetProperty("estimated_time", out var estimatedTimeElement))
                {
                    double estimatedTime = estimatedTimeElement.GetDouble();
                    string info = $"Szacowany czas transkrypcji: {FormatTimeRemaining(estimatedTime)}";
                    progressCallback?.Invoke(-1, info); // Użyj -1 jako specjalnego kodu dla informacji
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
                    string errorMessage = errorElement.GetString() ?? "Nieznany błąd";
                    Debug.WriteLine($"Error from Python: {errorMessage}");
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                // Jeśli nie jest to JSON, po prostu zapisujemy wyjście
                Debug.WriteLine($"Python stdout (not JSON): {args.Data}");
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

    /// <summary>
    /// Uruchamia skrypt Python do transkrypcji z dodatkowymi opcjami.
    /// </summary>
    /// <param name="audioFilePath">Ścieżka do pliku audio</param>
    /// <param name="outputPath">Ścieżka do pliku wyjściowego</param>
    /// <param name="language">Kod języka transkrypcji</param>
    /// <param name="addPunctuation">Czy dodać interpunkcję</param>
    /// <param name="highQuality">Czy użyć wysokiej jakości transkrypcji</param>
    /// <param name="options">Dodatkowe opcje transkrypcji</param>
    /// <param name="progressCallback">Funkcja zwrotna do raportowania postępu</param>
    /// <returns>Ścieżka do pliku z transkrypcją lub null</returns>
    private async Task<string?> RunPythonScriptWithOptionsAsync(
        string audioFilePath, 
        string outputPath, 
        string language, 
        bool addPunctuation, 
        bool highQuality,
        Dictionary<string, object>? options = null,
        Action<int, string?>? progressCallback = null)
    {
        StringBuilder errorBuilder = new StringBuilder();
        Process? process = null;

        try
        {
            // Sprawdź, czy w opcjach jest nowy rozmiar modelu
            if (options?.ContainsKey("model_size") == true)
            {
                ModelSize = options["model_size"].ToString() ?? "medium";
            }

            // Przygotowanie procesu
            process = new Process();
            process.StartInfo.FileName = _pythonPath;
            
            // Dodaj podstawowe argumenty
            StringBuilder argsBuilder = new StringBuilder();
            argsBuilder.Append($"\"{_scriptPath}\" \"{audioFilePath}\" --output \"{outputPath}\" --model {_modelSize}");
            
            // Dodaj argument języka
            argsBuilder.Append($" --language {language}");
            
            // Dodaj opcje jakości
            if (highQuality)
            {
                argsBuilder.Append(" --best_of 5 --beam_size 5");
            }
            
            // Dodaj opcję interpunkcji
            if (addPunctuation)
            {
                argsBuilder.Append(" --punctuation");
            }
            
            process.StartInfo.Arguments = argsBuilder.ToString();
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
                if (string.IsNullOrEmpty(args.Data)) return;
                
                try
                {
                    Debug.WriteLine($"Python output: {args.Data}");
                    
                    // Bezpieczne sprawdzenie czy to JSON
                    bool isJson = !string.IsNullOrEmpty(args.Data) && 
                                  args.Data.TrimStart().StartsWith("{") && 
                                  args.Data.TrimEnd().EndsWith("}");
                    
                    if (!isJson)
                    {
                        // Nie JSON, po prostu zapisujemy wyjście
                        return;
                    }
                    
                    // Próba parsowania JSON
                    var jsonData = JsonDocument.Parse(args.Data);
                    var root = jsonData.RootElement;

                    // Bezpieczne wywołanie callback'a
                    SafeInvokeCallback(progressCallback, root);
                    
                    // Sprawdzenie, czy mamy wynikową ścieżkę pliku
                    if (root.TryGetProperty("output_path", out var outputPathElement))
                    {
                        string? path = outputPathElement.GetString();
                        if (!string.IsNullOrEmpty(path))
                        {
                            outputBuilder.Clear();
                            outputBuilder.Append(path);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing Python output: {ex.Message}");
                }
            };
            
            // Obsługa wyjścia błędów
            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    errorBuilder.AppendLine(args.Data);
                    Debug.WriteLine($"Python stderr: {args.Data}");
                }
            };

            // Uruchomienie procesu z obsługą wyjątków
            try
            {
                if (!process.Start())
                {
                    throw new Exception("Nie udało się uruchomić procesu Python");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas uruchamiania procesu Python: {ex.Message}", ex);
            }
            
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            // Bezpieczne czekanie na zakończenie procesu
            await process.WaitForExitAsync();
            
            // Sprawdzenie kodu wyjścia
            if (process.ExitCode != 0)
            {
                string errorMessage = errorBuilder.ToString();
                throw new Exception($"Proces Pythona zakończył się z kodem {process.ExitCode}: {errorMessage}");
            }
            
            // Zwrócenie ścieżki wyniku lub ścieżki domyślnej
            string resultPath = outputBuilder.ToString();
            if (string.IsNullOrEmpty(resultPath))
            {
                resultPath = outputPath;
            }
            
            // Sprawdź, czy plik istnieje
            if (!File.Exists(resultPath))
            {
                throw new FileNotFoundException($"Plik transkrypcji nie został utworzony w ścieżce {resultPath}");
            }
            
            return resultPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas uruchamiania skryptu Python: {ex.Message}\n{ex.StackTrace}");
            
            // Spróbuj zabić proces Python, jeśli wciąż działa
            try
            {
                if (process != null && !process.HasExited)
                {
                    process.Kill(true);
                }
            }
            catch (Exception killEx)
            {
                Debug.WriteLine($"Błąd podczas zatrzymywania procesu Python: {killEx.Message}");
            }
            
            throw new Exception($"Błąd transkrypcji: {ex.Message}", ex);
        }
        finally
        {
            // Zwolnij zasoby procesu
            process?.Dispose();
        }
    }
    
    // Nowa metoda do bezpiecznego wywołania callback'a
    private void SafeInvokeCallback(Action<int, string?>? callback, JsonElement root)
    {
        if (callback == null) return;
        
        try
        {
            // Sprawdzenie, czy mamy informację o postępie
            if (root.TryGetProperty("progress", out var progressElement))
            {
                int progress = progressElement.GetInt32();
                
                // Sprawdź, czy mamy informację o pozostałym czasie
                string? statusInfo = null;
                if (root.TryGetProperty("time_remaining", out var timeRemainingElement))
                {
                    double timeRemaining = timeRemainingElement.GetDouble();
                    statusInfo = $"(Pozostało: {FormatTimeRemaining(timeRemaining)})";
                }
                
                callback(progress, statusInfo);
            }
            
            // Sprawdzenie, czy mamy informację o statusie
            if (root.TryGetProperty("info", out var infoElement))
            {
                string? info = infoElement.GetString();
                if (!string.IsNullOrEmpty(info))
                {
                    callback(-1, info); // Użyj -1 jako specjalnego kodu dla informacji
                }
            }
            
            // Sprawdzenie, czy mamy informację o szacowanym czasie
            if (root.TryGetProperty("estimated_time", out var estimatedTimeElement))
            {
                double estimatedTime = estimatedTimeElement.GetDouble();
                string info = $"Szacowany czas transkrypcji: {FormatTimeRemaining(estimatedTime)}";
                callback(-1, info);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in SafeInvokeCallback: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Formatuje czas w sekundach do formatu czytelnego dla człowieka.
    /// </summary>
    /// <param name="seconds">Czas w sekundach</param>
    /// <returns>Sformatowany czas</returns>
    private string FormatTimeRemaining(double seconds)
    {
        if (seconds < 60)
        {
            return $"{(int)seconds} sek.";
        }
        else if (seconds < 3600)
        {
            int minutes = (int)(seconds / 60);
            int remainingSeconds = (int)(seconds % 60);
            return $"{minutes} min. {remainingSeconds} sek.";
        }
        else
        {
            int hours = (int)(seconds / 3600);
            int minutes = (int)((seconds % 3600) / 60);
            return $"{hours} godz. {minutes} min.";
        }
    }
} 