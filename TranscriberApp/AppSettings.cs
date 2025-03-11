using System;
using System.IO;
using System.Text.Json;

namespace TranscriberApp
{
    /// <summary>
    /// Klasa obsługująca ustawienia aplikacji
    /// </summary>
    public class AppSettings
    {
        // Ścieżka do pliku ustawień
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TranscriberApp",
            "settings.json");
            
        // Domyślne wartości ustawień
        public const string DefaultLanguageCode = "pl";
        public const string DefaultModelSize = "medium";
        public const bool DefaultHighQuality = false;
        public const bool DefaultAddPunctuation = true;
            
        // Właściwości ustawień
        public string LanguageCode { get; set; } = DefaultLanguageCode;
        public string ModelSize { get; set; } = DefaultModelSize;
        public bool HighQualityTranscription { get; set; } = DefaultHighQuality;
        public bool AddPunctuation { get; set; } = DefaultAddPunctuation;
        
        /// <summary>
        /// Wczytuje ustawienia z pliku
        /// </summary>
        /// <returns>Załadowane ustawienia lub domyślne ustawienia jeśli nie można załadować pliku</returns>
        public static AppSettings Load()
        {
            try
            {
                // Upewnij się, że katalog istnieje
                string directory = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // Sprawdź czy plik z ustawieniami istnieje
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    
                    // Jeśli deserializacja się nie powiodła, użyj domyślnych ustawień
                    return settings ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                // W przypadku błędu zapisz informację w logach
                System.Diagnostics.Debug.WriteLine($"Błąd podczas wczytywania ustawień: {ex.Message}");
            }
            
            // Jeśli plik nie istnieje lub wystąpił błąd, użyj domyślnych ustawień
            return new AppSettings();
        }
        
        /// <summary>
        /// Zapisuje ustawienia do pliku
        /// </summary>
        public void Save()
        {
            try
            {
                // Upewnij się, że katalog istnieje
                string directory = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // Serializuj ustawienia do JSON i zapisz do pliku
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                // W przypadku błędu zapisz informację w logach
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania ustawień: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Przywraca ustawienia fabryczne
        /// </summary>
        public void RestoreDefaults()
        {
            LanguageCode = DefaultLanguageCode;
            ModelSize = DefaultModelSize;
            HighQualityTranscription = DefaultHighQuality;
            AddPunctuation = DefaultAddPunctuation;
        }
    }
} 