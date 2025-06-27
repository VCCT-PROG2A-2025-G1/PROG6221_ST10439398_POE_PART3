// Start of file: TaskManager.cs
// Purpose: Complete task management system for cybersecurity-related tasks
// Implements Part 3 requirements: Task creation, display, completion, deletion, reminders
// Sources:
// - ListView implementation: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.listview
// - DateTimePicker usage: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.datetimepicker
// - Timer for reminders: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.timer
// - JSON serialization: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using WinFormsTimer = System.Windows.Forms.Timer;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Complete task management system with reminders and completion tracking.
    /// </summary>
    public class FullTaskManager
    {
        private readonly List<CyberTask> _tasks;
        private readonly WinFormsTimer _reminderTimer;
        private readonly string _dataFile = "tasks.json";
        private ActivityLogger? _activityLogger;

        // UI Controls
        private ListView taskListView = null!;
        private TextBox titleTextBox = null!;
        private TextBox descriptionTextBox = null!;
        private DateTimePicker reminderDatePicker = null!;
        private DateTimePicker reminderTimePicker = null!;
        private Button addTaskButton = null!;
        private Button deleteTaskButton = null!;
        private Button markCompleteButton = null!;
        private Label statusLabel = null!;

        public FullTaskManager()
        {
            _tasks = new List<CyberTask>();

            // Set up reminder timer to check every minute
            // Source: Timer implementation https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.timer
            _reminderTimer = new WinFormsTimer
            {
                Interval = 60000, // 1 minute
                Enabled = true
            };
            _reminderTimer.Tick += CheckReminders;

            LoadTasks();
        }

        /// <summary>
        /// Initializes the complete task management interface.
        /// Source: Dynamic control creation https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-add-controls-to-windows-forms
        /// </summary>
        public void InitializeTaskInterface(Panel parentPanel, ActivityLogger activityLogger)
        {
            _activityLogger = activityLogger;

            // Clear placeholder content
            parentPanel.Controls.Clear();

            // Title
            var titleLabel = new Label
            {
                Text = "📋 Cybersecurity Task Manager",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            // Task input section
            var inputGroupBox = new GroupBox
            {
                Text = "Add New Task",
                Location = new Point(10, 50),
                Size = new Size(600, 200),
                Font = new Font("Segoe UI", 10F)
            };

            // Title input
            var titleInputLabel = new Label
            {
                Text = "Title:",
                Location = new Point(10, 30),
                AutoSize = true
            };

            titleTextBox = new TextBox
            {
                Location = new Point(10, 50),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 9F)
            };

            // Description input
            var descriptionLabel = new Label
            {
                Text = "Description:",
                Location = new Point(10, 85),
                AutoSize = true
            };

            descriptionTextBox = new TextBox
            {
                Location = new Point(10, 105),
                Size = new Size(400, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 9F)
            };

            // Reminder date/time
            var reminderLabel = new Label
            {
                Text = "Reminder Date & Time:",
                Location = new Point(420, 30),
                AutoSize = true
            };

            reminderDatePicker = new DateTimePicker
            {
                Location = new Point(420, 50),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today
            };

            reminderTimePicker = new DateTimePicker
            {
                Location = new Point(420, 80),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true
            };

            addTaskButton = new Button
            {
                Text = "Add Task",
                Location = new Point(420, 120),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            // Status label
            statusLabel = new Label
            {
                Location = new Point(420, 160),
                Size = new Size(150, 30),
                ForeColor = Color.Green,
                Font = new Font("Segoe UI", 8F)
            };

            inputGroupBox.Controls.AddRange(new Control[] {
                titleInputLabel, titleTextBox, descriptionLabel, descriptionTextBox,
                reminderLabel, reminderDatePicker, reminderTimePicker, addTaskButton, statusLabel
            });

            // Task list section
            var listGroupBox = new GroupBox
            {
                Text = "Your Tasks",
                Location = new Point(10, 260),
                Size = new Size(750, 300),
                Font = new Font("Segoe UI", 10F)
            };

            // Task ListView with cybersecurity-themed columns
            // Source: ListView columns https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.listview.columns
            taskListView = new ListView
            {
                Location = new Point(10, 25),
                Size = new Size(600, 220),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9F)
            };

            taskListView.Columns.AddRange(new ColumnHeader[]
            {
                new ColumnHeader { Text = "Title", Width = 200 },
                new ColumnHeader { Text = "Description", Width = 250 },
                new ColumnHeader { Text = "Reminder", Width = 120 },
                new ColumnHeader { Text = "Status", Width = 80 }
            });

            // Action buttons
            markCompleteButton = new Button
            {
                Text = "Mark Complete",
                Location = new Point(620, 25),
                Size = new Size(110, 30),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };

            deleteTaskButton = new Button
            {
                Text = "Delete Task",
                Location = new Point(620, 65),
                Size = new Size(110, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };

            var suggestionsLabel = new Label
            {
                Text = "💡 Suggested cybersecurity tasks:\n• Review account passwords\n• Check software updates\n• Enable 2FA on accounts\n• Review privacy settings\n• Backup important data",
                Location = new Point(620, 105),
                Size = new Size(120, 140),
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            listGroupBox.Controls.AddRange(new Control[] {
                taskListView, markCompleteButton, deleteTaskButton, suggestionsLabel
            });

            parentPanel.Controls.AddRange(new Control[] {
                titleLabel, inputGroupBox, listGroupBox
            });

            SetupTaskEventHandlers();
            RefreshTaskList();
        }

        /// <summary>
        /// Sets up event handlers for task management controls.
        /// </summary>
        private void SetupTaskEventHandlers()
        {
            addTaskButton.Click += AddTask_Click;
            markCompleteButton.Click += MarkComplete_Click;
            deleteTaskButton.Click += DeleteTask_Click;

            // Double-click to edit task
            taskListView.DoubleClick += EditTask_DoubleClick;

            // Enter key in title textbox adds task
            titleTextBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    AddTask_Click(s, e);
                }
            };
        }

        /// <summary>
        /// Adds a new cybersecurity task.
        /// Source: Event handling patterns https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/events-in-windows-forms-controls
        /// </summary>
        private void AddTask_Click(object? sender, EventArgs e)
        {
            var title = titleTextBox.Text.Trim();
            var description = descriptionTextBox.Text.Trim();

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a task title.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Combine date and time for reminder
            var reminderDate = reminderDatePicker.Value.Date;
            var reminderTime = reminderTimePicker.Value.TimeOfDay;
            var reminderDateTime = reminderDate.Add(reminderTime);

            if (reminderDateTime <= DateTime.Now)
            {
                var result = MessageBox.Show("The reminder time is in the past. Set it anyway?",
                    "Past Reminder", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return;
            }

            var newTask = new CyberTask
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                ReminderDateTime = reminderDateTime,
                CreatedDate = DateTime.Now,
                IsCompleted = false
            };

            _tasks.Add(newTask);
            SaveTasks();
            RefreshTaskList();

            // Clear input fields
            titleTextBox.Clear();
            descriptionTextBox.Clear();
            reminderDatePicker.Value = DateTime.Today.AddDays(1);
            reminderTimePicker.Value = DateTime.Today.AddHours(9);

            statusLabel.Text = "Task added successfully!";
            statusLabel.ForeColor = Color.Green;

            // Log activity
            _activityLogger?.LogActivity("Tasks", "Task created", $"'{title}' with reminder for {reminderDateTime:g}");

            // Clear status after 3 seconds
            var clearTimer = new WinFormsTimer { Interval = 3000 };
            clearTimer.Tick += (s, args) =>
            {
                statusLabel.Text = "";
                clearTimer.Stop();
                clearTimer.Dispose();
            };
            clearTimer.Start();
        }

        /// <summary>
        /// Marks selected task as complete.
        /// </summary>
        private void MarkComplete_Click(object? sender, EventArgs e)
        {
            if (taskListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a task to mark as complete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = taskListView.SelectedItems[0];
            var taskId = (Guid)selectedItem.Tag!;
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);

            if (task != null)
            {
                task.IsCompleted = true;
                task.CompletedDate = DateTime.Now;
                SaveTasks();
                RefreshTaskList();

                _activityLogger?.LogActivity("Tasks", "Task completed", $"'{task.Title}' marked as complete");

                MessageBox.Show($"Task '{task.Title}' marked as complete! 🎉", "Task Completed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Deletes selected task.
        /// </summary>
        private void DeleteTask_Click(object? sender, EventArgs e)
        {
            if (taskListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a task to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = taskListView.SelectedItems[0];
            var taskId = (Guid)selectedItem.Tag!;
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);

            if (task != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{task.Title}'?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _tasks.Remove(task);
                    SaveTasks();
                    RefreshTaskList();

                    _activityLogger?.LogActivity("Tasks", "Task deleted", $"'{task.Title}' was deleted");
                }
            }
        }

        /// <summary>
        /// Handles double-click to edit task.
        /// </summary>
        private void EditTask_DoubleClick(object? sender, EventArgs e)
        {
            if (taskListView.SelectedItems.Count == 0)
                return;

            var selectedItem = taskListView.SelectedItems[0];
            var taskId = (Guid)selectedItem.Tag!;
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);

            if (task != null && !task.IsCompleted)
            {
                // Pre-fill form with task data for editing
                titleTextBox.Text = task.Title;
                descriptionTextBox.Text = task.Description;
                reminderDatePicker.Value = task.ReminderDateTime.Date;
                reminderTimePicker.Value = DateTime.Today.Add(task.ReminderDateTime.TimeOfDay);

                // Remove the old task (will be re-added when user clicks Add)
                _tasks.Remove(task);
                SaveTasks();
                RefreshTaskList();
            }
        }

        /// <summary>
        /// Pre-fills the task form with extracted information from NLP.
        /// </summary>
        public void PreFillTaskForm(string title, string fullMessage)
        {
            titleTextBox.Text = title;

            // Try to extract more details from the full message
            var description = ExtractTaskDescription(fullMessage, title);
            descriptionTextBox.Text = description;

            // Try to extract date/time information
            var reminderTime = ExtractReminderTime(fullMessage);
            if (reminderTime.HasValue)
            {
                reminderDatePicker.Value = reminderTime.Value.Date;
                reminderTimePicker.Value = DateTime.Today.Add(reminderTime.Value.TimeOfDay);
            }
        }

        /// <summary>
        /// Extracts task description from natural language.
        /// Source: Basic NLP text processing https://en.wikipedia.org/wiki/Natural_language_processing
        /// </summary>
        private string ExtractTaskDescription(string fullMessage, string title)
        {
            var lowerMessage = fullMessage.ToLower();
            var lowerTitle = title.ToLower();

            // Remove the title part from the message to get description
            var titleIndex = lowerMessage.IndexOf(lowerTitle);
            if (titleIndex >= 0)
            {
                var afterTitle = fullMessage.Substring(titleIndex + title.Length).Trim();

                // Remove common connecting words
                var connectingWords = new[] { "to ", "for ", "about ", "regarding ", "concerning " };
                foreach (var word in connectingWords)
                {
                    if (afterTitle.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                    {
                        afterTitle = afterTitle.Substring(word.Length);
                        break;
                    }
                }

                return afterTitle;
            }

            return "Cybersecurity task - stay safe online!";
        }

        /// <summary>
        /// Extracts reminder time from natural language.
        /// Source: DateTime parsing techniques https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tryparse
        /// </summary>
        private DateTime? ExtractReminderTime(string message)
        {
            var lowerMessage = message.ToLower();

            // Check for common time expressions
            if (lowerMessage.Contains("tomorrow"))
            {
                return DateTime.Today.AddDays(1).AddHours(9); // 9 AM tomorrow
            }

            if (lowerMessage.Contains("today"))
            {
                return DateTime.Today.AddHours(Math.Max(DateTime.Now.Hour + 1, 9));
            }

            if (lowerMessage.Contains("next week"))
            {
                return DateTime.Today.AddDays(7).AddHours(9);
            }

            // Try to parse specific dates/times (basic implementation)
            var words = message.Split(' ');
            foreach (var word in words)
            {
                if (DateTime.TryParse(word, out var parsedDate))
                {
                    return parsedDate;
                }
            }

            return null;
        }

        /// <summary>
        /// Refreshes the task list display.
        /// </summary>
        private void RefreshTaskList()
        {
            taskListView.Items.Clear();

            // Sort tasks: incomplete first, then by reminder date
            var sortedTasks = _tasks
                .OrderBy(t => t.IsCompleted)
                .ThenBy(t => t.ReminderDateTime)
                .ToList();

            foreach (var task in sortedTasks)
            {
                var item = new ListViewItem(task.Title)
                {
                    Tag = task.Id
                };

                item.SubItems.Add(task.Description);
                item.SubItems.Add(task.ReminderDateTime.ToString("MM/dd HH:mm"));
                item.SubItems.Add(task.IsCompleted ? "✅ Done" : "⏰ Pending");

                // Color coding
                if (task.IsCompleted)
                {
                    item.ForeColor = Color.Gray;
                    item.Font = new Font(taskListView.Font, FontStyle.Strikeout);
                }
                else if (task.ReminderDateTime <= DateTime.Now)
                {
                    item.BackColor = Color.FromArgb(255, 235, 235); // Light red for overdue
                    item.ForeColor = Color.DarkRed;
                }
                else if (task.ReminderDateTime <= DateTime.Now.AddHours(1))
                {
                    item.BackColor = Color.FromArgb(255, 248, 220); // Light yellow for soon
                }

                taskListView.Items.Add(item);
            }
        }

        /// <summary>
        /// Checks for task reminders and shows notifications.
        /// Source: Windows Forms notifications https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.notifyicon
        /// </summary>
        private void CheckReminders(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            var upcomingTasks = _tasks
                .Where(t => !t.IsCompleted &&
                           t.ReminderDateTime <= now.AddMinutes(1) &&
                           t.ReminderDateTime > now.AddMinutes(-1))
                .ToList();

            foreach (var task in upcomingTasks)
            {
                ShowTaskReminder(task);
                _activityLogger?.LogActivity("Tasks", "Reminder shown", $"Reminder for '{task.Title}'");
            }
        }

        /// <summary>
        /// Shows a task reminder notification.
        /// </summary>
        private void ShowTaskReminder(CyberTask task)
        {
            var reminderForm = new Form
            {
                Text = "🔔 Cybersecurity Task Reminder",
                Size = new Size(400, 250),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                TopMost = true
            };

            var iconLabel = new Label
            {
                Text = "🛡️",
                Font = new Font("Segoe UI", 24F),
                Location = new Point(20, 20),
                Size = new Size(50, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var titleLabel = new Label
            {
                Text = task.Title,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(80, 20),
                Size = new Size(300, 25),
                ForeColor = Color.FromArgb(220, 53, 69)
            };

            var descriptionLabel = new Label
            {
                Text = task.Description,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(80, 50),
                Size = new Size(300, 60),
                ForeColor = Color.Black
            };

            var timeLabel = new Label
            {
                Text = $"Scheduled for: {task.ReminderDateTime:g}",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(80, 120),
                Size = new Size(300, 20),
                ForeColor = Color.Gray
            };

            var completeButton = new Button
            {
                Text = "Mark Complete",
                Location = new Point(80, 160),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var snoozeButton = new Button
            {
                Text = "Snooze 10min",
                Location = new Point(190, 160),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };

            var dismissButton = new Button
            {
                Text = "Dismiss",
                Location = new Point(300, 160),
                Size = new Size(70, 30),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            completeButton.Click += (s, e) =>
            {
                task.IsCompleted = true;
                task.CompletedDate = DateTime.Now;
                SaveTasks();
                RefreshTaskList();
                reminderForm.Close();
            };

            snoozeButton.Click += (s, e) =>
            {
                task.ReminderDateTime = DateTime.Now.AddMinutes(10);
                SaveTasks();
                RefreshTaskList();
                reminderForm.Close();
            };

            dismissButton.Click += (s, e) => reminderForm.Close();

            reminderForm.Controls.AddRange(new Control[] {
                iconLabel, titleLabel, descriptionLabel, timeLabel,
                completeButton, snoozeButton, dismissButton
            });

            reminderForm.Show();
        }

        /// <summary>
        /// Saves tasks to JSON file.
        /// Source: JSON serialization with Newtonsoft.Json https://www.newtonsoft.com/json/help/html/serializingjson.htm
        /// </summary>
        private void SaveTasks()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_tasks, Formatting.Indented);
                File.WriteAllText(_dataFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving tasks: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads tasks from JSON file.
        /// </summary>
        private void LoadTasks()
        {
            try
            {
                if (File.Exists(_dataFile))
                {
                    var json = File.ReadAllText(_dataFile);
                    var loadedTasks = JsonConvert.DeserializeObject<List<CyberTask>>(json);
                    if (loadedTasks != null)
                    {
                        _tasks.AddRange(loadedTasks);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Load Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Gets task statistics for activity logging.
        /// </summary>
        public TaskStatistics GetTaskStatistics()
        {
            return new TaskStatistics
            {
                TotalTasks = _tasks.Count,
                CompletedTasks = _tasks.Count(t => t.IsCompleted),
                PendingTasks = _tasks.Count(t => !t.IsCompleted),
                OverdueTasks = _tasks.Count(t => !t.IsCompleted && t.ReminderDateTime < DateTime.Now)
            };
        }
    }

    /// <summary>
    /// Represents a cybersecurity task with reminder functionality.
    /// Source: Data model design patterns https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/
    /// </summary>
    public class CyberTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ReminderDateTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Task statistics for reporting.
    /// </summary>
    public class TaskStatistics
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
    }
}

// End of file: TaskManager.cs