// Start of file: TtsHelper.cs
// Purpose: Isolates text-to-speech operations, providing a simplified interface for speaking text.
// This class is Windows-only due to reliance on SpeechGenerator.

using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Helper class to manage text-to-speech operations, isolating platform-specific code.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class TtsHelper
    {
        private readonly SpeechGenerator? _speech;
        private readonly DisplayManager _display;
        private readonly bool _isInitialized;

        /// <summary>
        /// Gets whether TTS is available and properly initialized.
        /// </summary>
        public bool IsAvailable => _isInitialized && _speech != null;

        /// <summary>
        /// Initializes a new instance of the TtsHelper class.
        /// Sets up the speech generator and handles initialization errors gracefully.
        /// </summary>
        /// <param name="display">The display manager for error messaging.</param>
        public TtsHelper(DisplayManager display)
        {
            _display = display ?? throw new ArgumentNullException(nameof(display));
            _isInitialized = false;

            try
            {
                _speech = new SpeechGenerator();
                _isInitialized = true;

                // Test if TTS is actually working by attempting to get available voices
                var voices = _speech.GetAvailableVoices();
                if (voices.Length == 0)
                {
                    _display.ShowMessage("No text-to-speech voices found on this system.", ConsoleColor.Yellow);
                    _speech?.Dispose();
                    _speech = null;
                }
            }
            catch (Exception ex)
            {
                _display.ShowError($"Failed to initialize text-to-speech: {ex.Message}");
                _display.ShowMessage("The bot will continue without voice output.", ConsoleColor.Yellow);
                _speech?.Dispose();
            }
        }

        /// <summary>
        /// Speaks the given text if TTS is supported, otherwise does nothing silently.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SpeakAsync(string text)
        {
            if (!IsAvailable || string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            try
            {
                await _speech!.SpeakAsync(text);
            }
            catch (Exception ex)
            {
                // Log the error but don't show it to user during normal operation
                // to avoid disrupting the conversation flow
                Console.WriteLine($"TTS Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Speaks a single segment of text if TTS is supported, otherwise does nothing.
        /// </summary>
        /// <param name="segment">The text segment to speak.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SpeakSegmentAsync(string segment)
        {
            if (!IsAvailable || string.IsNullOrWhiteSpace(segment))
            {
                return;
            }

            try
            {
                await _speech!.SpeakSegmentAsync(segment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TTS Segment Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Stops any currently playing speech.
        /// </summary>
        public void StopSpeaking()
        {
            if (IsAvailable)
            {
                try
                {
                    _speech!.StopSpeaking();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TTS Stop Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets the list of available voices on the system.
        /// </summary>
        /// <returns>Array of voice names, or empty array if TTS not available.</returns>
        public string[] GetAvailableVoices()
        {
            if (!IsAvailable)
                return Array.Empty<string>();

            try
            {
                return _speech!.GetAvailableVoices();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TTS Voice Query Error: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Disposes of the speech generator if it was initialized.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _speech?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TTS Dispose Error: {ex.Message}");
            }
        }
    }
}

// End of file: TtsHelper.cs