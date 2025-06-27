// Start of file: MainForm.cs
// Purpose: Complete GUI implementation with enhanced NLP, task management, quiz system, and activity logging
// Implements all Part 3 requirements with integration of Parts 1 & 2

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;

namespace CybersecurityAwarenessBot
{
    public partial class MainForm : Form
    {
        private readonly FullTaskManager _taskManager;
        private readonly ActivityLogger _activityLogger;
        private readonly AudioPlayer _audioPlayer;
        private readonly TtsHelper? _ttsHelper;
        private readonly ResponseHandler _responseHandler;
        private readonly EnhancedNlpHandler _nlpHandler;
        private string? _userName;

        private TabControl mainTabControl = null!;
        private TabPage chatTabPage = null!;
        private TabPage tasksTabPage = null!;
        private TabPage quizTabPage = null!;
        private TabPage activityTabPage = null!;
        private TabPage nlpInsightsTabPage = null!;

        private RichTextBox chatDisplay = null!;
        private TextBox chatInput = null!;
        private Button sendButton = null!;
        private Button clearChatButton = null!;
        private Label nlpStatusLabel = null!;
        private RichTextBox nlpInsightsDisplay = null!;

        public MainForm()
        {
            _taskManager = new FullTaskManager();
            _activityLogger = new ActivityLogger();
            _audioPlayer = new AudioPlayer();
            _responseHandler = new ResponseHandler();
            _nlpHandler = new EnhancedNlpHandler(_activityLogger);

            // Initialize TTS with simple approach
            try
            {
                _ttsHelper = new TtsHelper(new DisplayManager());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TTS not available: {ex.Message}");
                _ttsHelper = null;
            }

            InitializeComponent();
            SetupEventHandlers();

            // Initialize bot after form is shown
            this.Shown += async (s, e) => await InitializeBotSequence();
        }

        private void InitializeComponent()
        {
            this.Text = "🛡️ Cybersecurity Awareness Bot v3.0 - Enhanced NLP";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1200, 700);

            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };

            chatTabPage = new TabPage("💬 Chat");
            tasksTabPage = new TabPage("📋 Tasks");
            quizTabPage = new TabPage("🧠 Quiz");
            activityTabPage = new TabPage("📊 Activity Log");
            nlpInsightsTabPage = new TabPage("🔍 NLP Insights");

            mainTabControl.TabPages.AddRange(new TabPage[] {
                chatTabPage, tasksTabPage, quizTabPage, activityTabPage, nlpInsightsTabPage
            });

            this.Controls.Add(mainTabControl);

            InitializeChatTab();
            InitializeTasksTab();
            InitializeQuizTab();
            InitializeActivityTab();
            InitializeNlpInsightsTab();
        }

        private void InitializeChatTab()
        {
            chatDisplay = new RichTextBox
            {
                Dock = DockStyle.Top,
                Height = 500,
                ReadOnly = true,
                BackColor = Color.FromArgb(248, 249, 250),
                Font = new Font("Consolas", 10F),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle
            };

            // NLP Status Panel
            var nlpStatusPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(220, 248, 198),
                Padding = new Padding(10, 5, 10, 5)
            };

            nlpStatusLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = "🤖 Enhanced NLP Active - Advanced message understanding enabled",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                TextAlign = ContentAlignment.MiddleLeft
            };

            nlpStatusPanel.Controls.Add(nlpStatusLabel);

            var inputPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            chatInput = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Ask about cybersecurity, request tasks, or say 'start quiz'..."
            };

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 120,
                Padding = new Padding(5, 0, 0, 0)
            };

            sendButton = new Button
            {
                Text = "Send",
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            clearChatButton = new Button
            {
                Text = "Clear Chat",
                Dock = DockStyle.Bottom,
                Height = 35,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            buttonPanel.Controls.AddRange(new Control[] { sendButton, clearChatButton });
            inputPanel.Controls.AddRange(new Control[] { chatInput, buttonPanel });

            var welcomePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            welcomePanel.Controls.Add(chatDisplay);
            chatTabPage.Controls.AddRange(new Control[] { nlpStatusPanel, inputPanel, welcomePanel });
        }

        private void InitializeTasksTab()
        {
            var taskPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            tasksTabPage.Controls.Add(taskPanel);
            _taskManager.InitializeTaskInterface(taskPanel, _activityLogger);
        }

        private void InitializeQuizTab()
        {
            var quizPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            var quizButton = new Button
            {
                Text = "🧠 Start Cybersecurity Quiz",
                Size = new Size(300, 60),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold)
            };

            var quizDescription = new Label
            {
                Text = "Test your cybersecurity knowledge with our comprehensive quiz!\n\n" +
                       "Features:\n" +
                       "• 18 carefully crafted cybersecurity questions\n" +
                       "• Mix of true/false and multiple choice formats\n" +
                       "• Immediate feedback with detailed explanations\n" +
                       "• Performance scoring and analysis\n" +
                       "• Randomized questions for variety\n\n" +
                       "Topics covered: Passwords, Phishing, Malware, VPNs, Social Engineering, and more!",
                Location = new Point(20, 100),
                Size = new Size(700, 200),
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            quizButton.Click += (s, e) =>
            {
                var quizForm = new QuizForm(_activityLogger);
                quizForm.ShowDialog(this);
            };

            quizPanel.Controls.AddRange(new Control[] { quizButton, quizDescription });
            quizTabPage.Controls.Add(quizPanel);
        }

        private void InitializeActivityTab()
        {
            var activityPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            var activityViewer = new ActivityLogViewer(_activityLogger);
            activityViewer.InitializeActivityInterface(activityPanel);

            activityTabPage.Controls.Add(activityPanel);
        }

        private void InitializeNlpInsightsTab()
        {
            var nlpPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            var titleLabel = new Label
            {
                Text = "🔍 Natural Language Processing Insights",
                Location = new Point(10, 10),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            var infoLabel = new Label
            {
                Text = "This tab shows detailed analysis of your messages using advanced NLP techniques.",
                Location = new Point(10, 50),
                Size = new Size(600, 20),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            nlpInsightsDisplay = new RichTextBox
            {
                Location = new Point(10, 80),
                Size = new Size(800, 500),
                ReadOnly = true,
                BackColor = Color.FromArgb(248, 249, 250),
                Font = new Font("Consolas", 9F),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle
            };

            var clearInsightsButton = new Button
            {
                Text = "Clear Insights",
                Location = new Point(10, 590),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            clearInsightsButton.Click += (s, e) => nlpInsightsDisplay.Clear();

            nlpPanel.Controls.AddRange(new Control[] { titleLabel, infoLabel, nlpInsightsDisplay, clearInsightsButton });
            nlpInsightsTabPage.Controls.Add(nlpPanel);
        }

        private void SetupEventHandlers()
        {
            sendButton.Click += SendButton_Click;
            clearChatButton.Click += (s, e) => ClearChat();
            chatInput.KeyDown += ChatInput_KeyDown;
            this.FormClosing += MainForm_FormClosing;
        }

        private async void SendButton_Click(object? sender, EventArgs e)
        {
            await SendMessage();
        }

        private async void ChatInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.Handled = true;
                await SendMessage();
            }
        }

        private async Task InitializeBotSequence()
        {
            // Show welcome message
            AddSystemMessage("🛡️ Cybersecurity Awareness Bot v3.0 - Enhanced NLP", Color.FromArgb(0, 123, 255));
            await Task.Delay(500);

            // Play greeting audio
            _ = Task.Run(async () => await _audioPlayer.PlayAsync("greeting.wav"));

            // Get user name
            await GetUserName();

            // Set user name in response handler
            _responseHandler.SetUserName(_userName ?? "User");

            // Show personalized greeting
            var greetingMessage = $"Hello, {_userName}! I'm now powered by advanced NLP for better understanding of your cybersecurity needs!";
            AddBotMessage(greetingMessage);

            // Log initial interaction
            _activityLogger.LogActivity("Chat", "Session started", $"User {_userName} started a new session with Enhanced NLP");

            // Show main prompt
            var promptMessage = "Try asking about cybersecurity topics, saying 'start quiz', 'add task', or 'remind me to update my passwords'! " +
                               "Check the NLP Insights tab to see how I understand your messages.";
            AddBotMessage(promptMessage);
        }

        private async Task GetUserName()
        {
            using var nameForm = new Form
            {
                Text = "🛡️ Welcome to Enhanced NLP Bot!",
                Size = new Size(450, 250),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label
            {
                Text = "Welcome to the Enhanced NLP Cybersecurity Bot!\nPlease enter your name to get started:",
                Location = new Point(20, 20),
                Size = new Size(400, 50),
                Font = new Font("Segoe UI", 10F)
            };

            var textBox = new TextBox
            {
                Location = new Point(20, 80),
                Size = new Size(390, 25),
                Font = new Font("Segoe UI", 10F)
            };

            var okButton = new Button
            {
                Text = "Start Learning!",
                Location = new Point(250, 130),
                Size = new Size(120, 35),
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            var featuresLabel = new Label
            {
                Text = "✨ New in v3.0: Advanced intent detection, entity extraction, and phrase analysis!",
                Location = new Point(20, 175),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            nameForm.Controls.AddRange(new Control[] { label, textBox, okButton, featuresLabel });
            nameForm.AcceptButton = okButton;

            while (true)
            {
                textBox.Focus();
                var result = nameForm.ShowDialog(this);

                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    _userName = textBox.Text.Trim();
                    break;
                }

                MessageBox.Show("Please enter a valid name.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task SendMessage()
        {
            var message = chatInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(message))
                return;

            chatInput.Clear();

            // Add user message
            AddUserMessage(message);
            _activityLogger.LogActivity("Chat", "User message", message);

            // Update NLP status
            UpdateNlpStatus("Processing...");

            // Handle exit
            if (message.ToLower() == "exit")
            {
                var goodbyeMessage = $"Goodbye, {_userName}! Stay safe online!";
                AddBotMessage(goodbyeMessage);

                if (_ttsHelper != null)
                {
                    await _ttsHelper.SpeakAsync(goodbyeMessage);
                    await Task.Delay(2000);
                }

                Application.Exit();
                return;
            }

            // Process with Enhanced NLP
            var nlpResult = _nlpHandler.ProcessMessage(message);

            // Display NLP insights
            DisplayNlpInsights(nlpResult);

            // Update NLP status with results
            UpdateNlpStatus($"Intent: {nlpResult.Intent} | Confidence: {nlpResult.Confidence:P1} | Topics: {string.Join(", ", nlpResult.Topics)}");

            var response = await ProcessNlpResult(nlpResult, message);

            AddBotMessage(response);
            _activityLogger.LogActivity("Chat", "Bot response", "Enhanced NLP response provided");
        }

        private async Task<string> ProcessNlpResult(NlpResult nlpResult, string originalMessage)
        {
            // Handle specific intents
            switch (nlpResult.Intent)
            {
                case "create_task":
                case "set_reminder":
                    return await HandleTaskIntent(nlpResult, originalMessage);

                case "start_quiz":
                    return HandleQuizIntent();

                case "information_request":
                case "help_request":
                    return HandleInformationRequest(nlpResult, originalMessage);

                case "memory_recall":
                    return HandleMemoryRecall(nlpResult, originalMessage);

                case "security_assessment":
                    return HandleSecurityAssessment(nlpResult);

                case "quiz_results":
                    return HandleQuizResults();

                default:
                    // Use enhanced response with NLP context
                    var baseResponse = _responseHandler.GetResponse(originalMessage);

                    // Add NLP insights if confidence is high
                    if (nlpResult.Confidence > 0.7 && nlpResult.Topics.Any())
                    {
                        var topicInfo = $"\n\n🔍 I detected you're interested in: {string.Join(", ", nlpResult.Topics)}";
                        if (nlpResult.SuggestedActions.Any())
                        {
                            topicInfo += $"\n💡 Suggested actions: {string.Join(", ", nlpResult.SuggestedActions)}";
                        }
                        return baseResponse + topicInfo;
                    }

                    return baseResponse;
            }
        }

        private async Task<string> HandleTaskIntent(NlpResult nlpResult, string originalMessage)
        {
            // Extract task information from entities and message
            var taskTitle = ExtractTaskTitle(originalMessage, nlpResult);
            var timeEntities = nlpResult.Entities.Where(e => e.Type == "TIME").ToList();

            // Switch to tasks tab
            mainTabControl.SelectedTab = tasksTabPage;

            // Pre-fill task form if we extracted meaningful information
            if (!string.IsNullOrEmpty(taskTitle))
            {
                var timeInfo = timeEntities.Any() ? $" (Time detected: {string.Join(", ", timeEntities.Select(e => e.Value))})" : "";
                _taskManager.PreFillTaskForm(taskTitle, originalMessage);
                return $"I've switched you to the Tasks tab and pre-filled a task: '{taskTitle}'{timeInfo}. You can adjust the details and set a reminder time!";
            }
            else
            {
                return "I've switched you to the Tasks tab where you can create a new cybersecurity task with reminders!";
            }
        }

        private string HandleQuizIntent()
        {
            mainTabControl.SelectedTab = quizTabPage;
            return "Let's test your cybersecurity knowledge! I've switched you to the Quiz tab. Click the quiz button to begin your assessment!";
        }

        private string HandleInformationRequest(NlpResult nlpResult, string originalMessage)
        {
            var response = _responseHandler.GetResponse(originalMessage);

            // Add context based on detected topics
            if (nlpResult.Topics.Any())
            {
                var topicContext = $"\n\n📚 Based on your question about {string.Join(" and ", nlpResult.Topics.Take(2))}, " +
                                 "I can provide more specific information if you need it!";
                response += topicContext;
            }

            return response;
        }

        private string HandleMemoryRecall(NlpResult nlpResult, string originalMessage)
        {
            // Check activity log for relevant past interactions
            var recentActivities = _activityLogger.GetRecentActivities(10);
            var relevantActivities = recentActivities.Where(a =>
                nlpResult.Topics.Any(topic => a.Details.Contains(topic, StringComparison.OrdinalIgnoreCase)))
                .Take(3).ToList();

            if (relevantActivities.Any())
            {
                var memoryResponse = "Here's what I remember from our recent conversations:\n\n";
                foreach (var activity in relevantActivities)
                {
                    memoryResponse += $"• {activity.Timestamp:HH:mm} - {activity.Category}: {activity.Details}\n";
                }
                return memoryResponse;
            }

            return "I don't have specific memory of that topic from our recent conversations, but I'm here to help with any cybersecurity questions you have!";
        }

        private string HandleSecurityAssessment(NlpResult nlpResult)
        {
            var response = "Let me help assess that security concern. ";

            // Check for specific security entities
            var emailEntities = nlpResult.Entities.Where(e => e.Type == "EMAIL").ToList();
            var urlEntities = nlpResult.Entities.Where(e => e.Type == "URL").ToList();
            var ipEntities = nlpResult.Entities.Where(e => e.Type == "IP_ADDRESS").ToList();

            if (emailEntities.Any())
            {
                response += "⚠️ I noticed you mentioned an email address. Be cautious about sharing personal email addresses and always verify the sender's identity. ";
            }

            if (urlEntities.Any())
            {
                response += "🔗 I detected a URL in your message. Always verify links before clicking - hover over them to see the real destination and check for suspicious domains. ";
            }

            if (ipEntities.Any())
            {
                response += "🌐 I found an IP address in your message. Be careful when connecting to unfamiliar IP addresses, especially on public networks. ";
            }

            // Add topic-specific advice
            if (nlpResult.Topics.Contains("Email Security"))
            {
                response += "📧 For email security, always check sender authenticity, be wary of unexpected attachments, and use email filtering. ";
            }

            if (nlpResult.Topics.Contains("Network Security"))
            {
                response += "🔒 For network security, use VPNs on public WiFi, keep your router firmware updated, and use strong network passwords. ";
            }

            response += "\n\n💡 Consider creating a task to follow up on this security concern!";

            return response;
        }

        private string HandleQuizResults()
        {
            mainTabControl.SelectedTab = activityTabPage;
            return "I've switched you to the Activity Log tab where you can see your quiz results and performance history!";
        }

        private string ExtractTaskTitle(string message, NlpResult nlpResult)
        {
            var lowerMessage = message.ToLower();

            // Common task patterns
            var taskPatterns = new[]
            {
                @"remind me to (.+)",
                @"don't forget to (.+)",
                @"add task (?:to |for )?(.+)",
                @"create (?:a )?(?:task|reminder) (?:to |for )?(.+)",
                @"set (?:up )?(?:a )?reminder (?:to |for )?(.+)"
            };

            foreach (var pattern in taskPatterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(lowerMessage, pattern);
                if (match.Success && match.Groups.Count > 1)
                {
                    var taskText = match.Groups[1].Value.Trim();

                    // Clean up the extracted text
                    if (taskText.Length > 0)
                    {
                        taskText = char.ToUpper(taskText[0]) + taskText.Substring(1);
                    }

                    // Add cybersecurity context if not present
                    if (!taskText.Contains("password") && !taskText.Contains("security") &&
                        !taskText.Contains("update") && !taskText.Contains("cyber"))
                    {
                        // Check if it's a general cybersecurity task
                        var securityKeywords = new[] { "backup", "scan", "patch", "firewall", "antivirus" };
                        if (!securityKeywords.Any(keyword => taskText.Contains(keyword)))
                        {
                            taskText = $"Security: {taskText}";
                        }
                    }

                    return taskText;
                }
            }

            // Fallback: check for detected phrases
            var taskPhrases = nlpResult.DetectedPhrases.Where(p => p.StartsWith("Task Requests:")).ToList();
            if (taskPhrases.Any())
            {
                var phrase = taskPhrases.First().Substring("Task Requests: ".Length);
                return $"Follow up on: {phrase}";
            }

            return string.Empty;
        }

        private void DisplayNlpInsights(NlpResult nlpResult)
        {
            var insights = $"[{DateTime.Now:HH:mm:ss}] NLP Analysis Results:\n";
            insights += $"================================\n\n";
            insights += $"Original Message: \"{nlpResult.OriginalMessage}\"\n\n";
            insights += $"🎯 Intent: {nlpResult.Intent} (Confidence: {nlpResult.Confidence:P1})\n\n";

            if (nlpResult.Topics.Any())
            {
                insights += $"📋 Topics Detected:\n";
                foreach (var topic in nlpResult.Topics)
                {
                    insights += $"   • {topic}\n";
                }
                insights += "\n";
            }

            if (nlpResult.Entities.Any())
            {
                insights += $"🔍 Entities Extracted:\n";
                foreach (var entity in nlpResult.Entities)
                {
                    insights += $"   • {entity.Type}: \"{entity.Value}\" (pos: {entity.StartIndex}-{entity.StartIndex + entity.Length})\n";
                }
                insights += "\n";
            }

            if (nlpResult.DetectedPhrases.Any())
            {
                insights += $"💬 Phrases Detected:\n";
                foreach (var phrase in nlpResult.DetectedPhrases)
                {
                    insights += $"   • {phrase}\n";
                }
                insights += "\n";
            }

            if (nlpResult.SuggestedActions.Any())
            {
                insights += $"💡 Suggested Actions:\n";
                foreach (var action in nlpResult.SuggestedActions)
                {
                    insights += $"   • {action}\n";
                }
                insights += "\n";
            }

            insights += $"⏰ Processed at: {nlpResult.ProcessedAt:yyyy-MM-dd HH:mm:ss}\n";
            insights += "\n" + new string('=', 50) + "\n\n";

            // Add to insights display
            nlpInsightsDisplay.AppendText(insights);
            nlpInsightsDisplay.ScrollToCaret();
        }

        private void UpdateNlpStatus(string status)
        {
            nlpStatusLabel.Text = $"🤖 NLP Status: {status}";
        }

        private void AddUserMessage(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var userText = $"[{timestamp}] 👤 {_userName}: {message}\n\n";

            chatDisplay.SelectionStart = chatDisplay.TextLength;
            chatDisplay.SelectionLength = 0;
            chatDisplay.SelectionColor = Color.FromArgb(0, 123, 255);
            chatDisplay.SelectionFont = new Font("Consolas", 10F, FontStyle.Bold);
            chatDisplay.AppendText(userText);
            chatDisplay.ScrollToCaret();
        }

        private void AddBotMessage(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var botText = $"[{timestamp}] 🤖 Bot: {message}\n\n";

            chatDisplay.SelectionStart = chatDisplay.TextLength;
            chatDisplay.SelectionLength = 0;
            chatDisplay.SelectionColor = Color.FromArgb(40, 167, 69);
            chatDisplay.SelectionFont = new Font("Consolas", 10F);
            chatDisplay.AppendText(botText);
            chatDisplay.ScrollToCaret();

            // Speak the message if TTS is available
            if (_ttsHelper != null && !string.IsNullOrEmpty(message))
            {
                _ = Task.Run(async () => await _ttsHelper.SpeakAsync(message));
            }
        }

        private void AddSystemMessage(string message, Color color)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var systemText = $"[{timestamp}] 🔧 System: {message}\n\n";

            chatDisplay.SelectionStart = chatDisplay.TextLength;
            chatDisplay.SelectionLength = 0;
            chatDisplay.SelectionColor = color;
            chatDisplay.SelectionFont = new Font("Consolas", 10F, FontStyle.Bold);
            chatDisplay.AppendText(systemText);
            chatDisplay.ScrollToCaret();
        }

        private void ClearChat()
        {
            chatDisplay.Clear();
            AddSystemMessage("Chat cleared - Enhanced NLP ready!", Color.FromArgb(220, 53, 69));
            UpdateNlpStatus("Ready for new messages");
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _activityLogger.LogActivity("System", "Session ended", $"User {_userName} ended the session");
            _audioPlayer?.Dispose();
            _ttsHelper?.Dispose();
        }
    }
}

// End of file: MainForm.cs