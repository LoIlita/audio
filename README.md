# Plan pracy nad MVP aplikacji do transkrypcji nagrań z Discorda

Aby stworzyć minimalną wersję aplikacji (MVP), podzielimy pracę na etapy, zaczynając od podstawowej funkcjonalności, a potem stopniowo ją rozbudowując.

## 📋 Status projektu

🔸 Etap 1: Przygotowanie środowiska ✅
✅ Zainstalowane narzędzia:

- .NET 8
- Python
- Whisper AI (pip install openai-whisper)
- Audacity (opcjonalnie, do nagrywania)

✅ Utworzony projekt

- Nazwa projektu: TranscriberApp
- Typ projektu: Windows Forms App (.NET8)

🔸 Etap 2: Implementacja interfejsu użytkownika (WinForms) ✅
✅ Zaprojektowany prosty interfejs:

- Przycisk „Wybierz plik" – otwiera okno wyboru pliku .wav lub .mp3.
- Przycisk „Transkrybuj" – uruchamia transkrypcję.
- Etykieta „Status" – pokazuje komunikaty np. „Transkrypcja w toku…".

✅ Zgodnie z zasadami SOLID zaimplementowano:

- Interfejs `ITranscriber` - abstrakcja transkrypcji
- Klasę `WhisperTranscriber` - implementacja transkrypcji (symulowana w etapie 2)
- Formularz `Form1` - obsługa interfejsu użytkownika

✅ Utworzona struktura katalogów:

- `/PythonScripts` - skrypty Pythona
- `/audio` - pliki dźwiękowe
- `/transcriptions` - zapisane transkrypcje

📌 Po zakończeniu tego etapu: Można ręcznie wybierać plik audio, a transkrypcja jest symulowana.

🔸 Etap 3: Połączenie z Whisper AI ✅
✅ Rozwinięcie klasy `WhisperTranscriber` do wywoływania skryptu Pythona z parametrami:

- Uruchamianie procesu Python z odpowiednimi argumentami
- Odbiór i przetwarzanie wyników z wykorzystaniem JSON
- Obsługa błędów i walidacja plików

✅ Rozbudowa skryptu Python `whisper_transcribe.py`:

- Dodanie rzeczywistej transkrypcji z użyciem biblioteki Whisper
- Raportowanie postępu i statusu do aplikacji C#
- Generowanie plików Markdown z transkrypcją i znacznikami czasu

✅ Implementacja sprawdzania wymagań systemowych:

- Weryfikacja czy Python i Whisper są zainstalowane
- Sprawdzanie dostępności przyspieszenia GPU (CUDA)
- Informowanie użytkownika o brakujących zależnościach

📌 Po zakończeniu tego etapu: Użytkownik może wybrać plik audio, a aplikacja przetwarza go i zapisuje rzeczywistą transkrypcję z użyciem Whisper AI.

🔸 Etap 4: Integracja z Audacity ✅
✅ Dodanie uproszczonej integracji z Audacity:

- Interfejs `IAudacityController` - abstrakcja sterowania Audacity
- Klasa `AudacityController` - implementacja uruchamiania Audacity
- Przycisk do uruchamiania Audacity z poziomu aplikacji

✅ Uproszczenie interfejsu użytkownika:

- Przycisk do uruchamiania Audacity
- Wskaźnik stanu Audacity (uruchomione/nie uruchomione)
- Ręczne nagrywanie i zapisywanie plików w Audacity

📌 Po zakończeniu tego etapu: Aplikacja umożliwia uruchomienie Audacity do nagrywania dźwięku, a następnie transkrypcję zapisanych plików.

🔸 Etap 5: Poprawki i testy ⏳ (Planowane)

- Dodanie obsługi błędów, np. brak pliku audio.
- Testowanie aplikacji na różnych plikach audio.
- Opcjonalnie: Dodanie ustawień modelu Whisper (tiny, base, medium).

📌 Po zakończeniu tego etapu: Będziemy mieć działające MVP – aplikację, która umożliwia nagrywanie dźwięku i transkrybowanie rozmów.

## 🔹 Aktualna struktura projektu

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
 ├── TranscriberApp.csproj.user        # Ustawienia użytkownika
📂 audio                               # Folder na nagrania audio
 ├── README.txt                        # Instrukcje dotyczące plików audio
📂 transcriptions                      # Folder na pliki transkrypcji
 ├── README.txt                        # Instrukcje dotyczące transkrypcji
README.md                              # Ten plik
audio.sln                              # Plik rozwiązania Visual Studio
```

## 🚩 Następne kroki

1. **Testowanie aplikacji**:

   - Sprawdzenie uruchamiania Audacity
   - Testowanie transkrypcji różnych plików audio
   - Weryfikacja jakości transkrypcji

2. **Poprawki i ulepszenia**:

   - Obsługa błędów
   - Lepsze komunikaty dla użytkownika
   - Optymalizacja wydajności

3. **Dokumentacja**:
   - Aktualizacja instrukcji użytkownika
   - Opis funkcji
   - Wymagania systemowe

## 🔹 Kolejne kroki po MVP

Po zakończeniu MVP planujemy dodać:

- Obsługę wielu plików naraz (batch transkrypcja)
- Lepsze formatowanie Markdown (np. podział na rozmówców)
- Automatyczne otwieranie transkrypcji w Obsidian
- Możliwość edycji transkrypcji bezpośrednio w aplikacji

## 📝 Jak używać aplikacji

1. **Wybór pliku audio**:

   - Kliknij przycisk "Wybierz plik"
   - Wybierz plik .wav lub .mp3 z dialogu
   - Nazwa wybranego pliku pojawi się w statusie

2. **Nagrywanie z Audacity**:

   - Kliknij "Uruchom Audacity" (jeśli nie jest uruchomione)
   - Ręcznie nagraj dźwięk w Audacity
   - Zapisz plik w formacie WAV lub MP3
   - Wybierz zapisany plik w aplikacji do transkrypcji

3. **Transkrypcja**:

   - Kliknij przycisk "Transkrybuj"
   - Postęp będzie widoczny w statusie
   - Po zakończeniu transkrypcji możesz otworzyć plik .md z wynikiem

4. **Wymagania systemowe**:
   - Python 3.8 lub nowszy
   - Whisper AI (instalacja: `pip install openai-whisper`)
   - Audacity (do nagrywania)
   - Opcjonalnie: GPU z obsługą CUDA dla szybszej transkrypcji
