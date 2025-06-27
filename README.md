# CybersecurityAwarenessBot - Part 2

## Overview
The CybersecurityAwarenessBot is a C# console application designed to educate users on cybersecurity topics through an interactive chatbot experience. Built with .NET 8.0, this project fulfills the requirements for Part 2, incorporating advanced features like keyword recognition, random responses, conversation flow, memory and recall, sentiment detection, error handling, and a polished user interface with text-to-speech (TTS) capabilities.

### Key Features
- **Keyword Recognition**: Recognizes keywords such as "password", "scam", "phishing", "privacy", "firewall", "vpn", and "malware", providing relevant responses.
- **Random Responses**: Offers multiple response variations for each topic, selected randomly to keep conversations engaging.
- **Conversation Flow**: Maintains natural conversation flow with follow-up questions like "Would you like to know more?" and tailored follow-up responses.
- **Memory and Recall**: Stores user details (name, favorite topic, last question) and recalls them on demand (e.g., "What was my last question?").
- **Sentiment Detection**: Detects user sentiments ("worried", "curious", "frustrated", "happy") and adjusts responses accordingly (e.g., "I can see you’re worried—don’t stress, I’ll help you stay safe!").
- **Error Handling**: Gracefully handles invalid inputs, missing audio files, and unrecognized questions with appropriate messages.
- **Polished UI**: Features ASCII art, colored text (green for prompts, cyan for responses), and a typing animation for a professional look.
- **Text-to-Speech (TTS)**: Supports TTS on Windows, speaking responses naturally after a slight delay, overlapping with the typing animation for a seamless experience.
- **Code Optimization**: Utilizes efficient data structures (e.g., `Dictionary` for responses) and a modular design with separate classes for functionality.
- **Unit Tests**: Includes comprehensive unit tests in `ResponseHandlerTests.cs`, covering all major functionalities with 5 passing tests.
- **Documentation**: Fully commented code with start-of-file and end-of-file comments, explaining functionality and purpose.

### Bonus Features for Creativity and Polish
- Additional cybersecurity topics beyond the minimum requirements (e.g., "firewall", "vpn", "malware").
- Smooth TTS integration with a natural speaking flow.
- Engaging UI with ASCII art, colored text, and a typing animation (15ms delay for a faster effect).

## Project Structure
- **CybersecurityAwarenessBot/**
  - `Program.cs`: Main entry point, handles user interaction and bot execution.
  - `ResponseHandler.cs`: Manages response generation, keyword recognition, sentiment detection, and memory.
  - `DisplayManager.cs`: Handles UI elements like ASCII art, colored text, and typing animation.
  - `SpeechGenerator.cs`: Generates speech audio files for TTS.
  - `TtsHelper.cs`: Interfaces with the Windows TTS API.
  - `AudioPlayer.cs`: Plays audio files asynchronously using NAudio.
- **CybersecurityAwarenessBot.Tests/**
  - `ResponseHandlerTests.cs`: Unit tests for the `ResponseHandler` class.

## Prerequisites
- **.NET 8.0 SDK**: Required to build and run the project.
- **Windows OS (Optional)**: TTS and audio playback features are supported on Windows. On other platforms, these features are skipped with a warning.
- **NAudio**: Used for audio playback (included via NuGet).

## Setup Instructions
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/your-username/CyberSecurityAwarenessBotPart2.git
   cd CyberSecurityAwarenessBotPart2

Restore Dependencies:
bash

dotnet restore

Build the Project:
bash

dotnet build

Ensure Audio Files (Optional):
Place a greeting.wav file in the CybersecurityAwarenessBot/bin/Debug/net8.0/ directory for the welcome audio. If missing, the bot will skip audio playback with a warning.

Running the Bot
Navigate to the main project directory:
bash

cd CybersecurityAwarenessBot

Run the bot:
bash

dotnet run

Follow the prompts:
Enter your name to start.

Ask questions about cybersecurity topics (e.g., "Tell me about password safety").

Type "exit" to quit.

Usage Examples
Welcome and Name Input:

[ASCII Art Welcome Message]
Hello! Welcome to the Cybersecurity Awareness Bot!
What’s your name?
> Morgan
Hello, Morgan! I'm here to help with cybersecurity questions.

Keyword Recognition and Random Response:

> Tell me about password safety
Use strong passwords with at least 12 characters, including letters, numbers, and symbols.
Would you like to know more about password safety (yes/no)...

Conversation Flow:

> yes
Here’s another tip: Enable two-factor authentication for extra security!

Memory and Recall:

> What was my last question?
Your last question was: 'tell me about password safety'. Would you like to explore that topic further?

Sentiment Detection:

> I’m worried about scams
I can see you’re worried—don’t stress, I’ll help you stay safe! Scams often come via email, text, or calls. Always verify the sender before acting.

Error Handling:

> [Enter]
Question cannot be empty. Please try again.

Running Unit Tests
Navigate to the test directory:
bash

cd CybersecurityAwarenessBot.Tests

Run the tests:
bash

dotnet test

Expected output:

Test summary: total: 5, failed: 0, succeeded: 5, skipped: 0, duration: 0.7s
Build succeeded in 2.0s

Platform Compatibility
Windows: Full support for TTS and audio playback.

Other Platforms: TTS and audio playback are skipped with warnings (e.g., "Warning: Audio file 'greeting.wav' not found. Skipping playback.").

