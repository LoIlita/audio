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


## 📝 Jak używać

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



## 📜 Licencja

Ten projekt jest udostępniany na licencji MIT. Szczegóły znajdziesz w pliku LICENSE.

## 🤝 Wkład

Zachęcamy do zgłaszania problemów i propozycji ulepszeń poprzez Issues oraz Pull Requests.
