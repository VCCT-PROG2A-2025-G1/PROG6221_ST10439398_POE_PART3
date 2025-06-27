// Start of file: Program.cs
// Purpose: Main entry point for the Cybersecurity Awareness Bot.
// Orchestrates user interaction, display, audio playback, response generation, and TTS.

using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Main program class for the Cybersecurity Awareness Bot.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point of the application.
        /// Initializes components, handles user interaction, and manages the conversation loop.
        /// </summary>
        /// <param name="args">Command-line arguments (not used).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        static async Task Main(string[] args)
        {
            try
            {
                // Initialize components
                var display = new DisplayManager();
                var audio = new AudioPlayer();
                var responder = new ResponseHandler();

                // Check if TTS is supported (Windows-only)
                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                TtsHelper? tts = null;
#pragma warning disable CA1416 // Suppress CA1416: TtsHelper is Windows-only, but we check at runtime
                if (isWindows)
                {
                    try
                    {
                        tts = new TtsHelper(display);
                    }
                    catch (Exception ex)
                    {
                        display.ShowError($"Failed to initialize text-to-speech: {ex.Message}");
                        display.ShowMessage("The bot will continue without voice output.", ConsoleColor.Yellow);
                    }
                }
                else
                {
                    display.ShowMessage("Text-to-speech is not available on this platform. Continuing with text-only mode.", ConsoleColor.Yellow);
                }
#pragma warning restore CA1416

                // Display welcome UI
                await display.ShowAsciiArt();
                await display.ShowWelcomeMessage();

                // Play greeting audio asynchronously (non-blocking)
                _ = Task.Run(async () => await audio.PlayAsync("greeting.wav"));

                // Get user name with validation
                string userName = await display.GetUserName();

                // Personalized greeting
                responder.SetUserName(userName);
                string greetingMessage = $"Hello, {userName}! I'm here to help you learn about cybersecurity and stay safe online.";
                display.ShowMessage(greetingMessage, ConsoleColor.Yellow);

#pragma warning disable CA1416
                if (tts != null)
                {
                    _ = Task.Run(async () => await tts.SpeakAsync(greetingMessage));
                }
#pragma warning restore CA1416

                // Main interaction loop
                while (true)
                {
                    try
                    {
                        await display.ShowPrompt("Ask me a question (or type 'exit' to quit)", ConsoleColor.Magenta, tts);
                        string? userQuestionInput = Console.ReadLine();
                        string userQuestion = userQuestionInput?.Trim() ?? "";

                        if (string.IsNullOrWhiteSpace(userQuestion))
                        {
                            display.ShowError("Question cannot be empty. Please try again.");
                            continue;
                        }

                        string lowerQuestion = userQuestion.ToLower();
                        if (lowerQuestion == "exit" || lowerQuestion == "quit" || lowerQuestion == "bye")
                        {
                            string goodbyeMessage = $"Goodbye, {userName}! Remember to stay vigilant and keep learning about cybersecurity. Stay safe online!";
                            display.ShowMessage(goodbyeMessage, ConsoleColor.Yellow);

#pragma warning disable CA1416
                            if (tts != null)
                            {
                                await tts.SpeakAsync(goodbyeMessage);
                                await Task.Delay(3000); // Give time for speech to complete
                            }
#pragma warning restore CA1416
                            break;
                        }

                        // Get and display response with typing effect and speech
                        string response = responder.GetResponse(userQuestion);
                        await display.ShowResponse(response, tts);

                        // Enhanced follow-up question for conversational flow
                        if (responder.LastTopic != null && responder.LastTopic != "gratitude")
                        {
                            await display.ShowPrompt($"Would you like to know more about {responder.LastTopic}? (yes/no)", ConsoleColor.Cyan, tts);
                            string? followUpInput = Console.ReadLine();
                            string followUp = followUpInput?.ToLower().Trim() ?? "";

                            if (followUp == "yes" || followUp == "y" || followUp == "sure" || followUp == "ok")
                            {
                                string followUpResponse = responder.GetFollowUpResponse(responder.LastTopic);
                                await display.ShowResponse(followUpResponse, tts);
                            }
                            else if (followUp == "no" || followUp == "n")
                            {
                                display.ShowMessage("No problem! What else would you like to learn about?", ConsoleColor.Green);
                            }
                        }

                        // Add some spacing for better readability
                        Console.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        display.ShowError($"An error occurred while processing your question: {ex.Message}");
                        display.ShowMessage("Please try asking your question again.", ConsoleColor.Yellow);
                    }
                }

                // Ensure clean exit with a separator
                display.ShowSeparator('=', Console.WindowWidth - 1);
                display.ShowMessage("Thank you for using the Cybersecurity Awareness Bot!", ConsoleColor.Green);

#pragma warning disable CA1416
                tts?.Dispose(); // Clean up TTS resources
#pragma warning restore CA1416
            }
            catch (Exception ex)
            {
                // Global error handler
                var emergencyDisplay = new DisplayManager();
                emergencyDisplay.ShowError($"A critical error occurred: {ex.Message}");
                emergencyDisplay.ShowMessage("The application will now exit. Please restart the program.", ConsoleColor.Red);

                // Log error details for debugging (in a real application, this would go to a log file)
                Console.WriteLine($"\nError Details: {ex}");

                await Task.Delay(5000); // Give user time to read the error
                Environment.Exit(1);
            }
        }
    }
}

// End of file: Program.cs