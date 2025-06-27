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
        private string? _userName;

        private TabControl mainTabControl = null!;
        private TabPage chatTabPage = null!;
        private TabPage tasksTabPage = null!;
        private TabPage quizTabPage = null!;
        private TabPage activityTabPage = null!;

        private RichTextBox chatDisplay = null!;
        private TextBox chatInput = null!;
        private Button sendButton = null!;
        private Button clearChatButton = null!;

        public MainForm()
        {
            _taskManager = new FullTaskManager();
            _activityLogger = new ActivityLogger();
            _audioPlayer = new AudioPlayer();

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
            this.Text = "🛡️ Cybersecurity Awareness Bot v3.0";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);

            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };

            chatTabPage = new TabPage("💬 Chat");
            tasksTabPage = new TabPage("📋 Tasks");
            quizTabPage = new TabPage("🧠 Quiz");
            activityTabPage = new TabPage("📊 Activity Log");

            mainTabControl.TabPages.AddRange(new TabPage[] {
                chatTabPage, tasksTabPage, quizTabPage, activityTabPage
            });

            this.Controls.Add(mainTabControl);

            InitializeChatTab();
            InitializeOtherTabs();
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
                BorderStyle = BorderStyle.FixedSingle
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
            chatTabPage.Controls.AddRange(new Control[] { inputPanel, welcomePanel });
        }

        private void InitializeOtherTabs()
        {
            // Initialize Tasks tab
            var taskPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            tasksTabPage.Controls.Add(taskPanel);
            _taskManager.InitializeTaskInterface(taskPanel, _activityLogger);

            // Initialize Quiz tab
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

            // Initialize Activity Log tab
            var activityPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            var activityTitle = new Label
            {
                Text = "📊 Activity Log & Chat History",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            var activityList = new ListBox
            {
                Location = new Point(10, 50),
                Size = new Size(760, 400),
                Font = new Font("Consolas", 9F),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var instructionLabel = new Label
            {
                Text = "💡 This log tracks all your interactions: chat messages, tasks created/completed, quiz attempts, and NLP detections.",
                Location = new Point(10, 460),
                Size = new Size(760, 40),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            activityPanel.Controls.AddRange(new Control[] { activityTitle, activityList, instructionLabel });
            activityTabPage.Controls.Add(activityPanel);
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
            AddSystemMessage("🛡️ Cybersecurity Awareness Bot v3.0", Color.FromArgb(0, 123, 255));
            await Task.Delay(500);

            // Play greeting audio
            _ = Task.Run(async () => await _audioPlayer.PlayAsync("greeting.wav"));

            // Get user name
            await GetUserName();

            // Show personalized greeting
            var greetingMessage = $"Hello, {_userName}! I'm here to help with cybersecurity questions.";
            AddBotMessage(greetingMessage);

            // Log initial interaction
            _activityLogger.LogActivity("Chat", "Session started", $"User {_userName} started a new session");

            // Show main prompt
            var promptMessage = "Ask me a question about cybersecurity, or try commands like 'start quiz' or 'add task'!";
            AddBotMessage(promptMessage);
        }

        private async Task GetUserName()
        {
            using var nameForm = new Form
            {
                Text = "🛡️ Welcome!",
                Size = new Size(400, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label
            {
                Text = "Please enter your name:",
                Location = new Point(20, 20),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10F)
            };

            var textBox = new TextBox
            {
                Location = new Point(20, 60),
                Size = new Size(340, 25),
                Font = new Font("Segoe UI", 10F)
            };

            var okButton = new Button
            {
                Text = "Start Learning!",
                Location = new Point(200, 100),
                Size = new Size(100, 30),
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            nameForm.Controls.AddRange(new Control[] { label, textBox, okButton });
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

            // Check for quiz commands
            if (await ProcessQuizCommands(message))
                return;

            // Simple response system (we'll enhance this later)
            var response = GetSimpleResponse(message);
            AddBotMessage(response);

            _activityLogger.LogActivity("Chat", "Bot response", "Simple response provided");
        }

        // Fix for CS1503: Argument 1: cannot convert from 'CybersecurityAwarenessBot.ActivityLogger' to 'CybersecurityAwarenessBot.SimpleActivityLogger?'

        // Update the constructor call for QuizForm to use a compatible type.
        // Assuming SimpleActivityLogger is a simplified version of ActivityLogger, we need to create an instance of SimpleActivityLogger and pass it.

        private async Task<bool> ProcessQuizCommands(string message)
        {
            var lowerMessage = message.ToLower();
            var quizKeywords = new[] { "quiz", "test", "questions", "challenge", "game", "start quiz" };

            if (quizKeywords.Any(keyword => lowerMessage.Contains(keyword)))
            {
                mainTabControl.SelectedTab = quizTabPage;
                AddBotMessage("Let's test your cybersecurity knowledge! I've switched you to the Quiz tab. Click the quiz button to begin!");

                _activityLogger.LogActivity("NLP", "Quiz command detected", message);

                if (lowerMessage.Contains("start quiz") || lowerMessage.Contains("begin quiz"))
                {
                    // Create a SimpleActivityLogger instance and pass it to QuizForm
                    var simpleActivityLogger = new SimpleActivityLogger();
                    var quizForm = new QuizForm(simpleActivityLogger);
                    quizForm.Show();
                }

                return true;
            }
            return false;
        }

        private string GetSimpleResponse(string message)
        {
            var lowerMessage = message.ToLower();

            if (lowerMessage.Contains("password"))
                return "Strong passwords should be at least 12 characters long with a mix of letters, numbers, and symbols!";

            if (lowerMessage.Contains("phishing"))
                return "Phishing emails try to steal your information. Never click suspicious links and always verify the sender!";

            if (lowerMessage.Contains("malware"))
                return "Malware can harm your computer. Keep your antivirus updated and avoid downloading from untrusted sources!";

            if (lowerMessage.Contains("vpn"))
                return "VPNs encrypt your internet traffic and protect your privacy, especially on public Wi-Fi networks!";

            return "That's a great cybersecurity question! I can help with passwords, phishing, malware, VPNs, and more. Try asking about specific topics!";
        }

        private void AddBotMessage(string message)
        {
            AddMessage("Bot", message, Color.Blue);

            // Speak message if TTS available
            if (_ttsHelper != null)
            {
                _ = Task.Run(async () => await _ttsHelper.SpeakAsync(message));
            }
        }

        private void AddUserMessage(string message)
        {
            AddMessage(_userName ?? "User", message, Color.DarkGreen);
        }

        private void AddSystemMessage(string message, Color color)
        {
            AddMessage("System", message, color);
        }

        private void AddMessage(string sender, string message, Color color)
        {
            if (chatDisplay.InvokeRequired)
            {
                chatDisplay.Invoke(new Action(() => AddMessage(sender, message, color)));
                return;
            }

            var timestamp = DateTime.Now.ToString("HH:mm:ss");

            chatDisplay.SelectionColor = Color.Gray;
            chatDisplay.AppendText($"[{timestamp}] ");
            chatDisplay.SelectionColor = color;
            chatDisplay.SelectionFont = new Font(chatDisplay.Font, FontStyle.Bold);
            chatDisplay.AppendText($"{sender}: ");
            chatDisplay.SelectionFont = new Font(chatDisplay.Font, FontStyle.Regular);
            chatDisplay.AppendText($"{message}\n\n");

            chatDisplay.SelectionStart = chatDisplay.Text.Length;
            chatDisplay.ScrollToCaret();
        }

        private void ClearChat()
        {
            chatDisplay.Clear();
            _activityLogger.LogActivity("Chat", "Chat cleared", "User cleared the chat history");
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _activityLogger.LogActivity("System", "Session ended", $"User {_userName} ended the session");
            _ttsHelper?.Dispose();
        }
    }

    /// <summary>
    /// Standard activity logger for the entire application.
    /// </summary>
    public class ActivityLogger
    {
        private readonly List<string> _activities = new List<string>();

        public void LogActivity(string category, string action, string details)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"[{timestamp}] {category}: {action} - {details}";
            _activities.Add(logEntry);
            Console.WriteLine(logEntry);
        }

        public void InitializeActivityInterface(Panel parentPanel)
        {
            parentPanel.Controls.Clear();

            var titleLabel = new Label
            {
                Text = "📊 Activity Log & Chat History",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            var activityList = new ListBox
            {
                Location = new Point(10, 50),
                Size = new Size(760, 400),
                Font = new Font("Consolas", 9F),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Populate with current activities
            foreach (var activity in _activities.TakeLast(50))
            {
                activityList.Items.Add(activity);
            }

            var instructionLabel = new Label
            {
                Text = "💡 This log tracks all your interactions: chat messages, tasks created/completed, quiz attempts, and NLP detections.",
                Location = new Point(10, 460),
                Size = new Size(760, 40),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            parentPanel.Controls.AddRange(new Control[] { titleLabel, activityList, instructionLabel });
        }

        public List<string> GetRecentActivities(int count = 50)
        {
            return _activities.TakeLast(count).ToList();
        }
    }
}

// End of file: MainForm.cs