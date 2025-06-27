// Start of file: QuizForm.cs
// Purpose: Complete cybersecurity quiz system with immediate feedback and scoring
// Implements Part 3 requirements: 15+ questions, true/false & multiple choice, immediate feedback, score tracking
// Sources:
// - RadioButton implementation: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.radiobutton
// - ProgressBar usage: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.progressbar
// - Quiz design patterns: https://en.wikipedia.org/wiki/Quiz
// - Cybersecurity knowledge base: https://www.cisa.gov/cybersecurity-best-practices

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Complete cybersecurity quiz form with scoring and feedback system.
    /// </summary>
    public partial class QuizForm : Form
    {
        private readonly List<QuizQuestion> _questions;
        private readonly List<QuizAttempt> _attempts;
        private readonly Random _random;
        private int _currentQuestionIndex;
        private int _currentScore;
        private DateTime _quizStartTime;
        private readonly SimpleActivityLogger? _activityLogger;

        // UI Controls
        private Label titleLabel = null!;
        private Label questionCounterLabel = null!;
        private ProgressBar progressBar = null!;
        private Panel questionPanel = null!;
        private Label questionLabel = null!;
        private Panel answersPanel = null!;
        private Button submitAnswerButton = null!;
        private Button nextQuestionButton = null!;
        private Panel feedbackPanel = null!;
        private Label feedbackLabel = null!;
        private Label scoreLabel = null!;
        private Button startQuizButton = null!;
        private Button restartQuizButton = null!;
        private Panel resultsPanel = null!;
        private ActivityLogger activityLogger;

        public QuizForm(SimpleActivityLogger? activityLogger = null)
        {
            _questions = CreateCybersecurityQuestions();
            _attempts = new List<QuizAttempt>();
            _random = new Random();
            _currentQuestionIndex = 0;
            _currentScore = 0;
            _activityLogger = activityLogger;

            InitializeComponent();
            SetupEventHandlers();
        }

        public QuizForm(ActivityLogger activityLogger)
        {
            this.activityLogger = activityLogger;
        }

        /// <summary>
        /// Creates comprehensive cybersecurity quiz questions.
        /// Source: CISA Cybersecurity Best Practices https://www.cisa.gov/cybersecurity-best-practices
        /// Source: NIST Cybersecurity Framework https://www.nist.gov/cyberframework
        /// </summary>
        private List<QuizQuestion> CreateCybersecurityQuestions()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Id = 1,
                    Text = "What is the minimum recommended length for a strong password?",
                    Type = QuestionType.MultipleChoice,
                    Options = new[] { "6 characters", "8 characters", "12 characters", "16 characters" },
                    CorrectAnswer = 2, // 12 characters
                    Explanation = "Cybersecurity experts recommend passwords of at least 12 characters for optimal security. Longer passwords are exponentially harder to crack."
                },
                new QuizQuestion
                {
                    Id = 2,
                    Text = "True or False: It's safe to use the same password for multiple accounts if it's very strong.",
                    Type = QuestionType.TrueFalse,
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1, // False
                    Explanation = "Never reuse passwords! If one account gets compromised, all your accounts using that password become vulnerable. Use unique passwords for each account."
                },
                new QuizQuestion
                {
                    Id = 3,
                    Text = "What does 'phishing' refer to in cybersecurity?",
                    Type = QuestionType.MultipleChoice,
                    Options = new[] {
                        "Catching fish online",
                        "Fraudulent emails trying to steal information",
                        "A type of computer virus",
                        "Fishing for compliments on social media"
                    },
                    CorrectAnswer = 1,
                    Explanation = "Phishing is a social engineering attack where cybercriminals send fraudulent emails that appear to be from reputable sources to steal sensitive information."
                },
                new QuizQuestion
                {
                    Id = 4,
                    Text = "True or False: Public Wi-Fi networks are always safe to use for online banking.",
                    Type = QuestionType.TrueFalse,
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1, // False
                    Explanation = "Public Wi-Fi networks are inherently insecure. Never conduct sensitive activities like online banking on public Wi-Fi. Use a VPN if necessary."
                },
                new QuizQuestion
                {
                    Id = 5,
                    Text = "What does 'two-factor authentication' (2FA) provide?",
                    Type = QuestionType.MultipleChoice,
                    Options = new[] {
                        "Double the password strength",
                        "An extra layer of security beyond passwords",
                        "Two different usernames",
                        "Automatic password generation"
                    },
                    CorrectAnswer = 1,
                    Explanation = "Two-factor authentication adds an extra security layer by requiring a second form of verification (like a phone code) in addition to your password."
                },
                new QuizQuestion
                {
                    Id = 6,
                    Text = "True or False: Antivirus software alone is sufficient to protect against all cyber threats.",
                    Type = QuestionType.TrueFalse,
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1, // False
                    Explanation = "Antivirus is important but not sufficient alone. You need multiple layers: firewalls, safe browsing habits, software updates, and user awareness."
                },
                new QuizQuestion
                {
                    Id = 7,
                    Text = "What should you do if you receive an email asking for your password?",
                    Type = QuestionType.MultipleChoice,
                    Options = new[] {
                        "Reply with your password immediately",
                        "Forward it to friends for advice",
                        "Delete it and report as phishing",
                        "Change your password first, then reply"
                    },
                    CorrectAnswer = 2,
                    Explanation = "Legitimate organizations never ask for passwords via email. Always delete such emails and report them as phishing attempts."
                },
                new QuizQuestion
                {
                    Id = 8,
                    Text = "True or False: Software updates are only about adding new features.",
                    Type = QuestionType.TrueFalse,
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1, // False
                    Explanation = "Software updates often include critical security patches that fix vulnerabilities. Delaying updates leaves your system exposed to known threats."
                },
                new QuizQuestion
                {
                    Id = 9,
                    Text = "What is 'social engineering' in cybersecurity?",
                    Type = QuestionType.MultipleChoice,
                    Options = new[] {
                        "Building social media platforms",
                        "Manipulating people to reveal confidential information",
                        "Engineering social robots",
                        "Creating user-friendly interfaces"
                    },
                    CorrectAnswer = 1,
                    Explanation = "Social engineering exploits human psychology to manipulate people into divulging confidential information or performing actions that compromise security."
                },
                new QuizQuestion
                {
                    Id = 10,
                    Text = "True or False: It's safe to click on links in text messages from unknown numbers.",
                    Type = QuestionType.TrueFalse,
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1, // False
                    Explanation = "Never click links from unknown sources. These could lead to malicious websites that steal information or install malware on your device."
                },
                new QuizQuestion
                {
                    Id = 11,
                    Text = "What is the primary purpose of a firewall?",
                    Type = QuestionType.MultipleChoice,
                    Options = new[] {
                        "To prevent fires in computers",
                        "To monitor and control network traffic",
                        "To increase internet speed",
                        "To backup files automatically"
                    },
                    CorrectAnswer = 1,
                    Explanation = "A firewall acts as a barrier between trusted internal networks and untrusted external networks, monitoring and controlling incoming and outgoing traffic."
                },
                new QuizQuestion
                {
                    Id = 12,
                    Text = "True or False: Using a VPN makes you completely anonymous online.",
                    Type = QuestionType.TrueFalse,
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1, // False
                    Explanation = "While VPNs significantly improve privacy by encrypting traffic and masking IP addresses, they don't provide complete anonymity. Other tracking methods still exist."
                },
                new QuizQuestion
                {
                    Id = 13,
                    Text = "What should you do before disposing of an old computer?",
                    Type = QuestionType.MultipleChoice,
                    Options = new[] {
                        "Just delete all files",
                        "Remove the hard drive and keep it",
                        "Securely wipe all data multiple times",
                        "Nothing, it's already safe"
                    },
                    CorrectAnswer = 2,
                    Explanation = "Simply deleting files doesn't permanently remove them. Use secure wiping tools that overwrite data multiple times to prevent recovery of sensitive information."
                },
                new QuizQuestion
                {
                    Id = 14,
                    Text = "True or False: Backing up data is only necessary for businesses, not personal users.",
                    Type = QuestionType.TrueFalse,
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1, // False
                    Explanation = "Everyone should backup important data! Ransomware, hardware failures, and accidents can happen to anyone. Follow the 3-2-1 backup rule for optimal protection."
                },
                new QuizQuestion
                {
                    Id = 15,
                    Text = "What is 'ransomware'?",
                    Type = QuestionType.MultipleChoice,
                    Options = new[] {
                        "Software that demands payment to unlock your files",
                        "Free software available for ransom",
                        "A type of computer game",
                        "Software that protects against viruses"
                    },
                    CorrectAnswer = 0,
                    Explanation = "Ransomware encrypts your files and demands payment for the decryption key. The best defense is regular backups and avoiding suspicious downloads/emails."
                },
                new QuizQuestion
                {
                    Id = 16,
                    Text = "What does HTTPS stand for?",
                    Type = QuestionType.MultipleChoice,
                    Options = new[] {
                        "HyperText Transfer Protocol Secure",
                        "High Tech Transfer Protocol System",
                        "Home Transfer Text Protocol Safe",
                        "HyperText Transmission Protection Standard"
                    },
                    CorrectAnswer = 0,
                    Explanation = "HTTPS (HyperText Transfer Protocol Secure) encrypts data between your browser and websites, providing secure communication over the internet."
                },
                new QuizQuestion
                {
                    Id = 17,
                    Text = "True or False: It's safe to use public computers for online banking if you log out properly.",
                    Type = QuestionType.TrueFalse,
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1, // False
                    Explanation = "Public computers may have keyloggers or malware installed. Never use them for sensitive activities like banking, even if you log out properly."
                },
                new QuizQuestion
                {
                    Id = 18,
                    Text = "What is the best way to verify a suspicious email claiming to be from your bank?",
                    Type = QuestionType.MultipleChoice,
                    Options = new[] {
                        "Click the links to check",
                        "Reply to the email asking for verification",
                        "Call your bank using the official phone number",
                        "Forward it to friends for their opinion"
                    },
                    CorrectAnswer = 2,
                    Explanation = "Always verify suspicious communications by contacting the organization directly using official contact information, not the information provided in the suspicious message."
                }
            };
        }

        /// <summary>
        /// Initializes all UI components for the quiz form.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "🧠 Cybersecurity Knowledge Quiz";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);
            this.BackColor = Color.FromArgb(248, 249, 250);

            CreateHeaderSection();
            CreateQuestionSection();
            CreateControlSection();
            CreateResultsSection();
        }

        /// <summary>
        /// Creates the header section with title and progress indicators.
        /// </summary>
        private void CreateHeaderSection()
        {
            // Title
            titleLabel = new Label
            {
                Text = "🧠 Cybersecurity Knowledge Quiz",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var instructionLabel = new Label
            {
                Text = "Test your cybersecurity knowledge! Answer all questions to see your results.",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(20, 55)
            };

            // Progress section
            questionCounterLabel = new Label
            {
                Text = $"Question 1 of {_questions.Count}",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 90),
                Size = new Size(200, 25)
            };

            progressBar = new ProgressBar
            {
                Location = new Point(230, 92),
                Size = new Size(400, 20),
                Minimum = 0,
                Maximum = _questions.Count,
                Value = 0,
                Style = ProgressBarStyle.Continuous
            };

            scoreLabel = new Label
            {
                Text = "Score: 0/0",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(650, 90),
                Size = new Size(100, 25)
            };

            this.Controls.AddRange(new Control[] {
                titleLabel, instructionLabel, questionCounterLabel, progressBar, scoreLabel
            });
        }

        /// <summary>
        /// Creates the main question display section.
        /// </summary>
        private void CreateQuestionSection()
        {
            questionPanel = new Panel
            {
                Location = new Point(20, 130),
                Size = new Size(840, 400),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Visible = false
            };

            questionLabel = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(800, 80),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            answersPanel = new Panel
            {
                Location = new Point(20, 110),
                Size = new Size(800, 200),
                AutoScroll = true
            };

            submitAnswerButton = new Button
            {
                Text = "Submit Answer",
                Location = new Point(20, 320),
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Enabled = false
            };

            nextQuestionButton = new Button
            {
                Text = "Next Question",
                Location = new Point(160, 320),
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Visible = false
            };

            // Feedback panel
            feedbackPanel = new Panel
            {
                Location = new Point(310, 315),
                Size = new Size(510, 50),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            feedbackLabel = new Label
            {
                Location = new Point(15, 15),
                Size = new Size(480, 20),
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleLeft
            };

            feedbackPanel.Controls.Add(feedbackLabel);

            questionPanel.Controls.AddRange(new Control[] {
                questionLabel, answersPanel, submitAnswerButton, nextQuestionButton, feedbackPanel
            });

            this.Controls.Add(questionPanel);
        }

        /// <summary>
        /// Creates the control section with start/restart buttons.
        /// </summary>
        private void CreateControlSection()
        {
            startQuizButton = new Button
            {
                Text = "🚀 Start Quiz",
                Location = new Point(20, 550),
                Size = new Size(150, 45),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };

            restartQuizButton = new Button
            {
                Text = "🔄 Restart Quiz",
                Location = new Point(190, 550),
                Size = new Size(150, 45),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Visible = false
            };

            this.Controls.AddRange(new Control[] { startQuizButton, restartQuizButton });
        }

        /// <summary>
        /// Creates the results display section.
        /// </summary>
        private void CreateResultsSection()
        {
            resultsPanel = new Panel
            {
                Location = new Point(360, 540),
                Size = new Size(500, 60),
                Visible = false
            };

            this.Controls.Add(resultsPanel);
        }

        /// <summary>
        /// Sets up all event handlers.
        /// </summary>
        private void SetupEventHandlers()
        {
            startQuizButton.Click += StartQuiz_Click;
            restartQuizButton.Click += RestartQuiz_Click;
            submitAnswerButton.Click += SubmitAnswer_Click;
            nextQuestionButton.Click += NextQuestion_Click;
        }

        /// <summary>
        /// Starts a new quiz session.
        /// </summary>
        private void StartQuiz_Click(object? sender, EventArgs e)
        {
            _currentQuestionIndex = 0;
            _currentScore = 0;
            _quizStartTime = DateTime.Now;

            // Shuffle questions for variety
            // Source: Fisher-Yates shuffle algorithm https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
            for (int i = _questions.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (_questions[i], _questions[j]) = (_questions[j], _questions[i]);
            }

            startQuizButton.Visible = false;
            questionPanel.Visible = true;
            restartQuizButton.Visible = true;
            resultsPanel.Visible = false;

            ShowCurrentQuestion();

            _activityLogger?.LogActivity("Quiz", "Quiz started", $"Started quiz with {_questions.Count} questions");
        }

        /// <summary>
        /// Restarts the quiz.
        /// </summary>
        private void RestartQuiz_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to restart the quiz? Current progress will be lost.",
                "Restart Quiz", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                StartQuiz_Click(sender, e);
            }
        }

        /// <summary>
        /// Displays the current question with options.
        /// Source: Dynamic RadioButton creation https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.radiobutton
        /// </summary>
        private void ShowCurrentQuestion()
        {
            if (_currentQuestionIndex >= _questions.Count)
            {
                ShowQuizResults();
                return;
            }

            var question = _questions[_currentQuestionIndex];

            // Update progress
            questionCounterLabel.Text = $"Question {_currentQuestionIndex + 1} of {_questions.Count}";
            progressBar.Value = _currentQuestionIndex;
            scoreLabel.Text = $"Score: {_currentScore}/{_currentQuestionIndex}";

            // Display question
            questionLabel.Text = $"Q{_currentQuestionIndex + 1}: {question.Text}";

            // Clear previous answers
            answersPanel.Controls.Clear();

            // Create answer options
            for (int i = 0; i < question.Options.Length; i++)
            {
                var radioButton = new RadioButton
                {
                    Text = question.Options[i],
                    Location = new Point(20, i * 40 + 20),
                    Size = new Size(750, 35),
                    Font = new Font("Segoe UI", 11F),
                    Tag = i,
                    ForeColor = Color.FromArgb(73, 80, 87)
                };

                radioButton.CheckedChanged += (s, e) =>
                {
                    submitAnswerButton.Enabled = answersPanel.Controls.OfType<RadioButton>().Any(rb => rb.Checked);
                };

                answersPanel.Controls.Add(radioButton);
            }

            // Reset button states
            submitAnswerButton.Enabled = false;
            submitAnswerButton.Visible = true;
            nextQuestionButton.Visible = false;
            feedbackPanel.Visible = false;
        }

        /// <summary>
        /// Submits the current answer and shows feedback.
        /// </summary>
        private void SubmitAnswer_Click(object? sender, EventArgs e)
        {
            var selectedOption = answersPanel.Controls.OfType<RadioButton>()
                .FirstOrDefault(rb => rb.Checked);

            if (selectedOption == null) return;

            var question = _questions[_currentQuestionIndex];
            var selectedIndex = (int)selectedOption.Tag!;
            var isCorrect = selectedIndex == question.CorrectAnswer;

            if (isCorrect)
            {
                _currentScore++;
                feedbackLabel.Text = "✅ Correct! " + question.Explanation;
                feedbackLabel.ForeColor = Color.FromArgb(40, 167, 69);
                feedbackPanel.BackColor = Color.FromArgb(212, 237, 218);
            }
            else
            {
                feedbackLabel.Text = "❌ Incorrect. " + question.Explanation;
                feedbackLabel.ForeColor = Color.FromArgb(220, 53, 69);
                feedbackPanel.BackColor = Color.FromArgb(248, 215, 218);

                // Highlight correct answer
                var correctOption = answersPanel.Controls.OfType<RadioButton>()
                    .FirstOrDefault(rb => (int)rb.Tag! == question.CorrectAnswer);
                if (correctOption != null)
                {
                    correctOption.ForeColor = Color.FromArgb(40, 167, 69);
                    correctOption.Font = new Font(correctOption.Font, FontStyle.Bold);
                }
            }

            // Disable all options and show feedback
            foreach (RadioButton rb in answersPanel.Controls.OfType<RadioButton>())
            {
                rb.Enabled = false;
            }

            submitAnswerButton.Visible = false;
            nextQuestionButton.Visible = true;
            feedbackPanel.Visible = true;

            // Update score display
            scoreLabel.Text = $"Score: {_currentScore}/{_currentQuestionIndex + 1}";

            // Log the answer
            _activityLogger?.LogActivity("Quiz", "Question answered",
                $"Q{_currentQuestionIndex + 1}: {(isCorrect ? "Correct" : "Incorrect")}");
        }

        /// <summary>
        /// Moves to the next question.
        /// </summary>
        private void NextQuestion_Click(object? sender, EventArgs e)
        {
            _currentQuestionIndex++;
            ShowCurrentQuestion();
        }

        /// <summary>
        /// Shows the final quiz results with performance analysis.
        /// Source: Performance analysis algorithms https://en.wikipedia.org/wiki/Performance_indicator
        /// </summary>
        private void ShowQuizResults()
        {
            questionPanel.Visible = false;
            progressBar.Value = _questions.Count;
            questionCounterLabel.Text = "Quiz Complete!";
            scoreLabel.Text = $"Final Score: {_currentScore}/{_questions.Count}";

            // Calculate performance metrics
            var percentage = Math.Round((_currentScore / (double)_questions.Count) * 100, 1);
            var timeTaken = DateTime.Now - _quizStartTime;

            // Create detailed results
            resultsPanel.Controls.Clear();
            resultsPanel.Visible = true;
            resultsPanel.Size = new Size(500, 120);

            var resultsTitle = new Label
            {
                Text = "🎯 Quiz Results",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var scoreText = new Label
            {
                Text = $"Score: {_currentScore}/{_questions.Count} ({percentage}%)",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(10, 40),
                AutoSize = true
            };

            var timeText = new Label
            {
                Text = $"Time: {timeTaken:mm\\:ss}",
                Font = new Font("Segoe UI", 11F),
                Location = new Point(10, 70),
                AutoSize = true
            };

            var performanceText = new Label
            {
                Location = new Point(10, 90),
                Size = new Size(480, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };

            // Performance feedback based on score
            if (percentage >= 90)
            {
                performanceText.Text = "🌟 Excellent! You're a cybersecurity expert!";
                performanceText.ForeColor = Color.FromArgb(40, 167, 69);
                scoreText.ForeColor = Color.FromArgb(40, 167, 69);
            }
            else if (percentage >= 80)
            {
                performanceText.Text = "👍 Great job! You have strong cybersecurity knowledge.";
                performanceText.ForeColor = Color.FromArgb(0, 123, 255);
                scoreText.ForeColor = Color.FromArgb(0, 123, 255);
            }
            else if (percentage >= 70)
            {
                performanceText.Text = "📚 Good work! Consider reviewing some topics.";
                performanceText.ForeColor = Color.FromArgb(255, 193, 7);
                scoreText.ForeColor = Color.FromArgb(255, 193, 7);
            }
            else
            {
                performanceText.Text = "📖 Keep learning! Cybersecurity knowledge is crucial.";
                performanceText.ForeColor = Color.FromArgb(220, 53, 69);
                scoreText.ForeColor = Color.FromArgb(220, 53, 69);
            }

            resultsPanel.Controls.AddRange(new Control[] {
                resultsTitle, scoreText, timeText, performanceText
            });

            // Record quiz attempt
            var attempt = new QuizAttempt
            {
                Date = DateTime.Now,
                Score = _currentScore,
                TotalQuestions = _questions.Count,
                TimeTaken = timeTaken,
                Percentage = percentage
            };
            _attempts.Add(attempt);

            // Log quiz completion
            _activityLogger?.LogActivity("Quiz", "Quiz completed",
                $"Score: {_currentScore}/{_questions.Count} ({percentage}%) in {timeTaken:mm\\:ss}");

            // Show congratulations message
            MessageBox.Show($"Quiz completed!\n\nScore: {_currentScore}/{_questions.Count} ({percentage}%)\nTime: {timeTaken:mm\\:ss}\n\n{performanceText.Text}",
                "Quiz Results", MessageBoxButtons.OK,
                percentage >= 80 ? MessageBoxIcon.Information : MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Gets quiz statistics for reporting.
        /// </summary>
        public QuizStatistics GetQuizStatistics()
        {
            if (!_attempts.Any())
            {
                return new QuizStatistics();
            }

            return new QuizStatistics
            {
                TotalAttempts = _attempts.Count,
                AverageScore = _attempts.Average(a => a.Percentage),
                BestScore = _attempts.Max(a => a.Percentage),
                LastAttemptDate = _attempts.Max(a => a.Date),
                AverageTime = TimeSpan.FromTicks((long)_attempts.Average(a => a.TimeTaken.Ticks))
            };
        }
    }

    /// <summary>
    /// Represents a quiz question with multiple choice or true/false format.
    /// </summary>
    public class QuizQuestion
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public string[] Options { get; set; } = Array.Empty<string>();
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }

    /// <summary>
    /// Quiz question types.
    /// </summary>
    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse
    }

    /// <summary>
    /// Records a quiz attempt for statistics.
    /// </summary>
    public class QuizAttempt
    {
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public double Percentage { get; set; }
    }

    /// <summary>
    /// Quiz performance statistics.
    /// </summary>
    public class QuizStatistics
    {
        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
        public double BestScore { get; set; }
        public DateTime LastAttemptDate { get; set; }
        public TimeSpan AverageTime { get; set; }
    }

    /// <summary>
    /// Simple activity logger interface for quiz integration.
    /// </summary>
    public class SimpleActivityLogger
    {
        public void LogActivity(string category, string action, string details)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"[{timestamp}] {category}: {action} - {details}";
            Console.WriteLine(logEntry);
        }
    }
}

// End of file: QuizForm.cs