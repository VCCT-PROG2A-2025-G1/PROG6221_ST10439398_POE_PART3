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
    public class AudioPlayer : IDisposable
    {
        private WaveOutEvent? _outputDevice;
        private AudioFileReader? _audioFile;
        private bool _disposed = false;

        /// <summary>
        /// Plays an audio file asynchronously if it exists.
        /// </summary>
        /// <param name="filePath">The path to the audio file (e.g., "greeting.wav").</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task PlayAsync(string filePath)
        {
            if (_disposed)
                return;

            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Warning: Audio file '{filePath}' not found. Skipping playback.");
                Console.ResetColor();
                return;
            }

            try
            {
                // Dispose previous instances if they exist
                _outputDevice?.Dispose();
                _audioFile?.Dispose();

                _audioFile = new AudioFileReader(filePath);
                _outputDevice = new WaveOutEvent();
                _outputDevice.Init(_audioFile);
                _outputDevice.Play();

                // Wait for playback to complete
                await Task.Run(() =>
                {
                    while (_outputDevice.PlaybackState == PlaybackState.Playing && !_disposed)
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

        /// <summary>
        /// Stops audio playback.
        /// </summary>
        public void Stop()
        {
            try
            {
                _outputDevice?.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping audio: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes of audio resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose method.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                try
                {
                    _outputDevice?.Stop();
                    _outputDevice?.Dispose();
                    _audioFile?.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disposing audio resources: {ex.Message}");
                }
                finally
                {
                    _outputDevice = null;
                    _audioFile = null;
                    _disposed = true;
                }
            }
        }
    }
}

// End of file: AudioPlayer.cs