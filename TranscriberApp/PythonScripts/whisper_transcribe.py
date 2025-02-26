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

def transcribe_audio(audio_path, output_path=None, model_size='small', language='pl'):
    """
    Transkrybuje audio za pomocą Whisper AI.
    
    Args:
        audio_path: Ścieżka do pliku audio
        output_path: Ścieżka do pliku wyjściowego (.md)
        model_size: Rozmiar modelu Whisper (tiny, base, small, medium, large)
        language: Język transkrypcji (np. 'pl', 'en', 'auto')
        
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
        # Wypisujemy informację o rozpoczęciu
        print(json.dumps({"progress": 0, "status": "loading_model"}))
        sys.stdout.flush()
        
        # Sprawdzenie dostępności GPU
        device = "cuda" if torch.cuda.is_available() else "cpu"
        print(json.dumps({"info": f"Używam urządzenia: {device}"}))
        sys.stdout.flush()
        
        # Ładowanie modelu
        model = whisper.load_model(model_size, device=device)
        print(json.dumps({"progress": 10, "status": "model_loaded"}))
        sys.stdout.flush()
        
        # Opcje transkrypcji
        transcribe_options = {
            "language": language if language != "auto" else None,
            "task": "transcribe"
        }
        
        # Rozpoczęcie transkrypcji
        print(json.dumps({"progress": 20, "status": "transcribing"}))
        sys.stdout.flush()
        
        # Faktyczna transkrypcja
        result = model.transcribe(audio_path, **transcribe_options, verbose=False)
        
        # Informacja o zakończeniu transkrypcji
        print(json.dumps({"progress": 90, "status": "formatting"}))
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
            f.write(f"**Data:** {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n\n")
            
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
        
        # Informacja o zakończeniu
        print(json.dumps({"progress": 100, "status": "completed", "output_path": output_path}))
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
        transcribe_audio(args.audio_path, output_path, args.model, args.language)
        return 0
    except Exception as e:
        # W przypadku błędu, zwracamy informację w formacie JSON
        error_info = {"error": str(e), "status": "error"}
        print(json.dumps(error_info))
        return 1

if __name__ == "__main__":
    sys.exit(main()) 