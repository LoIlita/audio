# Aplikacja do Transkrypcji Audio

Aplikacja desktopowa do transkrypcji plików audio z wykorzystaniem modelu Whisper AI oraz integracji z Audacity.

## 📋 Funkcje

- Transkrypcja plików audio (WAV, MP3) do plików tekstowych w formacie Markdown
- Integracja z Audacity do nagrywania dźwięku
- Wsparcie dla przyspieszenia GPU (CUDA) dla szybszej transkrypcji
- Generowanie transkrypcji ze znacznikami czasu
- Sprawdzanie wymagań systemowych

## 🔧 Wymagania

- .NET 8.0 lub nowszy
- Python 3.8 lub nowszy
- Whisper AI (`pip install openai-whisper`)
- Audacity (opcjonalnie, do nagrywania)
- Opcjonalnie: GPU z obsługą CUDA dla szybszej transkrypcji

## 🚀 Instalacja

1. Sklonuj repozytorium:

   ```
   git clone https://github.com/twój-użytkownik/aplikacja-transkrypcja.git
   ```

2. Zainstaluj wymagane pakiety Python:

   ```
   pip install openai-whisper
   ```

3. Otwórz projekt w Visual Studio lub zbuduj z linii poleceń:

   ```
   dotnet build
   ```

4. Uruchom aplikację:
   ```
   dotnet run --project TranscriberApp
   ```

## 📝 Jak używać

1. **Wybór pliku audio**:

   - Kliknij przycisk "Wybierz plik"
   - Wskaż plik audio (WAV lub MP3)
   - Kliknij "Transkrybuj"

3. **Aby nagrać nowy dźwięk**:

   - Kliknij "Uruchom Audacity" (jeśli nie jest uruchomione)
   - Ręcznie nagraj dźwięk w Audacity
   - Zapisz plik w formacie WAV lub MP3
   - Wybierz zapisany plik w aplikacji do transkrypcji

3. **Transkrypcja**:
   - Kliknij przycisk "Transkrybuj"
   - Postęp będzie widoczny w statusie
   - Po zakończeniu transkrypcji możesz otworzyć plik .md z wynikiem

## 🔍 Struktura projektu

```
📂 TranscriberApp
 ├── 📂 PythonScripts                  # Skrypty do transkrypcji
 │   ├── whisper_transcribe.py         # Skrypt Python do Whisper AI
 ├── Form1.cs                          # Główny formularz aplikacji
 ├── Form1.Designer.cs                 # Kod projektanta interfejsu
 ├── ITranscriber.cs                   # Interfejs dla mechanizmu transkrypcji
 ├── WhisperTranscriber.cs             # Implementacja transkrypcji
 ├── IAudacityController.cs            # Interfejs dla sterowania Audacity
 ├── AudacityController.cs             # Implementacja uruchamiania Audacity
 ├── SystemRequirements.cs             # Sprawdzanie wymagań systemowych
 ├── Program.cs                        # Główna klasa aplikacji
 ├── TranscriberApp.csproj             # Plik projektu
📂 audio                               # Folder na nagrania audio
📂 transcriptions                      # Folder na pliki transkrypcji
```

## 📜 Licencja

Ten projekt jest udostępniany na licencji MIT. Szczegóły znajdziesz w pliku LICENSE.
