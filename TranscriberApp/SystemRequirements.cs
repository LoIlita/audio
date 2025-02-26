using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TranscriberApp;

/// <summary>
/// Klasa odpowiedzialna za sprawdzanie, czy wymagane zależności systemowe są zainstalowane.
/// </summary>
public static class SystemRequirements
{
    /// <summary>
    /// Sprawdza, czy Python jest zainstalowany i zwraca jego wersję.
    /// </summary>
    /// <param name="pythonPath">Ścieżka do interpretera Python (domyślnie 'python')</param>
    /// <returns>Informacja o wersji Python lub null, jeśli nie jest zainstalowany</returns>
    public static async Task<string?> CheckPythonAsync(string pythonPath = "python")
    {
        try
        {
            using var process = new Process();
            process.StartInfo.FileName = pythonPath;
            process.StartInfo.Arguments = "--version";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            
            var outputBuilder = new StringBuilder();
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                    outputBuilder.AppendLine(args.Data);
            };
            
            process.Start();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
            
            if (process.ExitCode == 0)
            {
                string output = outputBuilder.ToString().Trim();
                // Typowo formatem output jest "Python 3.x.y"
                var versionMatch = Regex.Match(output, @"Python (\d+\.\d+\.\d+)");
                if (versionMatch.Success)
                {
                    return versionMatch.Groups[1].Value;
                }
                return output;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Sprawdza, czy pakiet Whisper jest zainstalowany w Pythonie.
    /// </summary>
    /// <param name="pythonPath">Ścieżka do interpretera Python (domyślnie 'python')</param>
    /// <returns>True, jeśli Whisper jest zainstalowany, False w przeciwnym razie</returns>
    public static async Task<bool> CheckWhisperInstalledAsync(string pythonPath = "python")
    {
        try
        {
            using var process = new Process();
            process.StartInfo.FileName = pythonPath;
            process.StartInfo.Arguments = "-c \"import whisper; print('Whisper installed')\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            
            var outputBuilder = new StringBuilder();
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                    outputBuilder.AppendLine(args.Data);
            };
            
            process.Start();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
            
            return process.ExitCode == 0 && outputBuilder.ToString().Contains("Whisper installed");
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Sprawdza, czy PyTorch jest zainstalowany i ma obsługę CUDA.
    /// </summary>
    /// <param name="pythonPath">Ścieżka do interpretera Python (domyślnie 'python')</param>
    /// <returns>True, jeśli PyTorch z CUDA jest dostępny, False w przeciwnym razie</returns>
    public static async Task<bool> CheckPyTorchCudaAsync(string pythonPath = "python")
    {
        try
        {
            using var process = new Process();
            process.StartInfo.FileName = pythonPath;
            process.StartInfo.Arguments = "-c \"import torch; print(f'PyTorch: {torch.__version__}, CUDA available: {torch.cuda.is_available()}')\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            
            var outputBuilder = new StringBuilder();
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                    outputBuilder.AppendLine(args.Data);
            };
            
            process.Start();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
            
            if (process.ExitCode == 0)
            {
                // Sprawdzamy, czy CUDA jest dostępne
                return outputBuilder.ToString().Contains("CUDA available: True");
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Sprawdza wszystkie wymagania systemowe i zwraca wynik w formie obiektu.
    /// </summary>
    /// <param name="pythonPath">Ścieżka do interpretera Python</param>
    /// <returns>Obiekt zawierający informacje o zainstalowanych zależnościach</returns>
    public static async Task<RequirementsCheckResult> CheckAllRequirementsAsync(string pythonPath = "python")
    {
        var result = new RequirementsCheckResult();
        
        // Sprawdzamy Python
        result.PythonVersion = await CheckPythonAsync(pythonPath);
        result.IsPythonInstalled = result.PythonVersion != null;
        
        if (result.IsPythonInstalled)
        {
            // Sprawdzamy Whisper
            result.IsWhisperInstalled = await CheckWhisperInstalledAsync(pythonPath);
            
            // Sprawdzamy PyTorch i CUDA
            result.IsCudaAvailable = await CheckPyTorchCudaAsync(pythonPath);
        }
        
        return result;
    }
    
    /// <summary>
    /// Informuje, czy system spełnia minimalne wymagania do działania transkrypcji.
    /// </summary>
    /// <param name="result">Wynik sprawdzenia wymagań</param>
    /// <returns>True, jeśli minimalne wymagania są spełnione</returns>
    public static bool MeetsMinimumRequirements(RequirementsCheckResult result)
    {
        // Minimalne wymagania to zainstalowany Python i Whisper
        return result.IsPythonInstalled && result.IsWhisperInstalled;
    }
}

/// <summary>
/// Klasa przechowująca wyniki sprawdzenia wymagań systemowych.
/// </summary>
public class RequirementsCheckResult
{
    /// <summary>
    /// Czy Python jest zainstalowany
    /// </summary>
    public bool IsPythonInstalled { get; set; }
    
    /// <summary>
    /// Wersja Pythona
    /// </summary>
    public string? PythonVersion { get; set; }
    
    /// <summary>
    /// Czy Whisper jest zainstalowany
    /// </summary>
    public bool IsWhisperInstalled { get; set; }
    
    /// <summary>
    /// Czy PyTorch ma dostęp do CUDA (przyspieszenie GPU)
    /// </summary>
    public bool IsCudaAvailable { get; set; }
    
    /// <summary>
    /// Zwraca tekstową reprezentację wyników sprawdzenia.
    /// </summary>
    public string GetSummary()
    {
        var summary = new StringBuilder();
        
        summary.AppendLine("Wyniki sprawdzania wymagań systemowych:");
        summary.AppendLine($"Python zainstalowany: {(IsPythonInstalled ? "Tak" : "Nie")}");
        
        if (IsPythonInstalled && PythonVersion != null)
        {
            summary.AppendLine($"Wersja Python: {PythonVersion}");
        }
        
        summary.AppendLine($"Whisper zainstalowany: {(IsWhisperInstalled ? "Tak" : "Nie")}");
        summary.AppendLine($"Dostępne przyspieszenie GPU (CUDA): {(IsCudaAvailable ? "Tak" : "Nie")}");
        
        if (!IsPythonInstalled)
        {
            summary.AppendLine("\nAby kontynuować, zainstaluj Python: https://www.python.org/downloads/");
        }
        
        if (IsPythonInstalled && !IsWhisperInstalled)
        {
            summary.AppendLine("\nAby kontynuować, zainstaluj Whisper:");
            summary.AppendLine("pip install openai-whisper");
        }
        
        return summary.ToString();
    }
} 