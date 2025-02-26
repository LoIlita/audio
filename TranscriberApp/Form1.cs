using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

namespace TranscriberApp;

public partial class Form1 : Form
{
    private string selectedFilePath = string.Empty;
    private ITranscriber? transcriber;
    private IExternalAppController externalAppController;
    private string transcriptionText = string.Empty;
    private string transcriptionFilePath = string.Empty;
    private string lastTranscriptionFilePath = string.Empty;
    private Stopwatch stopwatch = new Stopwatch();

    public Form1()
    {
        InitializeComponent();
        InitializeTranscriber();
        InitializeLanguageOptions();
        externalAppController = new ObsidianController();
    }

    private void InitializeTranscriber()
    {
        try
        {
            transcriber = new WhisperTranscriber();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd inicjalizacji transkrybera: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void InitializeLanguageOptions()
    {
        // Dodaj dostępne języki
        cboLanguage.Items.Add("Polski");
        cboLanguage.Items.Add("Angielski");
        cboLanguage.Items.Add("Niemiecki");
        cboLanguage.Items.Add("Francuski");
        cboLanguage.Items.Add("Hiszpański");
        cboLanguage.Items.Add("Włoski");
        cboLanguage.Items.Add("Rosyjski");
        cboLanguage.Items.Add("Ukraiński");
        cboLanguage.Items.Add("Czeski");
        cboLanguage.SelectedIndex = 0; // Domyślnie polski
    }

    private void btnSelectFile_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Pliki audio (*.wav;*.mp3)|*.wav;*.mp3|Wszystkie pliki (*.*)|*.*";
            openFileDialog.Title = "Wybierz plik audio do transkrypcji";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
                lblStatus.Text = $"Status: Wybrano plik {Path.GetFileName(selectedFilePath)}";
                btnTranscribe.Enabled = true;
                ResetProgressBar();
            }
        }
    }

    private async void btnTranscribe_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(selectedFilePath))
        {
            MessageBox.Show("Proszę najpierw wybrać plik audio.", "Brak pliku", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (transcriber == null)
        {
            MessageBox.Show("Transkryber nie został poprawnie zainicjalizowany.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            btnTranscribe.Enabled = false;
            btnSelectFile.Enabled = false;
            txtTranscription.Text = "Trwa transkrypcja...";
            lblStatus.Text = "Status: Transkrypcja w toku...";
            
            // Przygotuj pasek postępu
            progressBarTranscription.Value = 0;
            progressBarTranscription.Visible = true;

            // Przygotuj opcje transkrypcji
            Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "language", GetLanguageCode() },
                { "add_punctuation", chkAddPunctuation.Checked },
                { "high_quality", chkHighQualityTranscription.Checked }
            };

            // Funkcja zwrotna do raportowania postępu
            Action<int> progressCallback = (progress) =>
            {
                // Używamy Invoke, aby aktualizować UI z innego wątku
                if (lblStatus.InvokeRequired)
                {
                    lblStatus.Invoke(new Action(() => 
                    {
                        lblStatus.Text = $"Status: Transkrypcja w toku... {progress}%";
                        progressBarTranscription.Value = progress;
                    }));
                }
                else
                {
                    lblStatus.Text = $"Status: Transkrypcja w toku... {progress}%";
                    progressBarTranscription.Value = progress;
                }
            };

            // Uruchom stoper
            stopwatch.Restart();
            
            // Wykonaj transkrypcję
            transcriptionFilePath = await transcriber.TranscribeAsync(selectedFilePath, progressCallback);
            
            // Zatrzymaj stoper
            stopwatch.Stop();
            
            if (!string.IsNullOrEmpty(transcriptionFilePath) && File.Exists(transcriptionFilePath))
            {
                // Wczytaj transkrypcję
                transcriptionText = File.ReadAllText(transcriptionFilePath);
                txtTranscription.Text = transcriptionText;
                btnSaveTranscription.Enabled = true;
                btnClearTranscription.Enabled = true;
                lblStatus.Text = $"Status: Transkrypcja zakończona. Czas: {stopwatch.Elapsed.TotalSeconds:F1}s";
                progressBarTranscription.Value = 100;
            }
            else
            {
                txtTranscription.Text = "Błąd podczas transkrypcji.";
                lblStatus.Text = "Status: Błąd transkrypcji";
                progressBarTranscription.Visible = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd podczas transkrypcji: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = $"Status: Błąd - {ex.Message}";
            txtTranscription.Text = $"Wystąpił błąd: {ex.Message}";
            progressBarTranscription.Visible = false;
        }
        finally
        {
            btnTranscribe.Enabled = true;
            btnSelectFile.Enabled = true;
        }
    }

    private string GetLanguageCode()
    {
        switch (cboLanguage.SelectedIndex)
        {
            case 0: return "pl";
            case 1: return "en";
            case 2: return "de";
            case 3: return "fr";
            case 4: return "es";
            case 5: return "it";
            case 6: return "ru";
            case 7: return "uk";
            case 8: return "cs";
            default: return "pl";
        }
    }

    private async void btnStartObsidian_Click(object sender, EventArgs e)
    {
        try
        {
            // Wyłączamy przycisk na czas uruchamiania, aby zapobiec wielokrotnym kliknięciom
            btnStartObsidian.Enabled = false;
            lblRecordingStatus.Text = "Status: Uruchamianie Obsidian...";
            
            // Uruchamiamy asynchronicznie
            bool success = await externalAppController.StartAppAsync();
            
            if (success)
            {
                lblRecordingStatus.Text = "Status: Obsidian uruchomiony";
            }
            else
            {
                lblRecordingStatus.Text = "Status: Nie udało się uruchomić Obsidian";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd podczas uruchamiania Obsidian: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblRecordingStatus.Text = $"Status: Błąd - {ex.Message}";
        }
        finally
        {
            // Zawsze włączamy przycisk z powrotem
            btnStartObsidian.Enabled = true;
        }
    }

    private void btnRecord_Click(object sender, EventArgs e)
    {
        try
        {
            // Wyświetl informację o możliwościach nagrywania
            MessageBox.Show(
                "Możesz nagrywać dźwięk z mikrofonu lub dźwięk systemowy (np. z Discorda, przeglądarki).\n\n" +
                "Aby nagrać dźwięk z Discorda, wybierz opcję 'Dźwięk systemowy' z listy źródeł dźwięku.",
                "Informacja o nagrywaniu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            
            // Otwórz formularz nagrywania
            RecorderForm recorderForm = new RecorderForm(this);
            recorderForm.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd podczas otwierania okna nagrywania: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Otwiera aktualną transkrypcję w Obsidianie
    /// </summary>
    private void OpenTranscriptionInObsidian()
    {
        try
        {
            // Najpierw upewnij się, że Obsidian jest uruchomiony
            if (!externalAppController.StartApp())
            {
                MessageBox.Show("Nie udało się uruchomić Obsidian. Sprawdź czy jest zainstalowany.", 
                    "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Jeśli mamy zapisaną transkrypcję, otwórz ją w Obsidianie
            string fileToOpen = string.Empty;
            
            if (!string.IsNullOrEmpty(lastTranscriptionFilePath) && File.Exists(lastTranscriptionFilePath))
            {
                fileToOpen = lastTranscriptionFilePath;
            }
            else if (!string.IsNullOrEmpty(transcriptionFilePath) && File.Exists(transcriptionFilePath))
            {
                fileToOpen = transcriptionFilePath;
            }
            
            if (!string.IsNullOrEmpty(fileToOpen))
            {
                // Otwórz plik w Obsidianie za pomocą protokołu obsidian://
                string obsidianUrl = $"obsidian://open?path={Uri.EscapeDataString(fileToOpen)}";
                Process.Start(new ProcessStartInfo
                {
                    FileName = obsidianUrl,
                    UseShellExecute = true
                });
                
                lblRecordingStatus.Text = $"Status: Otwarto transkrypcję w Obsidianie";
            }
            else
            {
                MessageBox.Show("Brak transkrypcji do otwarcia. Najpierw wykonaj transkrypcję i zapisz ją.", 
                    "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd podczas otwierania transkrypcji w Obsidianie: {ex.Message}", 
                "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblRecordingStatus.Text = $"Status: Błąd - {ex.Message}";
        }
    }

    public void SetSelectedFile(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            selectedFilePath = filePath;
            lblStatus.Text = $"Status: Wybrano plik {Path.GetFileName(selectedFilePath)}";
            btnTranscribe.Enabled = true;
            ResetProgressBar();
        }
    }

    private void btnSaveTranscription_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtTranscription.Text) && string.IsNullOrEmpty(transcriptionText))
        {
            MessageBox.Show("Brak transkrypcji do zapisania.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
        {
            saveFileDialog.Filter = "Pliki Markdown (*.md)|*.md|Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
            saveFileDialog.Title = "Zapisz transkrypcję";
            saveFileDialog.DefaultExt = "md";
            
            if (!string.IsNullOrEmpty(transcriptionFilePath))
            {
                saveFileDialog.FileName = Path.GetFileName(transcriptionFilePath);
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(transcriptionFilePath);
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Zapisz transkrypcję - zawsze używamy aktualnej zawartości kontrolki
                    string textToSave = txtTranscription.Text;
                    File.WriteAllText(saveFileDialog.FileName, textToSave);
                    
                    // Aktualizujemy zmienną transcriptionText, aby odzwierciedlała zapisaną zawartość
                    transcriptionText = textToSave;
                    lastTranscriptionFilePath = saveFileDialog.FileName;
                    lblStatus.Text = $"Status: Transkrypcja zapisana do {Path.GetFileName(saveFileDialog.FileName)}";
                    btnSaveTranscription.Enabled = true;
                    btnClearTranscription.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas zapisywania transkrypcji: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
            "Czy na pewno chcesz wyczyścić tekst transkrypcji?", 
            "Potwierdzenie", 
            MessageBoxButtons.YesNo, 
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            txtTranscription.Clear();
            transcriptionText = string.Empty;
            btnClearTranscription.Enabled = false;
            btnSaveTranscription.Enabled = false;
            lblStatus.Text = "Status: Transkrypcja została wyczyszczona.";
            ResetProgressBar();
        }
    }

    /// <summary>
    /// Resetuje pasek postępu do stanu początkowego (ukryty, wartość 0).
    /// </summary>
    private void ResetProgressBar()
    {
        progressBarTranscription.Value = 0;
        progressBarTranscription.Visible = false;
    }

    private void progressBarTranscription_Click(object sender, EventArgs e)
    {
        // Pusty handler dla zdarzenia kliknięcia paska postępu
    }

    private void UpdateProgress(int percentComplete)
    {
        progressBarTranscription.Value = percentComplete;
        progressBarTranscription.Visible = true;
    }
    
    /// <summary>
    /// Aktualizuje zmienną transcriptionText przy każdej zmianie tekstu w polu txtTranscription.
    /// Włącza przyciski zapisu i czyszczenia, jeśli pole zawiera tekst.
    /// </summary>
    private void txtTranscription_TextChanged(object sender, EventArgs e)
    {
        // Aktualizujemy zmienną przy każdej zmianie tekstu
        transcriptionText = txtTranscription.Text;
        
        // Włączamy/wyłączamy przyciski w zależności od tego, czy jest tekst
        bool hasText = !string.IsNullOrEmpty(transcriptionText);
        btnSaveTranscription.Enabled = hasText;
        btnClearTranscription.Enabled = hasText;
    }
}


