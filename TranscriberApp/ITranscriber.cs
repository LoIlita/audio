namespace TranscriberApp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Interfejs definiujący funkcjonalność transkrypcji.
/// </summary>
public interface ITranscriber
{
    /// <summary>
    /// Wykonuje transkrypcję pliku audio.
    /// </summary>
    /// <param name="audioFilePath">Ścieżka do pliku audio</param>
    /// <param name="progressCallback">Opcjonalna funkcja zwrotna do raportowania postępu (0-100) i informacji o statusie</param>
    /// <returns>Ścieżka do pliku z transkrypcją lub null w przypadku błędu</returns>
    Task<string?> TranscribeAsync(string audioFilePath, Action<int, string?>? progressCallback = null);
    
    /// <summary>
    /// Wykonuje transkrypcję pliku audio z dodatkowymi opcjami.
    /// </summary>
    /// <param name="audioFilePath">Ścieżka do pliku audio</param>
    /// <param name="options">Słownik z opcjami transkrypcji</param>
    /// <param name="progressCallback">Opcjonalna funkcja zwrotna do raportowania postępu i informacji o statusie</param>
    /// <returns>Ścieżka do pliku z transkrypcją</returns>
    Task<string?> TranscribeWithOptionsAsync(string audioFilePath, Dictionary<string, object> options, Action<int, string?>? progressCallback = null);
} 