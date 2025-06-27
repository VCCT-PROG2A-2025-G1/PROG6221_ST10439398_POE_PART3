// Start of file: AudioPlayer.cs
// Purpose: Handles audio playback for pre-recorded audio files (e.g., greeting.wav).
// Implements error handling for missing audio files as per Part 2 requirements.

using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Handles audio playback for pre-recorded audio files using NAudio.
    /// </summary>
    public class AudioPlayer
    {
        /// <summary>
        /// Plays an audio file asynchronously if it exists.
        /// </summary>
        /// <param name="filePath">The path to the audio file (e.g., "greeting.wav").</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task PlayAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Warning: Audio file '{filePath}' not found. Skipping playback.");
                Console.ResetColor();
                return;
            }

            try
            {
                using var audioFile = new AudioFileReader(filePath);
                using var outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Play();

                // Wait for playback to complete
                await Task.Run(() =>
                {
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Task.Delay(100).Wait();
                    }
                });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error playing audio file '{filePath}': {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}

// End of file: AudioPlayer.cs