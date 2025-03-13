# Aplikacja do Transkrypcji Audio - Wersja 1.0.2

Prosta aplikacja do zamiany nagrań audio na tekst przy użyciu technologii Whisper AI.

## Co nowego w wersji 1.0.2

- **Usunięto limit czasowy transkrypcji** - aplikacja nie przerywa już procesu po 30 minutach
- **Wsparcie dla dłuższych nagrań** - możesz transkrybować nagrania o dowolnej długości
- Ulepszenia stabilności procesu transkrypcji

## Funkcje aplikacji

- **Transkrypcja plików audio** - zamień pliki WAV i MP3 na tekst w formacie Markdown
- **Nagrywanie dźwięku** - nagrywaj dźwięk bezpośrednio z aplikacji (dźwięk systemowy lub mikrofon)
- **Integracja z Audacity** - możliwość zaawansowanej edycji audio
- **Wsparcie dla GPU** - znacznie szybsza transkrypcja na komputerach z kartą NVIDIA

## Jak korzystać z aplikacji

1. **Uruchomienie**: Pobierz najnowszą wersję z katalogu Releases i kliknij dwukrotnie na plik `TranscriberApp.exe`

2. **Aby transkrybować istniejący plik**:

   - Kliknij przycisk "Wybierz plik"
   - Wskaż plik audio (WAV lub MP3)
   - Kliknij "Transkrybuj"

3. **Aby nagrać nowy dźwięk**:

   - Kliknij przycisk "Nagraj audio"
   - Wybierz lokalizację do zapisania nagrania
   - Kliknij "Rozpocznij nagrywanie"
   - Po zakończeniu kliknij "Zatrzymaj nagrywanie"
   - Aplikacja automatycznie wróci do głównego panelu z wybranym plikiem

4. **Transkrypcja**:
   - Po wybraniu pliku kliknij "Transkrybuj"
   - Postęp transkrypcji będzie widoczny na pasku
   - Po zakończeniu możesz otworzyć i zapisać plik z transkrypcją

## Wymagania

- Python 3.8+ (dla modułu transkrypcji)
- Model Whisper AI (automatycznie pobierany przy pierwszym użyciu)

## Licencja

Ten projekt jest udostępniany na licencji MIT. Szczegóły znajdziesz w pliku LICENSE.
