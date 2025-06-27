// Start of file: SpeechGenerator.cs
// Purpose: Handles text-to-speech generation for the chatbot, providing methods to speak text asynchronously.
// This class is Windows-only due to reliance on System.Speech.Synthesis.

using System;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Linq;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Handles text-to-speech generation for the chatbot's responses.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class SpeechGenerator
    {
        private readonly SpeechSynthesizer? _synthesizer;
        private readonly bool _isSupported;

        /// <summary>
        /// Initializes a new instance of the SpeechGenerator class.
        /// Sets up the speech synthesizer if the platform is Windows.
        /// </summary>
        public SpeechGenerator()
        {
            // Check if running on Windows using multiple methods for better compatibility
            bool isWindowsRuntime = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool isWindowsEnv = Environment.OSVersion.Platform == PlatformID.Win32NT ||
                               Environment.OSVersion.Platform == PlatformID.Win32S ||
                               Environment.OSVersion.Platform == PlatformID.Win32Windows;
            _isSupported = isWindowsRuntime || isWindowsEnv;

            if (_isSupported)
            {
                try
                {
                    _synthesizer = new SpeechSynthesizer();

                    // Check if any voices are installed
                    var installedVoices = _synthesizer.GetInstalledVoices();
                    if (installedVoices.Count == 0)
                    {
                        _isSupported = false;
                        _synthesizer?.Dispose();
                        _synthesizer = null;
                        return;
                    }

                    // Try to select a preferred voice (male, adult) for better user experience
                    try
                    {
                        var preferredVoice = installedVoices
                            .Where(v => v.VoiceInfo.Gender == VoiceGender.Male && v.VoiceInfo.Age == VoiceAge.Adult)
                            .FirstOrDefault();

                        if (preferredVoice != null)
                        {
                            _synthesizer.SelectVoice(preferredVoice.VoiceInfo.Name);
                        }
                        else
                        {
                            // Fallback to any available voice
                            _synthesizer.SelectVoice(installedVoices.First().VoiceInfo.Name);
                        }
                    }
                    catch
                    {
                        // If voice selection fails, continue with default voice
                    }

                    // Configure speech parameters for optimal experience
                    _synthesizer.Volume = 80;  // Slightly lower volume to be less jarring
                    _synthesizer.Rate = 0;     // Normal speech rate (range: -10 to 10)
                }
                catch (Exception)
                {
                    _isSupported = false;
                    _synthesizer?.Dispose();
                }
            }
        }

        /// <summary>
        /// Converts text to speech and plays it asynchronously if supported.
        /// Runs the speech in the background without blocking the caller.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SpeakAsync(string text)
        {
            if (!_isSupported || _synthesizer == null || string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            try
            {
                // Clean up the text for better speech synthesis
                string cleanedText = CleanTextForSpeech(text);

                // Use a simpler approach to avoid TaskCompletionSource issues
                await Task.Run(() =>
                {
                    try
                    {
                        _synthesizer.Speak(cleanedText); // Use synchronous method in background thread
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Speech synthesis error: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                // Log error but don't throw - TTS should be non-critical
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error generating speech: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Speaks a single segment of text asynchronously if supported.
        /// Used for incremental speech, though not used in the final implementation.
        /// </summary>
        /// <param name="segment">The text segment to speak.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SpeakSegmentAsync(string segment)
        {
            if (!_isSupported || _synthesizer == null || string.IsNullOrWhiteSpace(segment))
            {
                return;
            }

            try
            {
                string cleanedSegment = CleanTextForSpeech(segment);

                // Use simple background task approach
                await Task.Run(() =>
                {
                    try
                    {
                        _synthesizer.Speak(cleanedSegment);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Speech segment synthesis error: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error generating speech for segment: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Cleans text for better speech synthesis by removing or replacing problematic characters.
        /// </summary>
        /// <param name="text">The text to clean.</param>
        /// <returns>Cleaned text suitable for speech synthesis.</returns>
        private string CleanTextForSpeech(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            // Replace common symbols and abbreviations for better pronunciation
            return text
                .Replace("&", "and")
                .Replace("@", "at")
                .Replace("#", "number")
                .Replace("%", "percent")
                .Replace("$", "dollar")
                .Replace("2FA", "two factor authentication")
                .Replace("VPN", "V P N")
                .Replace("URL", "U R L")
                .Replace("Wi-Fi", "WiFi")
                .Replace("ID", "I D")
                .Replace("IP", "I P")
                .Replace("DNS", "D N S")
                .Replace("HTTP", "H T T P")
                .Replace("HTTPS", "H T T P S")
                .Replace("...", ".")
                .Replace("..", ".")
                .Trim();
        }

        /// <summary>
        /// Stops any current speech synthesis.
        /// </summary>
        public void StopSpeaking()
        {
            if (_isSupported && _synthesizer != null)
            {
                try
                {
                    _synthesizer.SpeakAsyncCancelAll();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error stopping speech: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Gets information about available voices on the system.
        /// </summary>
        /// <returns>Array of voice names, or empty array if TTS not supported.</returns>
        public string[] GetAvailableVoices()
        {
            if (!_isSupported || _synthesizer == null)
                return Array.Empty<string>();

            try
            {
                return _synthesizer.GetInstalledVoices()
                    .Select(v => v.VoiceInfo.Name)
                    .ToArray();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Disposes of the speech synthesizer to release resources if supported.
        /// </summary>
        public void Dispose()
        {
            if (_isSupported && _synthesizer != null)
            {
                try
                {
                    _synthesizer.SpeakAsyncCancelAll();
                    _synthesizer.Dispose();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error disposing speech synthesizer: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}

// End of file: SpeechGenerator.cs