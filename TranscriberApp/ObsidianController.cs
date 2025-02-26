using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TranscriberApp;

/// <summary>
/// Klasa implementująca sterowanie aplikacją Obsidian.
/// </summary>
public class ObsidianController : IExternalAppController
{
    private const string ObsidianProcessName = "Obsidian";
    
    // Typowe lokalizacje instalacji Obsidian
    private static readonly string[] PossibleObsidianPaths = {
        // Ścieżki AppData - typowe dla aplikacji typu Electron
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Obsidian", "Obsidian.exe"),
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Obsidian", "Obsidian.exe"),
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Obsidian", "Obsidian.exe"),
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Roaming", "Obsidian", "Obsidian.exe"),
        
        // Potencjalne ścieżki instalacji
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Obsidian", "Obsidian.exe"),
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Obsidian", "Obsidian.exe"),
        @"C:\Program Files\Obsidian\Obsidian.exe",
        @"C:\Program Files (x86)\Obsidian\Obsidian.exe"
    };
    
    /// <summary>
    /// Inicjalizuje nową instancję klasy ObsidianController.
    /// </summary>
    public ObsidianController()
    {
    }
    
    /// <summary>
    /// Sprawdza, czy Obsidian jest uruchomiony.
    /// </summary>
    /// <returns>True, jeśli Obsidian jest uruchomiony</returns>
    public Task<bool> IsRunningAsync()
    {
        var isRunning = Process.GetProcessesByName(ObsidianProcessName).Any();
        return Task.FromResult(isRunning);
    }
    
    /// <summary>
    /// Uruchamia Obsidian, jeśli nie jest już uruchomiony (wersja synchroniczna).
    /// </summary>
    /// <returns>True, jeśli uruchomienie się powiodło lub Obsidian już działa</returns>
    public bool StartApp()
    {
        // Wywołujemy wersję asynchroniczną i czekamy na jej zakończenie
        return StartAppAsync().GetAwaiter().GetResult();
    }
    
    /// <summary>
    /// Uruchamia Obsidian, jeśli nie jest już uruchomiony (wersja asynchroniczna).
    /// </summary>
    /// <returns>True, jeśli uruchomienie się powiodło lub Obsidian już działa</returns>
    public async Task<bool> StartAppAsync()
    {
        if (await IsRunningAsync())
            return true;
        
        try
        {
            // Znajdź ścieżkę do pliku wykonywalnego Obsidian
            string obsidianPath = FindObsidianExecutable();
            
            if (string.IsNullOrEmpty(obsidianPath))
            {
                Debug.WriteLine("Nie znaleziono pliku wykonawczego Obsidian.");
                return false;
            }
            
            // Uruchomienie Obsidian
            var processInfo = new ProcessStartInfo
            {
                FileName = obsidianPath,
                UseShellExecute = true
            };
            
            Process.Start(processInfo);
            
            // Poczekaj na uruchomienie, sprawdzając co 100ms zamiast 500ms
            int attempts = 0;
            const int maxAttempts = 20; // Zwiększamy liczbę prób, ale z krótszym opóźnieniem
            
            while (!await IsRunningAsync() && attempts < maxAttempts)
            {
                await Task.Delay(100); // Krótsze opóźnienie
                attempts++;
            }
            
            // Jeśli Obsidian uruchomił się pomyślnie, dajemy mu tylko 500ms na inicjalizację
            // zamiast 2 sekund, ponieważ nie potrzebujemy czekać aż będzie całkowicie gotowy
            if (await IsRunningAsync())
            {
                await Task.Delay(500);
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas uruchamiania Obsidian: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Znajduje ścieżkę do pliku wykonywalnego Obsidian.
    /// </summary>
    /// <returns>Ścieżka do pliku Obsidian.exe lub pusty string jeśli nie znaleziono</returns>
    private string FindObsidianExecutable()
    {
        Debug.WriteLine("Szukanie pliku wykonywalnego Obsidian w znanych lokalizacjach...");
        
        // Sprawdź znane lokalizacje
        foreach (var path in PossibleObsidianPaths)
        {
            Debug.WriteLine($"Sprawdzam ścieżkę: {path}");
            if (File.Exists(path))
            {
                Debug.WriteLine($"Znaleziono Obsidian w: {path}");
                return path;
            }
        }
        
        // Sprawdź w PATH - jeśli Obsidian jest dostępny globalnie
        var pathVariable = Environment.GetEnvironmentVariable("PATH");
        if (!string.IsNullOrEmpty(pathVariable))
        {
            foreach (var directory in pathVariable.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(directory, "Obsidian.exe");
                if (File.Exists(fullPath))
                {
                    Debug.WriteLine($"Znaleziono Obsidian w PATH: {fullPath}");
                    return fullPath;
                }
            }
        }
        
        Debug.WriteLine("Nie znaleziono pliku wykonywalnego Obsidian.");
        return string.Empty;
    }
} 