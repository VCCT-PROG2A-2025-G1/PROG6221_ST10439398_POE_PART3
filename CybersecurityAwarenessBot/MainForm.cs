// Start of file: MainForm.cs
// Purpose: Main Windows Forms GUI for the Cybersecurity Awareness Bot
// Uses enhanced ResponseHandler with all console improvements
// Sources:
// - Windows Forms documentation: https://docs.microsoft.com/en-us/dotnet/desktop/winforms/
// - TabControl implementation: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.tabcontrol
// - RichTextBox for chat display: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.richtextbox

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using CybersecurityAwarenessBot;

namespace CybersecurityAwarenessBot
{
    public class ActivityLogger
    {
        public void LogActivity(string category, string action, string details)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {category}: {action} - {details}");
        }

        /// <summary>
        /// Enhanced ActivityLogger matching console version functionality
        /// </summary>
        public class ActivityLogger
        {
            private readonly List<string> _activities = new List<string>();

            public void LogActivity(string category, string action, string details)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var logEntry = $"[{timestamp}] {category}: {action} - {details}";
                _activities.Add(logEntry);
                Console.WriteLine(logEntry); // Also log to console like original
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
                foreach (var activity in _activities.TakeLast(50)) // Show last 50 activities
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
        }

        /// <summary>
        /// Enhanced QuizManager placeholder - will implement full version next
        /// </summary>
        public class QuizManager
        {
            public void InitializeQuizInterface(Panel parentPanel, ActivityLogger activityLogger)
            {
                parentPanel.Controls.Clear();

                var titleLabel = new Label
                {
                    Text = "🧠 Cybersecurity Knowledge Quiz",
                    Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 123, 255),
                    AutoSize = true,
                    Location = new Point(10, 10)
                };

                var comingSoonLabel = new Label
                {
                    Text = "📝 Advanced Quiz System - Implementation in Progress\n\n" +
                           "Features being implemented:\n" +
                           "• 15+ cybersecurity questions\n" +
                           "• True/false and multiple choice formats\n" +
                           "• Immediate feedback system\n" +
                           "• Score tracking and performance analysis\n" +
                           "• Question randomization\n\n" +
                           "This will be the next component implemented!",
                    Location = new Point(10, 50),
                    Size = new Size(700, 200),
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.FromArgb(73, 80, 87)
                };

                parentPanel.Controls.AddRange(new Control[] { titleLabel, comingSoonLabel });
            }
        }
}

// End of file: MainForm.cs

        public void InitializeActivityInterface(Panel parentPanel)
        {
            var label = new Label
            {
                Text = "Activity Logger - Coming Soon!",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.Gray
            };
            parentPanel.Controls.Add(label);
        }
    }

    public class TaskManager
{
    public void InitializeTaskInterface(Panel parentPanel, ActivityLogger activityLogger)
    {
        var label = new Label
        {
            Text = "Task Manager - Being Updated!",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12F),
            ForeColor = Color.Gray
        };
        parentPanel.Controls.Add(label);
    }

    public void PreFillTaskForm(string title, string message)
    {
        Console.WriteLine($"Task suggestion: {title}");
    }
}

public class QuizManager
{
    public void InitializeQuizInterface(Panel parentPanel, ActivityLogger activityLogger)
    {
        var label = new Label
        {
            Text = "Quiz Manager - Coming Soon!",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12F),
            ForeColor = Color.Gray
        };
        parentPanel.Controls.Add(label);
    }
}

public partial class MainForm : Form
{
    private readonly ResponseHandler _responseHandler;
    private readonly FullTaskManager _taskManager;
    private readonly QuizManager _quizManager;
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
        _responseHandler = new ResponseHandler();
        _taskManager = new FullTaskManager();
        _quizManager = new QuizManager();
        _activityLogger = new ActivityLogger();
        _audioPlayer = new AudioPlayer();

        InitializeComponent();
        SetupEventHandlers();
        InitializeChat();
    }

    private void InitializeComponent()
    {
        this.Text = "Cybersecurity Awareness Bot v3.0";
        this.Size = new Size(1000, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(800, 600);

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
            Height = 400,
            ReadOnly = true,
            BackColor = Color.FromArgb(240, 240, 240),
            Font = new Font("Segoe UI", 10F),
            ScrollBars = RichTextBoxScrollBars.Vertical
        };

        var inputPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 80,
            Padding = new Padding(10)
        };

        chatInput = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
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
            Height = 30,
            BackColor = Color.FromArgb(0, 123, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        clearChatButton = new Button
        {
            Text = "Clear",
            Dock = DockStyle.Bottom,
            Height = 30,
            BackColor = Color.FromArgb(108, 117, 125),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        buttonPanel.Controls.AddRange(new Control[] { sendButton, clearChatButton });
        inputPanel.Controls.AddRange(new Control[] { chatInput, buttonPanel });

        var welcomePanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };

        var welcomeLabel = new Label
        {
            Text = "🛡️ Welcome to Cybersecurity Awareness Bot v3.0!\nAsk me questions about cybersecurity!",
            Dock = DockStyle.Top,
            Height = 60,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(40, 167, 69),
            TextAlign = ContentAlignment.MiddleCenter
        };

        welcomePanel.Controls.Add(welcomeLabel);
        welcomePanel.Controls.Add(chatDisplay);

        chatTabPage.Controls.AddRange(new Control[] { inputPanel, welcomePanel });
    }

    private void InitializeOtherTabs()
    {
        var taskPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        tasksTabPage.Controls.Add(taskPanel);
        _taskManager.InitializeTaskInterface(taskPanel, _activityLogger);

        var quizPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        quizTabPage.Controls.Add(quizPanel);
        _quizManager.InitializeQuizInterface(quizPanel, _activityLogger);

        var activityPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        activityTabPage.Controls.Add(activityPanel);
        _activityLogger.InitializeActivityInterface(activityPanel);
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
            e.Handled = true; // Use Handled instead of SuppressKeyDown
            await SendMessage();
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _activityLogger.LogActivity("System", "Session ended", $"User {_userName} ended the session");
        _ttsHelper?.Dispose();
    }

    private async void InitializeChat()
    {
        // Play greeting audio first
        try
        {
            await _audioPlayer.PlayAsync("greeting.wav");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not play greeting: {ex.Message}");
            // Try alternative path
            try
            {
                await _audioPlayer.PlayAsync("Assets/greeting.wav");
            }
            catch
            {
                Console.WriteLine("No greeting audio file found. Continuing without audio.");
            }
        }

        await Task.Delay(500);
        await GetUserName();

        // Add welcome message to chat AND activity log
        _activityLogger.LogActivity("Chat", "Session started", $"User {_userName} started a new session");

        // Show the initial prompt that was missing
        var initialPrompt = "Ask me a question about cybersecurity, or try commands like:\n• 'add task to update passwords'\n• 'start quiz'\n• 'show activity'";
        AddMessageToChat("Bot", initialPrompt, Color.Blue);

        // Speak the initial prompt
        if (_ttsHelper != null)
        {
            try
            {
                await _ttsHelper.SpeakAsync("Ask me a question about cybersecurity, or tell me to add a task, start a quiz, or show your activity.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TTS error: {ex.Message}");
            }
        }
    }

    private async Task GetUserName()
    {
        using var nameForm = new Form
        {
            Text = "Welcome!",
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
            Text = "OK",
            Location = new Point(200, 100),
            Size = new Size(75, 30),
            DialogResult = DialogResult.OK,
            BackColor = Color.FromArgb(0, 123, 255),
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

        _responseHandler.SetUserName(_userName);
        var welcomeMessage = $"Hello, {_userName}! I'm here to help you learn about cybersecurity and stay safe online.";
        AddMessageToChat("Bot", welcomeMessage, Color.Blue);

        // Speak welcome message if TTS is available
        if (_ttsHelper != null)
        {
            try
            {
                await _ttsHelper.SpeakAsync(welcomeMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TTS error: {ex.Message}");
            }
        }
    }

    private async Task SendMessage()
    {
        var message = chatInput.Text.Trim();
        if (string.IsNullOrWhiteSpace(message))
            return;

        chatInput.Clear();
        AddMessageToChat(_userName ?? "User", message, Color.DarkGreen);
        _activityLogger.LogActivity("Chat", "User message", message);

        // Check for task-related commands using NLP
        if (await ProcessTaskCommands(message))
            return;

        // Check for quiz commands
        if (await ProcessQuizCommands(message))
            return;

        // Check for activity log commands
        if (await ProcessActivityCommands(message))
            return;

        var response = _responseHandler.GetResponse(message);
        await AddBotResponseWithTyping(response);
        _activityLogger.LogActivity("Chat", "Bot response", $"Topic: {_responseHandler.LastTopic ?? "General"}");

        // Handle follow-up questions
        if (_responseHandler.LastTopic != null && _responseHandler.LastTopic != "gratitude")
        {
            await Task.Delay(1000);
            var followUpQuestion = $"Would you like to know more about {_responseHandler.LastTopic}?";
            AddMessageToChat("Bot", followUpQuestion, Color.Blue);

            if (_ttsHelper != null)
            {
                try
                {
                    await _ttsHelper.SpeakAsync(followUpQuestion);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TTS error: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Processes task-related commands using NLP techniques.
    /// </summary>
    private async Task<bool> ProcessTaskCommands(string message)
    {
        var lowerMessage = message.ToLower();
        var taskKeywords = new[] { "add task", "create task", "new task", "remind me", "set reminder", "task for", "don't forget" };
        var isTaskCommand = taskKeywords.Any(keyword => lowerMessage.Contains(keyword));

        if (isTaskCommand)
        {
            mainTabControl.SelectedTab = tasksTabPage;
            var taskTitle = ExtractTaskTitle(message);
            if (!string.IsNullOrEmpty(taskTitle))
            {
                _taskManager.PreFillTaskForm(taskTitle, message);
            }
            AddMessageToChat("Bot", "I've switched you to the Tasks tab where you can add your task!", Color.Blue);
            _activityLogger.LogActivity("NLP", "Task command detected", message);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Extracts task title from natural language input.
    /// </summary>
    private string ExtractTaskTitle(string message)
    {
        var lowerMessage = message.ToLower();
        var patterns = new[] { "remind me to ", "add task ", "create task ", "new task ", "don't forget to ", "task for " };

        foreach (var pattern in patterns)
        {
            var index = lowerMessage.IndexOf(pattern);
            if (index >= 0)
            {
                var startIndex = index + pattern.Length;
                var remaining = message.Substring(startIndex);
                var endings = new[] { " tomorrow", " today", " next week", " on ", " at ", " by " };

                foreach (var ending in endings)
                {
                    var endIndex = remaining.ToLower().IndexOf(ending);
                    if (endIndex > 0)
                    {
                        return remaining.Substring(0, endIndex).Trim();
                    }
                }
                return remaining.Trim();
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// Processes quiz-related commands.
    /// </summary>
    private async Task<bool> ProcessQuizCommands(string message)
    {
        var lowerMessage = message.ToLower();
        var quizKeywords = new[] { "quiz", "test", "questions", "challenge", "game" };

        if (quizKeywords.Any(keyword => lowerMessage.Contains(keyword)))
        {
            mainTabControl.SelectedTab = quizTabPage;
            AddMessageToChat("Bot", "Let's test your cybersecurity knowledge! I've switched you to the Quiz tab.", Color.Blue);
            _activityLogger.LogActivity("NLP", "Quiz command detected", message);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Processes activity log related commands.
    /// </summary>
    private async Task<bool> ProcessActivityCommands(string message)
    {
        var lowerMessage = message.ToLower();
        var activityKeywords = new[] { "activity", "history", "log", "what have i done", "show activity" };

        if (activityKeywords.Any(keyword => lowerMessage.Contains(keyword)))
        {
            mainTabControl.SelectedTab = activityTabPage;
            AddMessageToChat("Bot", "Here's your activity history! I've switched you to the Activity Log tab.", Color.Blue);
            _activityLogger.LogActivity("NLP", "Activity command detected", message);
            return true;
        }
        return false;
    }

    private void AddMessageToChat(string sender, string message, Color color)
    {
        if (chatDisplay.InvokeRequired)
        {
            chatDisplay.Invoke(new Action(() => AddMessageToChat(sender, message, color)));
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

    private async Task AddBotResponseWithTyping(string response)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");

        if (chatDisplay.InvokeRequired)
        {
            chatDisplay.Invoke(new Action(() => AddBotResponseWithTypingSync(response, timestamp)));
            return;
        }

        AddBotResponseWithTypingSync(response, timestamp);
    }

    private async void AddBotResponseWithTypingSync(string response, string timestamp)
    {
        chatDisplay.SelectionColor = Color.Gray;
        chatDisplay.AppendText($"[{timestamp}] ");
        chatDisplay.SelectionColor = Color.Blue;
        chatDisplay.SelectionFont = new Font(chatDisplay.Font, FontStyle.Bold);
        chatDisplay.AppendText("Bot: ");

        chatDisplay.SelectionFont = new Font(chatDisplay.Font, FontStyle.Regular);

        // Start TTS in background while typing
        if (_ttsHelper != null)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(500); // Small delay to sync with typing
                    await _ttsHelper.SpeakAsync(response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TTS error: {ex.Message}");
                }
            });
        }

        foreach (char c in response)
        {
            chatDisplay.AppendText(c.ToString());
            chatDisplay.SelectionStart = chatDisplay.Text.Length;
            chatDisplay.ScrollToCaret();
            await Task.Delay(30);
        }

        chatDisplay.AppendText("\n\n");
        chatDisplay.SelectionStart = chatDisplay.Text.Length;
        chatDisplay.ScrollToCaret();
    }

    private void ClearChat()
    {
        chatDisplay.Clear();
        _activityLogger.LogActivity("Chat", "Chat cleared", "User cleared the chat history");
    }
}
}