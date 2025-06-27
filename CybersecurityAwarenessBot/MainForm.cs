using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;

namespace CybersecurityAwarenessBot
{
    public class ActivityLogger
    {
        public void LogActivity(string category, string action, string details)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {category}: {action} - {details}");
        }

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
                Text = "Task Manager - Coming Soon!",
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
        private readonly TaskManager _taskManager;
        private readonly QuizManager _quizManager;
        private readonly ActivityLogger _activityLogger;
        private readonly AudioPlayer _audioPlayer;
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
            _taskManager = new TaskManager();
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
        }

        private async void InitializeChat()
        {
            await Task.Delay(500);
            await GetUserName();
            _activityLogger.LogActivity("Chat", "Session started", $"User {_userName} started a new session");
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
        }

        private async Task SendMessage()
        {
            var message = chatInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(message))
                return;

            chatInput.Clear();
            AddMessageToChat(_userName ?? "User", message, Color.DarkGreen);
            _activityLogger.LogActivity("Chat", "User message", message);

            var response = _responseHandler.GetResponse(message);
            await AddBotResponseWithTyping(response);
            _activityLogger.LogActivity("Chat", "Bot response", $"Topic: {_responseHandler.LastTopic ?? "General"}");
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