#!/usr/bin/env python3
# -*- coding: utf-8 -*-

"""
Skrypt do transkrypcji nagrań audio za pomocą Whisper AI.
Przyjmuje ścieżkę do pliku audio i zwraca transkrypcję.
"""

import os
import sys
import json
import argparse
import time
from datetime import datetime
import wave
import contextlib
import math

try:
    import whisper
    import torch
    WHISPER_AVAILABLE = True
except ImportError:
    WHISPER_AVAILABLE = False

def setup_args():
    """Konfiguracja parsera argumentów wiersza poleceń."""
    parser = argparse.ArgumentParser(description='Transkrypcja audio za pomocą Whisper AI')
    parser.add_argument('audio_path', type=str, help='Ścieżka do pliku audio')
    parser.add_argument('--output', '-o', type=str, help='Ścieżka do pliku wyjściowego')
    parser.add_argument('--model', type=str, default='small', help='Model Whisper (tiny, base, small, medium, large)')
    parser.add_argument('--language', type=str, default='pl', help='Język transkrypcji (domyślnie polski)')
    parser.add_argument('--punctuation', action='store_true', help='Dodaj automatycznie interpunkcję')
    parser.add_argument('--best_of', type=int, default=1, help='Parametr best_of dla Whisper')
    parser.add_argument('--beam_size', type=int, default=1, help='Parametr beam_size dla Whisper')
    return parser.parse_args()

def check_dependencies():
    """Sprawdza, czy wymagane zależności są zainstalowane."""
    missing_deps = []
    
    try:
        import whisper
    except ImportError:
        missing_deps.append("whisper")
    
    try:
        import torch
    except ImportError:
        missing_deps.append("torch")
    
    return missing_deps

def get_audio_duration(audio_path):
    """Pobiera długość pliku audio w sekundach."""
    # Dla plików WAV
    if audio_path.lower().endswith('.wav'):
        try:
            with contextlib.closing(wave.open(audio_path, 'r')) as f:
                frames = f.getnframes()
                rate = f.getframerate()
                duration = frames / float(rate)
                return duration
        except Exception as e:
            print(json.dumps({"warning": f"Nie można określić długości pliku WAV: {str(e)}"}))
    
    # Próba użycia ffprobe do innych formatów
    try:
        import subprocess
        result = subprocess.run(
            ["ffprobe", "-v", "error", "-show_entries", "format=duration", "-of", "default=noprint_wrappers=1:nokey=1", audio_path],
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True
        )
        return float(result.stdout)
    except Exception as e:
        print(json.dumps({"warning": f"Nie można określić długości pliku audio: {str(e)}"}))
        # Zwróć szacunkową długość (10 minut)
        return 600

def estimate_completion_time(audio_duration, model_size, device):
    """
    Szacuje czas potrzebny do ukończenia transkrypcji w sekundach.
    Bazuje na długości pliku audio, wybranym modelu i urządzeniu (CPU/GPU).
    """
    # Współczynniki prędkości dla różnych modeli i urządzeń
    # (wartości oparte na przybliżonych pomiarach)
    speed_factors = {
        'cuda': {  # dla GPU
            'tiny': 0.05,   # ~20x szybciej niż długość audio
            'base': 0.1,    # ~10x szybciej niż długość audio
            'small': 0.2,   # ~5x szybciej niż długość audio
            'medium': 0.33, # ~3x szybciej niż długość audio
            'large': 0.5    # ~2x szybciej niż długość audio
        },
        'cpu': {   # dla CPU
            'tiny': 0.5,    # ~2x szybciej niż długość audio
            'base': 0.75,   # ~1.3x szybciej niż długość audio
            'small': 1.0,   # ~tyle samo co długość audio
            'medium': 1.5,  # ~1.5x dłużej niż długość audio
            'large': 3.0    # ~3x dłużej niż długość audio
        }
    }
    
    # Wybór odpowiedniego współczynnika
    device_type = 'cuda' if device == 'cuda' else 'cpu'
    factor = speed_factors[device_type].get(model_size, 1.0)
    
    # Obliczenie szacowanego czasu
    estimated_time = audio_duration * factor
    
    # Minimalny czas to 5 sekund
    return max(5, estimated_time)

def format_time_remaining(seconds):
    """Formatuje czas pozostały w formacie czytelnym dla człowieka."""
    if seconds < 60:
        return f"{int(seconds)} sek."
    elif seconds < 3600:
        minutes = int(seconds / 60)
        seconds_remainder = int(seconds % 60)
        return f"{minutes} min. {seconds_remainder} sek."
    else:
        hours = int(seconds / 3600)
        minutes_remainder = int((seconds % 3600) / 60)
        return f"{hours} godz. {minutes_remainder} min."

def transcribe_audio(audio_path, output_path=None, model_size='small', language='pl', add_punctuation=False, best_of=1, beam_size=1):
    """
    Transkrybuje audio za pomocą Whisper AI.
    
    Args:
        audio_path: Ścieżka do pliku audio
        output_path: Ścieżka do pliku wyjściowego (.md)
        model_size: Rozmiar modelu Whisper (tiny, base, small, medium, large)
        language: Język transkrypcji (np. 'pl', 'en', 'auto')
        add_punctuation: Czy dodać automatycznie interpunkcję
        best_of: Parametr best_of dla modelu Whisper
        beam_size: Parametr beam_size dla modelu Whisper
        
    Returns:
        Ścieżka do pliku z transkrypcją
    """
    # Sprawdzenie, czy wszystkie wymagane biblioteki są zainstalowane
    missing_deps = check_dependencies()
    if missing_deps:
        error_message = "Brakujące biblioteki Python: " + ", ".join(missing_deps)
        install_cmd = "pip install " + " ".join(missing_deps)
        full_error = f"{error_message}\nZainstaluj je używając polecenia:\n{install_cmd}"
        print(json.dumps({"error": full_error, "status": "error", "missing_deps": missing_deps}))
        return None
    
    try:
        # Pobranie długości pliku audio
        audio_duration = get_audio_duration(audio_path)
        print(json.dumps({"info": f"Długość audio: {format_time_remaining(audio_duration)}"}))
        sys.stdout.flush()
        
        # Wypisujemy informację o rozpoczęciu
        print(json.dumps({"progress": 0, "status": "loading_model"}))
        sys.stdout.flush()
        
        # Sprawdzenie dostępności GPU
        device = "cuda" if torch.cuda.is_available() else "cpu"
        print(json.dumps({"info": f"Używam urządzenia: {device}"}))
        sys.stdout.flush()
        
        # Szacowanie czasu ukończenia
        estimated_time = estimate_completion_time(audio_duration, model_size, device)
        print(json.dumps({
            "info": f"Szacowany czas transkrypcji: {format_time_remaining(estimated_time)}",
            "estimated_time": estimated_time
        }))
        sys.stdout.flush()
        
        # Zapisz czas rozpoczęcia
        start_time = time.time()
        
        # Ładowanie modelu
        try:
            print(json.dumps({"info": f"Próbuję załadować model: {model_size}"}))
            sys.stdout.flush()
            model = whisper.load_model(model_size, device=device)
            model_load_time = time.time() - start_time
            print(json.dumps({
                "progress": 10, 
                "status": "model_loaded",
                "time_elapsed": model_load_time,
                "time_remaining": estimated_time - model_load_time
            }))
            sys.stdout.flush()
        except Exception as e:
            error_msg = f"Błąd podczas ładowania modelu {model_size}: {str(e)}"
            print(json.dumps({"error": error_msg, "status": "error", "model_error": True}))
            sys.stdout.flush()
            raise Exception(error_msg)
        
        # Opcje transkrypcji
        transcribe_options = {
            "language": language if language != "auto" else None,
            "task": "transcribe"
        }
        
        # Dodaj opcje jakości, jeśli określone
        if best_of > 1:
            transcribe_options["best_of"] = best_of
        if beam_size > 1:
            transcribe_options["beam_size"] = beam_size
            
        # Opcje interpunkcji
        if add_punctuation:
            # W Whisper nie ma bezpośredniej opcji dla interpunkcji, jest ona domyślnie włączona
            # Możemy dodać dodatkową informację w pliku wyjściowym
            print(json.dumps({"info": "Interpunkcja włączona"}))
            sys.stdout.flush()
        
        # Rozpoczęcie transkrypcji
        print(json.dumps({
            "progress": 20, 
            "status": "transcribing",
            "time_elapsed": time.time() - start_time,
            "time_remaining": estimated_time - (time.time() - start_time)
        }))
        sys.stdout.flush()
        
        # Faktyczna transkrypcja
        result = model.transcribe(audio_path, **transcribe_options, verbose=False)
        
        # Okresowe aktualizacje postępu podczas transkrypcji
        elapsed_time = time.time() - start_time
        progress = min(90, int(20 + 70 * (elapsed_time / estimated_time)))
        print(json.dumps({
            "progress": progress, 
            "status": "processing",
            "time_elapsed": elapsed_time,
            "time_remaining": max(0, estimated_time - elapsed_time)
        }))
        sys.stdout.flush()
        
        # Informacja o zakończeniu transkrypcji
        print(json.dumps({
            "progress": 90, 
            "status": "formatting",
            "time_elapsed": time.time() - start_time,
            "time_remaining": max(0, (estimated_time * 0.1))  # Zostało około 10% czasu
        }))
        sys.stdout.flush()
        
        # Jeśli nie podano ścieżki wyjściowej, utworzymy ją
        if output_path is None:
            base_name = os.path.splitext(os.path.basename(audio_path))[0]
            output_dir = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', '..', 'transcriptions')
            os.makedirs(output_dir, exist_ok=True)
            output_path = os.path.join(output_dir, f"{base_name}.md")
        
        # Formatowanie i zapis transkrypcji
        with open(output_path, 'w', encoding='utf-8') as f:
            # Nagłówek
            f.write(f"# Transkrypcja: {os.path.basename(audio_path)}\n\n")
            f.write(f"**Plik:** {audio_path}\n")
            f.write(f"**Model:** {model_size}\n")
            f.write(f"**Język:** {language}\n")
            if add_punctuation:
                f.write(f"**Interpunkcja:** Włączona\n")
            if best_of > 1 or beam_size > 1:
                f.write(f"**Jakość:** Best_of={best_of}, Beam_size={beam_size}\n")
            f.write(f"**Data:** {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n")
            f.write(f"**Czas przetwarzania:** {format_time_remaining(time.time() - start_time)}\n\n")
            
            # Wykryty język
            if result.get("language"):
                f.write(f"**Wykryty język:** {result['language']}\n\n")
            
            # Główna treść transkrypcji
            f.write("## Treść transkrypcji\n\n")
            f.write(result["text"].strip())
            f.write("\n\n")
            
            # Segmenty czasowe (opcjonalnie)
            if "segments" in result:
                f.write("## Segmenty czasowe\n\n")
                for i, segment in enumerate(result["segments"]):
                    start_time = format_timestamp(segment["start"])
                    end_time = format_timestamp(segment["end"])
                    f.write(f"**[{start_time} -> {end_time}]** {segment['text'].strip()}\n\n")
        
        # Obliczenie faktycznego czasu transkrypcji
        total_time = time.time() - start_time
        
        # Informacja o zakończeniu
        print(json.dumps({
            "progress": 100, 
            "status": "completed", 
            "output_path": output_path,
            "total_time": total_time,
            "audio_duration": audio_duration,
            "speed_ratio": audio_duration / total_time if total_time > 0 else 0
        }))
        sys.stdout.flush()
        
        return output_path
        
    except Exception as e:
        # W przypadku błędu, zwracamy informację
        error_info = {"error": str(e), "status": "error"}
        print(json.dumps(error_info))
        return None

def format_timestamp(seconds):
    """Formatuje czas w sekundach do formatu MM:SS."""
    minutes = int(seconds // 60)
    seconds = int(seconds % 60)
    return f"{minutes:02d}:{seconds:02d}"

def main():
    """Główna funkcja skryptu."""
    # Jeśli Whisper nie jest dostępny, wyświetl jasny komunikat błędu
    if not WHISPER_AVAILABLE:
        error_message = (
            "Brakujące biblioteki Python: whisper, torch\n"
            "Zainstaluj je używając polecenia:\n"
            "pip install openai-whisper torch\n\n"
            "Uwaga: Instalacja Whisper może zająć kilka minut."
        )
        print(json.dumps({"error": error_message, "status": "error", "missing_deps": ["whisper", "torch"]}))
        return 1
    
    args = setup_args()
    try:
        output_path = args.output
        transcribe_audio(args.audio_path, output_path, args.model, args.language, args.punctuation, args.best_of, args.beam_size)
        return 0
    except Exception as e:
        # W przypadku błędu, zwracamy informację w formacie JSON
        error_info = {"error": str(e), "status": "error"}
        print(json.dumps(error_info))
        return 1

if __name__ == "__main__":
    sys.exit(main()) 