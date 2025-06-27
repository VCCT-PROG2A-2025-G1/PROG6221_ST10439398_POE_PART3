// Start of file: DisplayManager.cs
// Purpose: Manages console display operations, including ASCII art, messages, prompts, and responses.
// Integrates with TTS to display and speak text simultaneously for a fluid user experience.

using System;
using System.Threading.Tasks;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Handles all console display operations, including ASCII art, colored messages, and typing animations.
    /// </summary>
    public class DisplayManager
    {
        private const int TypingDelay = 15; // Reduced delay for faster typing animation
        private const int AnimationDelay = 100; // Delay for UI animations
        private const int SpeechStartDelay = 300; // Delay before starting speech to align with typing

        /// <summary>
        /// Displays the ASCII art header with a colorful animation.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ShowAsciiArt()
        {
            ConsoleColor[] colors = { ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Yellow };
            string[] lines = new[]
            {
                "   _____ _    _ ______  _____ ",
                "  / ____| |  | |  ____|/ ____|",
                " | |    | |__| | |__  | (___  ",
                " | |    |  __  |  __|  \\___ \\ ",
                " | |____| |  | | |____ ____) |",
                "  \\_____|_|  |_|______|_____/ ",
                "Cybersecurity Awareness Bot v2.0"
            };

            for (int i = 0; i < lines.Length; i++)
            {
                Console.ForegroundColor = colors[i % colors.Length];
                Console.WriteLine(lines[i].PadRight(Console.WindowWidth));
                await Task.Delay(AnimationDelay);
            }
            Console.ResetColor();
            ShowSeparator('=', Console.WindowWidth - 1);
            Console.WriteLine();
        }

        /// <summary>
        /// Displays the welcome message with a decorative border and animation.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ShowWelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            ShowSeparator('=', Console.WindowWidth - 1);
            string message = " Welcome to the Cybersecurity Awareness Bot v2.0! ";
            int padding = (Console.WindowWidth - message.Length) / 2;
            Console.WriteLine(message.PadLeft(padding + message.Length).PadRight(Console.WindowWidth));
            ShowSeparator('=', Console.WindowWidth - 1);
            Console.WriteLine();
            Console.ResetColor();
            await Task.Delay(AnimationDelay);
        }

        /// <summary>
        /// Prompts the user for input with a colored prompt and animated dots.
        /// Optionally speaks the prompt if TTS is available.
        /// </summary>
        /// <param name="prompt">The prompt message to display.</param>
        /// <param name="color">The color of the prompt text.</param>
        /// <param name="tts">The TTS helper to speak the prompt (optional).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ShowPrompt(string prompt, ConsoleColor color, TtsHelper? tts = null)
        {
            Console.ForegroundColor = color;
            Console.Write(prompt);
            for (int i = 0; i < 3; i++)
            {
                Console.Write(".");
                await Task.Delay(AnimationDelay);
            }
            Console.Write(" ");
#pragma warning disable CA1416 // Suppress CA1416: TtsHelper.SpeakAsync is Windows-only, but we check for null
            if (tts != null)
            {
                // Fire-and-forget TTS to avoid blocking
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(SpeechStartDelay);
                        await tts.SpeakAsync(prompt + "...");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"TTS Prompt Error: {ex.Message}");
                    }
                });
            }
#pragma warning restore CA1416
            Console.ResetColor();
        }

        /// <summary>
        /// Displays a message with a typing animation effect and optionally speaks it.
        /// The speech starts after a short delay to align with the typing animation.
        /// </summary>
        /// <param name="message">The message to display and speak.</param>
        /// <param name="tts">The TTS helper to speak the message (optional).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ShowResponse(string message, TtsHelper? tts = null)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nAnswer: ");

            // Start speech in background while typing begins
#pragma warning disable CA1416 // Suppress CA1416: TtsHelper.SpeakAsync is Windows-only, but we check for null
            if (tts != null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(SpeechStartDelay); // Delay speech to align with typing
                        await tts.SpeakAsync(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"TTS Response Error: {ex.Message}");
                    }
                });
            }
#pragma warning restore CA1416

            // Split the message into words for typing animation
            string[] words = message.Split(' ');

            foreach (string word in words)
            {
                foreach (char c in word)
                {
                    Console.Write(c);
                    await Task.Delay(TypingDelay);
                }
                Console.Write(" "); // Add space between words
                await Task.Delay(TypingDelay * 2); // Slight pause between words
            }

            Console.WriteLine("\n");
            Console.ResetColor();
        }

        /// <summary>
        /// Displays an error message in red with a bordered layout.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        public void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ShowSeparator('-', Console.WindowWidth - 1);
            Console.WriteLine($" Error: {message} ".PadRight(Console.WindowWidth));
            ShowSeparator('-', Console.WindowWidth - 1);
            Console.WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// Displays a message in the specified color with centered padding.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="color">The color of the message text.</param>
        public void ShowMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            int padding = (Console.WindowWidth - message.Length) / 2;
            Console.WriteLine(message.PadLeft(padding + message.Length).PadRight(Console.WindowWidth));
            Console.WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// Displays a separator line with the specified character.
        /// </summary>
        /// <param name="character">The character to use for the separator.</param>
        /// <param name="length">The length of the separator line.</param>
        public void ShowSeparator(char character, int length)
        {
            Console.WriteLine(new string(character, length));
        }

        /// <summary>
        /// Prompts for and validates the user's name with an animated prompt.
        /// </summary>
        /// <returns>A task that resolves to the validated user name.</returns>
        public async Task<string> GetUserName()
        {
            await ShowPrompt("Please enter your name", ConsoleColor.Green);
            string? userName = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(userName))
            {
                ShowError("Name cannot be empty. Please try again.");
                await ShowPrompt("Please enter your name", ConsoleColor.Green);
                userName = Console.ReadLine();
            }
            return userName;
        }
    }
}

// End of file: DisplayManager.cs