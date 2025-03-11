using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TranscriberApp
{
    public partial class OptionsForm : Form
    {
        // Główny formularz, który będzie odbierał ustawienia
        private Form1 mainForm;
        
        // Konstruktor
        public OptionsForm(Form1 form, string selectedLanguage, string selectedModelSize, 
                          bool highQuality, bool addPunctuation)
        {
            InitializeComponent();
            mainForm = form;
            
            // Ustawienie początkowych wartości
            InitializeLanguageOptions();
            InitializeModelSizeOptions();
            
            // Ustawienie wartości przekazanych z głównego formularza
            SetSelectedLanguage(selectedLanguage);
            SetSelectedModelSize(selectedModelSize);
            chkHighQualityTranscription.Checked = highQuality;
            chkAddPunctuation.Checked = addPunctuation;
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
        }

        private void InitializeModelSizeOptions()
        {
            // Dodaj dostępne rozmiary modeli
            cboModelSize.Items.Add("Tiny (najszybszy, najmniej dokładny)");
            cboModelSize.Items.Add("Base (szybki, podstawowa dokładność)");
            cboModelSize.Items.Add("Small (zbalansowany)");
            cboModelSize.Items.Add("Medium (dokładny, wolniejszy)");
            cboModelSize.Items.Add("Large (najdokładniejszy, najwolniejszy)");
        }
        
        private void SetSelectedLanguage(string languageCode)
        {
            int index;
            switch (languageCode.ToLower())
            {
                case "pl": index = 0; break;
                case "en": index = 1; break;
                case "de": index = 2; break;
                case "fr": index = 3; break;
                case "es": index = 4; break;
                case "it": index = 5; break;
                case "ru": index = 6; break;
                case "uk": index = 7; break;
                case "cs": index = 8; break;
                default: index = 0; break;
            }
            
            if (index < cboLanguage.Items.Count)
            {
                cboLanguage.SelectedIndex = index;
            }
        }
        
        private void SetSelectedModelSize(string modelSize)
        {
            int index;
            switch (modelSize.ToLower())
            {
                case "tiny": index = 0; break;
                case "base": index = 1; break;
                case "small": index = 2; break;
                case "medium": index = 3; break;
                case "large": index = 4; break;
                default: index = 3; break;
            }
            
            if (index < cboModelSize.Items.Count)
            {
                cboModelSize.SelectedIndex = index;
            }
        }
        
        // Metody do pobierania ustawień
        public string GetSelectedLanguageCode()
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
        
        public string GetSelectedModelSize()
        {
            switch (cboModelSize.SelectedIndex)
            {
                case 0: return "tiny";
                case 1: return "base";
                case 2: return "small";
                case 3: return "medium";
                case 4: return "large";
                default: return "medium";
            }
        }
        
        public bool IsHighQualityEnabled()
        {
            return chkHighQualityTranscription.Checked;
        }
        
        public bool IsAddPunctuationEnabled()
        {
            return chkAddPunctuation.Checked;
        }
        
        // Obsługa przycisku OK
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Przekazanie ustawień do głównego formularza
            mainForm.UpdateOptions(
                GetSelectedLanguageCode(),
                GetSelectedModelSize(),
                IsHighQualityEnabled(),
                IsAddPunctuationEnabled()
            );
            
            // Zamknij formularz
            DialogResult = DialogResult.OK;
            Close();
        }
        
        // Obsługa przycisku Anuluj
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        
        // Obsługa przycisku Przywróć ustawienia fabryczne
        private void btnRestoreDefaults_Click(object sender, EventArgs e)
        {
            // Wyświetl potwierdzenie
            DialogResult result = MessageBox.Show(
                "Czy na pewno chcesz przywrócić ustawienia fabryczne?",
                "Potwierdzenie",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            
            if (result != DialogResult.Yes)
            {
                return;
            }
            
            // Ustaw domyślne wartości
            SetSelectedLanguage(AppSettings.DefaultLanguageCode);
            SetSelectedModelSize(AppSettings.DefaultModelSize);
            chkHighQualityTranscription.Checked = AppSettings.DefaultHighQuality;
            chkAddPunctuation.Checked = AppSettings.DefaultAddPunctuation;
            
            MessageBox.Show(
                "Przywrócono ustawienia fabryczne. Kliknij OK, aby je zapisać.",
                "Informacja",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
} 