using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

namespace TranscriberApp;

public partial class Form1 : Form
{
    private string selectedFilePath = string.Empty;
    private ITranscriber? transcriber;
    private string transcriptionText = string.Empty;
    private string? transcriptionFilePath = string.Empty;
    private string lastTranscriptionFilePath = string.Empty;
    private Stopwatch stopwatch = new Stopwatch();
    private CancellationTokenSource? cancellationTokenSource;
    
    // Flaga wskazująca, czy bieżący plik został już przetranskrybowany
    private bool currentFileTranscribed = false;
    
    // Ustawienia opcji
    private string selectedLanguageCode = "pl";
    private string selectedModelSize = "medium";
    private bool highQualityTranscription = false;
    private bool addPunctuation = true;
    
    // Obiekt ustawień aplikacji
    private AppSettings appSettings = new AppSettings();

    public Form1()
    {
        InitializeComponent();
        
        // Dodanie globalnego handlera wyjątków dla wątku UI
        Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        
        // Wczytaj ustawienia
        LoadSettings();
        
        InitializeTranscriber();
    }
    
    // Metoda wczytująca ustawienia
    private void LoadSettings()
    {
        try
        {
            // Wczytaj ustawienia z pliku
            appSettings = AppSettings.Load();
            
            // Zastosuj wczytane ustawienia
            selectedLanguageCode = appSettings.LanguageCode;
            selectedModelSize = appSettings.ModelSize;
            highQualityTranscription = appSettings.HighQuality;
            addPunctuation = appSettings.AddPunctuation;
            
            Debug.WriteLine($"Wczytano ustawienia: język={selectedLanguageCode}, model={selectedModelSize}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas wczytywania ustawień: {ex.Message}");
            
            // W przypadku błędu użyj domyślnych ustawień
            appSettings = new AppSettings();
            selectedLanguageCode = appSettings.LanguageCode;
            selectedModelSize = appSettings.ModelSize;
            highQualityTranscription = appSettings.HighQuality;
            addPunctuation = appSettings.AddPunctuation;
        }
    }
    
    // Metoda zapisująca ustawienia
    private void SaveSettings()
    {
        try
        {
            // Aktualizuj obiekt ustawień
            appSettings.LanguageCode = selectedLanguageCode;
            appSettings.ModelSize = selectedModelSize;
            appSettings.HighQuality = highQualityTranscription;
            appSettings.AddPunctuation = addPunctuation;
            
            // Zapisz ustawienia
            appSettings.Save();
            
            Debug.WriteLine("Zapisano ustawienia");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas zapisywania ustawień: {ex.Message}");
        }
    }
    
    // Globalny handler wyjątków dla wątku UI
    private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        try
        {
            Exception ex = e.Exception;
            string errorMessage = $"Wystąpił nieoczekiwany błąd: {ex.Message}";
            
            if (ex.InnerException != null)
            {
                errorMessage += $"\n\nSzczegóły: {ex.InnerException.Message}";
            }
            
            Debug.WriteLine($"Thread Exception: {ex}");
            MessageBox.Show(errorMessage, "Błąd aplikacji", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch
        {
            try
            {
                MessageBox.Show("Wystąpił poważny błąd", "Poważny błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Nie zamykaj aplikacji
            }
        }
    }
    
    // Globalny handler dla nieobsłużonych wyjątków
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            Exception ex = (Exception)e.ExceptionObject;
            string errorMessage = $"Wystąpił nieoczekiwany błąd: {ex.Message}";
            
            if (ex.InnerException != null)
            {
                errorMessage += $"\n\nSzczegóły: {ex.InnerException.Message}";
            }
            
            Debug.WriteLine($"Unhandled Exception: {ex}");
            MessageBox.Show(errorMessage, "Błąd aplikacji", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch
        {
            try
            {
                MessageBox.Show("Wystąpił poważny błąd", "Poważny błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Nie możemy nic więcej zrobić
            }
        }
    }

    private void InitializeTranscriber()
    {
        try
        {
            var settings = AppSettings.Load();
            
            // Użyj standardowego transkrybera
            transcriber = new WhisperTranscriber(
                pythonPath: "python",
                scriptPath: null,
                outputDirectory: "transcriptions",
                modelSize: settings.ModelSize);
            
            lblStatus.Text = "Gotowy do transkrypcji";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd podczas inicjalizacji transkrybera: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Błąd inicjalizacji transkrybera";
        }
    }

    private void btnSelectFile_Click(object sender, EventArgs e)
    {
        try
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki audio (*.mp3;*.wav;*.flac;*.ogg;*.m4a)|*.mp3;*.wav;*.flac;*.ogg;*.m4a|Wszystkie pliki (*.*)|*.*";
            openFileDialog.Title = "Wybierz plik audio do transkrypcji";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
                lblStatus.Text = $"Status: Wybrano plik {Path.GetFileName(selectedFilePath)}";
                
                // Zresetuj flagę transkrypcji, ponieważ wybrano nowy plik
                currentFileTranscribed = false;
                
                // Teraz można transkrybować plik
                btnTranscribe.Enabled = true;
                
                // Wyświetl informację o wybranym pliku
                txtTranscription.Text = $"Wybrany plik: {Path.GetFileName(selectedFilePath)}\nGotowy do transkrypcji.";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd podczas wybierania pliku: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnTranscribe_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Proszę najpierw wybrać plik audio.", "Brak wybranego pliku", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sprawdź, czy plik został już przetranskrybowany
            if (currentFileTranscribed)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Ten plik został już przetranskrybowany. Czy chcesz przeprowadzić transkrypcję ponownie?",
                    "Plik już transkrybowany",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }

            if (transcriber == null)
            {
                MessageBox.Show("Transkryber nie został zainicjalizowany.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Wyłącz przyciski, aby uniknąć wielokrotnego kliknięcia
            btnTranscribe.Enabled = false;
            btnSelectFile.Enabled = false;
            btnOptions.Enabled = false;
            txtTranscription.Text = "Trwa transkrypcja...";
            lblStatus.Text = "Status: Przygotowanie do transkrypcji...";

            // Przygotuj opcje transkrypcji
            Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "language", selectedLanguageCode },
                { "add_punctuation", addPunctuation },
                { "high_quality", highQualityTranscription },
                { "model_size", selectedModelSize }
            };
            
            // Utwórz token anulowania
            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Funkcja zwrotna do raportowania postępu
            Action<int, string?> progressCallback = (progress, statusInfo) =>
            {
                try 
                {
                    // Sprawdź, czy anulowano operację
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                
                    // Używamy Invoke, aby aktualizować UI z innego wątku
                    if (lblStatus.InvokeRequired)
                    {
                        lblStatus.Invoke(new Action(() => 
                        {
                            UpdateStatusLabel(progress, statusInfo);
                        }));
                    }
                    else
                    {
                        UpdateStatusLabel(progress, statusInfo);
                    }
                }
                catch (Exception ex)
                {
                    // Rejestruj błędy, ale nie pozwól, aby zakończyły aplikację
                    Debug.WriteLine($"Błąd w callback: {ex.Message}");
                }
            };

            // Uruchom stoper
            stopwatch.Restart();
            
            // Zapisz kod wybranego języka dla logowania
            lblStatus.Text = $"Status: Rozpoczynam transkrypcję w języku {selectedLanguageCode}...";
            
            string? result = null;
            
            // Wykonaj transkrypcję w oddzielnym wątku z obsługą błędów
            try 
            {
                // Wykonaj transkrypcję w zadaniu z obsługą anulowania
                Task<string?> transcriptionTask;
                
                if (transcriber is WhisperTranscriber whisperTranscriber)
                {
                    // Użyj bardziej zaawansowanej metody z opcjami
                    transcriptionTask = whisperTranscriber.TranscribeWithOptionsAsync(selectedFilePath, options, progressCallback);
                }
                else
                {
                    // Dla innych implementacji użyj standardowej metody
                    transcriptionTask = transcriber.TranscribeAsync(selectedFilePath, progressCallback);
                }
                
                // Oczekuj na zakończenie z obsługą anulowania
                result = await transcriptionTask;
                
                // Przypisz wynik
                transcriptionFilePath = result;
            }
            catch (OperationCanceledException)
            {
                // Obsługa anulowania przez użytkownika
                lblStatus.Text = "Status: Transkrypcja została anulowana";
                txtTranscription.Text = "Transkrypcja anulowana przez użytkownika.";
                return;
            }
            catch (Exception ex)
            {
                // Szczegółowe logowanie błędu
                Debug.WriteLine($"Szczegóły błędu transkrypcji: {ex}");
                
                string errorMessage = $"Wystąpił błąd podczas transkrypcji: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nSzczegóły: {ex.InnerException.Message}";
                }
                
                lblStatus.Text = $"Status: Błąd - {ex.Message}";
                txtTranscription.Text = $"Wystąpił błąd: {ex.Message}";
                
                MessageBox.Show(errorMessage, "Błąd transkrypcji", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                // Zatrzymaj stoper i anuluj token
                stopwatch.Stop();
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
                
                // Włącz przyciski bez względu na wynik
                btnSelectFile.Enabled = true;
                btnOptions.Enabled = true;
            }
            
            // Przetwarzanie wyniku
            if (!string.IsNullOrEmpty(transcriptionFilePath) && File.Exists(transcriptionFilePath))
            {
                try 
                {
                    // Wczytaj transkrypcję
                    transcriptionText = File.ReadAllText(transcriptionFilePath);
                    txtTranscription.Text = transcriptionText;
                    btnClearTranscription.Enabled = true;
                    
                    // Automatycznie zapisz transkrypcję
                    SaveTranscriptionAutomatically(transcriptionText, transcriptionFilePath);
                    
                    // Zaktualizuj status
                    TimeSpan elapsed = stopwatch.Elapsed;
                    lblStatus.Text = $"Status: Transkrypcja zakończona (czas: {FormatElapsedTime(elapsed)})";
                    
                    // Oznacz plik jako przetranskrybowany
                    currentFileTranscribed = true;
                    
                    // Wyłącz przycisk transkrypcji, ponieważ plik został już przetranskrybowany
                    btnTranscribe.Enabled = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Błąd odczytu pliku transkrypcji: {ex}");
                    lblStatus.Text = $"Status: Błąd odczytu pliku transkrypcji - {ex.Message}";
                    txtTranscription.Text = "Błąd podczas odczytu pliku transkrypcji.";
                }
            }
            else
            {
                txtTranscription.Text = "Błąd podczas transkrypcji - nie udało się wygenerować pliku wyjściowego.";
                lblStatus.Text = "Status: Błąd transkrypcji - brak pliku wyjściowego";
            }
        }
        catch (Exception ex)
        {
            // Ostatnia linia obrony - przechwycenie wszystkich innych błędów
            Debug.WriteLine($"Nieoczekiwany błąd: {ex}");
            
            string errorMessage = $"Wystąpił nieoczekiwany błąd: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\n\nSzczegóły: {ex.InnerException.Message}";
            }
            
            MessageBox.Show(errorMessage, "Błąd podczas transkrypcji", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = $"Status: Nieoczekiwany błąd - {ex.Message}";
            txtTranscription.Text = $"Wystąpił nieoczekiwany błąd: {ex.Message}";
        }
        finally
        {
            // Włącz przyciski z powrotem
            btnSelectFile.Enabled = true;
            btnOptions.Enabled = true;
            
            // Nie włączamy btnTranscribe - jeśli transkrypcja się powiodła, przycisk powinien pozostać wyłączony
            // btnTranscribe jest włączany tylko gdy użytkownik wybierze nowy plik
        }
    }

    private void btnRecord_Click(object sender, EventArgs e)
    {
        try
        {
            // Otwórz formularz nagrywania bez wyświetlania komunikatu informacyjnego
            RecorderForm recorderForm = new RecorderForm(this);
            recorderForm.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd podczas otwierania okna nagrywania: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void SetSelectedFile(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            selectedFilePath = filePath;
            lblStatus.Text = $"Status: Wybrano plik {Path.GetFileName(selectedFilePath)}";
            
            // Zresetuj flagę transkrypcji, ponieważ wybrano nowy plik
            currentFileTranscribed = false;
            
            // Włącz przycisk transkrypcji
            btnTranscribe.Enabled = true;
        }
    }

    /// <summary>
    /// Obsługuje zdarzenie kliknięcia przycisku czyszczenia transkrypcji.
    /// Czyści pole tekstowe transkrypcji i wyłącza przycisk czyszczenia.
    /// </summary>
    private void btnClearTranscription_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtTranscription.Text))
        {
            MessageBox.Show("Nie ma tekstu do wyczyszczenia.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        DialogResult result = MessageBox.Show(
            "Czy na pewno chcesz wyczyścić transkrypcję?", 
            "Potwierdzenie", 
            MessageBoxButtons.YesNo, 
            MessageBoxIcon.Question);
            
        if (result == DialogResult.Yes)
        {
            transcriptionText = string.Empty;
            txtTranscription.Text = string.Empty;
            btnClearTranscription.Enabled = false;
            lblStatus.Text = "Status: Transkrypcja wyczyszczona";
        }
    }

    /// <summary>
    /// Aktualizuje zmienną transcriptionText przy każdej zmianie tekstu w polu txtTranscription.
    /// Włącza przyciski zapisu i czyszczenia, jeśli pole zawiera tekst.
    /// </summary>
    private void txtTranscription_TextChanged(object sender, EventArgs e)
    {
        bool hasText = !string.IsNullOrEmpty(txtTranscription.Text) && 
                       txtTranscription.Text != "Trwa transkrypcja...";
        
        btnClearTranscription.Enabled = hasText;
    }

    /// <summary>
    /// Otwiera folder z transkrypcjami w Eksploratorze Windows.
    /// </summary>
    private void btnOpenTranscriptionsFolder_Click(object sender, EventArgs e)
    {
        try
        {
            // Określ ścieżkę do folderu z transkrypcjami
            string transcriptionsFolder;
            
            // Jeśli istnieje ostatnio używany plik transkrypcji, użyj jego folderu
            if (!string.IsNullOrEmpty(lastTranscriptionFilePath) && File.Exists(lastTranscriptionFilePath))
            {
                transcriptionsFolder = Path.GetDirectoryName(lastTranscriptionFilePath) ?? string.Empty;
            }
            // W przeciwnym razie użyj domyślnego folderu transkrypcji
            else
            {
                // Sprawdź ścieżkę względną w folderze aplikacji
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                string relativeFolder = Path.Combine(appFolder, "transcriptions");
                
                // Sprawdź ścieżkę w katalogu głównym projektu
                string projectRootFolder = Path.GetFullPath(Path.Combine(appFolder, "..", ".."));
                string rootFolder = Path.Combine(projectRootFolder, "transcriptions");
                
                // Użyj folderu, który istnieje, lub utwórz domyślny
                if (Directory.Exists(relativeFolder))
                {
                    transcriptionsFolder = relativeFolder;
                }
                else if (Directory.Exists(rootFolder))
                {
                    transcriptionsFolder = rootFolder;
                }
                else
                {
                    // Utwórz folder, jeśli nie istnieje
                    transcriptionsFolder = relativeFolder;
                    Directory.CreateDirectory(transcriptionsFolder);
                }
            }
            
            // Otwórz folder w Eksploratorze Windows
            if (!string.IsNullOrEmpty(transcriptionsFolder) && Directory.Exists(transcriptionsFolder))
            {
                Process.Start("explorer.exe", transcriptionsFolder);
            }
            else
            {
                MessageBox.Show("Nie można znaleźć folderu z transkrypcjami.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd podczas otwierania folderu: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveTranscriptionAutomatically(string transcriptionText, string suggestedFilePath)
    {
        try
        {
            if (string.IsNullOrEmpty(transcriptionText))
            {
                return;
            }

            string fileName = Path.GetFileNameWithoutExtension(suggestedFilePath);
            string directory = Path.GetDirectoryName(suggestedFilePath) ?? "transcriptions";
            
            // Upewnij się, że katalog istnieje
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // Zapisz transkrypcję automatycznie
            File.WriteAllText(suggestedFilePath, transcriptionText);
            
            // Aktualizuj ostatnią zapisaną ścieżkę
            lastTranscriptionFilePath = suggestedFilePath;
            
            // Aktualizuj status
            lblStatus.Text = $"Status: Transkrypcja zakończona i automatycznie zapisana do {Path.GetFileName(suggestedFilePath)}. Czas: {stopwatch.Elapsed.TotalSeconds:F1}s";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd podczas automatycznego zapisywania transkrypcji: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateStatusLabel(int progress, string? statusInfo)
    {
        try
        {
            // Usuwamy Application.DoEvents() - może powodować problemy
            if (progress >= 0)
            {
                // Format: Status: Transkrypcja w toku... XX% (Pozostało: YY min ZZ sek)
                if (string.IsNullOrEmpty(statusInfo))
                {
                    lblStatus.Text = $"Status: Transkrypcja w toku... {progress}%";
                }
                else
                {
                    // Wyróżnij informację o czasie
                    lblStatus.Text = $"Status: Transkrypcja w toku... {progress}% {statusInfo}";
                }
            }
            else if (progress == -1) // Specjalny kod dla informacji
            {
                // Informacje diagnostyczne i dotyczące przewidywanego czasu
                if (!string.IsNullOrEmpty(statusInfo))
                {
                    // Wyświetl informacje statusowe
                    lblStatus.Text = $"Status: {statusInfo}";
                }
            }
        }
        catch (Exception ex)
        {
            // Loguj błąd, ale nie przerywaj działania
            Debug.WriteLine($"Błąd w UpdateStatusLabel: {ex.Message}");
        }
    }

    private string FormatElapsedTime(TimeSpan elapsed)
    {
        return $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
    }

    // Dodajemy metodę obsługi przycisku Opcje
    private void btnOptions_Click(object sender, EventArgs e)
    {
        try
        {
            // Pobierz bieżące ustawienia
            var appSettings = AppSettings.Load();
            
            // Utwórz formularz opcji z bieżącymi ustawieniami
            using (var optionsForm = new OptionsForm(
                this,
                appSettings.LanguageCode,
                appSettings.ModelSize,
                appSettings.HighQuality,
                appSettings.AddPunctuation))
            {
                if (optionsForm.ShowDialog() == DialogResult.OK)
                {
                    // Sprawdź, czy trzeba zainicjalizować transkrybera ponownie
                    bool reinitializeTranscriber =
                        appSettings.LanguageCode != optionsForm.GetSelectedLanguageCode() ||
                        appSettings.ModelSize != optionsForm.GetSelectedModelSize();
                    
                    appSettings.LanguageCode = optionsForm.GetSelectedLanguageCode();
                    appSettings.ModelSize = optionsForm.GetSelectedModelSize();
                    appSettings.HighQuality = optionsForm.IsHighQualityEnabled();
                    appSettings.AddPunctuation = optionsForm.IsAddPunctuationEnabled();
                    
                    // Zapisz ustawienia
                    appSettings.Save();
                    
                    // Jeśli zmieniono model lub język, zainicjuj transkrybera ponownie
                    if (reinitializeTranscriber)
                    {
                        InitializeTranscriber();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd podczas otwierania opcji: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Metoda aktualizująca opcje, wywoływana z formularza OptionsForm
    public void UpdateOptions(string languageCode, string modelSize, bool highQuality, bool addPunctuation)
    {
        // Aktualizuj lokalne zmienne
        selectedLanguageCode = languageCode;
        selectedModelSize = modelSize;
        highQualityTranscription = highQuality;
        addPunctuation = addPunctuation;
        
        // Aktualizuj ustawienia aplikacji
        appSettings.LanguageCode = languageCode;
        appSettings.ModelSize = modelSize;
        appSettings.HighQuality = highQuality;
        appSettings.AddPunctuation = addPunctuation;
        
        // Zapisz ustawienia
        appSettings.Save();
        
        // Zainicjalizuj transkrybera ponownie
        InitializeTranscriber();
    }
}


