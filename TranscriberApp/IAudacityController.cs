using System;
using System.Threading.Tasks;

namespace TranscriberApp;

/// <summary>
/// Interfejs definiujący funkcjonalność sterowania zewnętrzną aplikacją.
/// </summary>
public interface IExternalAppController
{
    /// <summary>
    /// Sprawdza, czy aplikacja jest uruchomiona.
    /// </summary>
    /// <returns>True, jeśli aplikacja jest uruchomiona</returns>
    Task<bool> IsRunningAsync();
    
    /// <summary>
    /// Uruchamia aplikację, jeśli nie jest już uruchomiona (wersja asynchroniczna).
    /// </summary>
    /// <returns>True, jeśli uruchomienie się powiodło lub aplikacja już działa</returns>
    Task<bool> StartAppAsync();
    
    /// <summary>
    /// Uruchamia aplikację, jeśli nie jest już uruchomiona (wersja synchroniczna).
    /// </summary>
    /// <returns>True, jeśli uruchomienie się powiodło lub aplikacja już działa</returns>
    bool StartApp();
} 