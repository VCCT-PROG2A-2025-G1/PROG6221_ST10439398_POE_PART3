// Start of file: ActivityLogViewer.cs
// Purpose: Complete activity log viewer with filtering, export, and detailed history
// Implements Part 3 requirement for activity logging and chat history

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Complete activity log viewer with advanced filtering and export capabilities.
    /// </summary>
    public class ActivityLogViewer
    {
        private readonly ActivityLogger _activityLogger;
        private ListView? _activityListView;
        private ComboBox? _categoryFilter;
        private DateTimePicker? _dateFromPicker;
        private DateTimePicker? _dateToPicker;
        private TextBox? _searchTextBox;
        private Button? _filterButton;
        private Button? _exportButton;
        private Button? _clearLogButton;
        private Label? _statsLabel;

        public ActivityLogViewer(ActivityLogger activityLogger)
        {
            _activityLogger = activityLogger ?? throw new ArgumentNullException(nameof(activityLogger));
        }

        /// <summary>
        /// Initializes the complete activity log interface.
        /// </summary>
        public void InitializeActivityInterface(Panel parentPanel)
        {
            parentPanel.Controls.Clear();
            CreateActivityInterface(parentPanel);
            RefreshActivityList();
            UpdateStatistics();
        }

        /// <summary>
        /// Creates the comprehensive activity log user interface.
        /// </summary>
        private void CreateActivityInterface(Panel parentPanel)
        {
            // Title
            var titleLabel = new Label
            {
                Text = "📊 Activity Log & Chat History",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(10, 10),
                AutoSize = true
            };

            // Statistics panel
            var statsPanel = new Panel
            {
                Location = new Point(10, 50),
                Size = new Size(760, 40),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            _statsLabel = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(740, 20),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(73, 80, 87),
                Text = "Loading statistics..."
            };

            statsPanel.Controls.Add(_statsLabel);

            // Filter panel
            var filterPanel = new Panel
            {
                Location = new Point(10, 100),
                Size = new Size(760, 80),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Category filter
            var categoryLabel = new Label
            {
                Text = "Category:",
                Location = new Point(10, 15),
                Size = new Size(60, 20),
                Font = new Font("Segoe UI", 9F)
            };

            _categoryFilter = new ComboBox
            {
                Location = new Point(75, 13),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };

            _categoryFilter.Items.AddRange(new[] { "All", "Chat", "Tasks", "Quiz", "NLP", "System" });
            _categoryFilter.SelectedIndex = 0;

            // Date range filters
            var dateFromLabel = new Label
            {
                Text = "From:",
                Location = new Point(190, 15),
                Size = new Size(35, 20),
                Font = new Font("Segoe UI", 9F)
            };

            _dateFromPicker = new DateTimePicker
            {
                Location = new Point(230, 13),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(-7)
            };

            var dateToLabel = new Label
            {
                Text = "To:",
                Location = new Point(360, 15),
                Size = new Size(25, 20),
                Font = new Font("Segoe UI", 9F)
            };

            _dateToPicker = new DateTimePicker
            {
                Location = new Point(390, 13),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            // Search box
            var searchLabel = new Label
            {
                Text = "Search:",
                Location = new Point(10, 45),
                Size = new Size(50, 20),
                Font = new Font("Segoe UI", 9F)
            };

            _searchTextBox = new TextBox
            {
                Location = new Point(65, 43),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 9F),
                PlaceholderText = "Search in activities..."
            };

            // Filter and action buttons
            _filterButton = new Button
            {
                Text = "🔍 Filter",
                Location = new Point(280, 40),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            _exportButton = new Button
            {
                Text = "📤 Export",
                Location = new Point(370, 40),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            _clearLogButton = new Button
            {
                Text = "🗑️ Clear",
                Location = new Point(460, 40),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            var refreshButton = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(550, 40),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            filterPanel.Controls.AddRange(new Control[] {
                categoryLabel, _categoryFilter, dateFromLabel, _dateFromPicker,
                dateToLabel, _dateToPicker, searchLabel, _searchTextBox,
                _filterButton, _exportButton, _clearLogButton, refreshButton
            });

            // Activity list
            _activityListView = new ListView
            {
                Location = new Point(10, 190),
                Size = new Size(760, 350),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Consolas", 9F),
                Scrollable = true
            };

            _activityListView.Columns.AddRange(new ColumnHeader[]
            {
                new ColumnHeader { Text = "Time", Width = 120 },
                new ColumnHeader { Text = "Category", Width = 80 },
                new ColumnHeader { Text = "Action", Width = 120 },
                new ColumnHeader { Text = "Details", Width = 420 }
            });

            // Instructions
            var instructionsLabel = new Label
            {
                Text = "💡 This log tracks all your interactions: chat messages, tasks created/completed, quiz attempts, and NLP detections. " +
                       "Use filters to find specific activities or export logs for analysis.",
                Location = new Point(10, 550),
                Size = new Size(760, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            // Event handlers
            _filterButton.Click += FilterButton_Click;
            _exportButton.Click += ExportButton_Click;
            _clearLogButton.Click += ClearLogButton_Click;
            refreshButton.Click += (s, e) => { RefreshActivityList(); UpdateStatistics(); };
            _searchTextBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) FilterButton_Click(s, e); };

            parentPanel.Controls.AddRange(new Control[] {
                titleLabel, statsPanel, filterPanel, _activityListView, instructionsLabel
            });
        }

        /// <summary>
        /// Applies filters and refreshes the activity list.
        /// </summary>
        private void FilterButton_Click(object? sender, EventArgs e)
        {
            RefreshActivityList();
        }

        /// <summary>
        /// Exports activity log to a file.
        /// </summary>
        private void ExportButton_Click(object? sender, EventArgs e)
        {
            try
            {
                using var saveDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    DefaultExt = "csv",
                    FileName = $"CyberBot_ActivityLog_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var activities = GetFilteredActivities();
                    ExportActivities(activities, saveDialog.FileName);

                    MessageBox.Show($"Activity log exported successfully!\n\nFile: {saveDialog.FileName}\nRecords: {activities.Count}",
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _activityLogger.LogActivity("System", "Log exported", $"Exported {activities.Count} activities to {Path.GetFileName(saveDialog.FileName)}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting activity log: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Clears the activity log after confirmation.
        /// </summary>
        private void ClearLogButton_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear the entire activity log?\n\nThis action cannot be undone.",
                "Clear Activity Log", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                var activityCount = _activityLogger.GetRecentActivities(int.MaxValue).Count;
                _activityLogger.ClearLog();
                RefreshActivityList();
                UpdateStatistics();

                MessageBox.Show($"Activity log cleared successfully!\n\nRemoved {activityCount} activities.",
                    "Log Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Refreshes the activity list with current filters applied.
        /// </summary>
        private void RefreshActivityList()
        {
            if (_activityListView == null) return;

            _activityListView.Items.Clear();
            var activities = GetFilteredActivities();

            foreach (var activity in activities.Take(500)) // Limit to 500 for performance
            {
                var item = new ListViewItem(activity.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"))
                {
                    Tag = activity
                };

                item.SubItems.Add(activity.Category);
                item.SubItems.Add(activity.Action);
                item.SubItems.Add(activity.Details);

                // Color coding by category
                switch (activity.Category.ToLower())
                {
                    case "chat":
                        item.ForeColor = Color.FromArgb(0, 123, 255);
                        break;
                    case "tasks":
                        item.ForeColor = Color.FromArgb(40, 167, 69);
                        break;
                    case "quiz":
                        item.ForeColor = Color.FromArgb(255, 193, 7);
                        break;
                    case "nlp":
                        item.ForeColor = Color.FromArgb(220, 53, 69);
                        break;
                    case "system":
                        item.ForeColor = Color.FromArgb(108, 117, 125);
                        break;
                }

                _activityListView.Items.Add(item);
            }

            // Auto-scroll to latest activity
            if (_activityListView.Items.Count > 0)
            {
                _activityListView.Items[_activityListView.Items.Count - 1].EnsureVisible();
            }
        }

        /// <summary>
        /// Gets filtered activities based on current filter settings.
        /// </summary>
        private List<ActivityLogEntry> GetFilteredActivities()
        {
            var activities = _activityLogger.GetRecentActivities(int.MaxValue);

            // Apply category filter
            if (_categoryFilter?.SelectedItem?.ToString() != "All")
            {
                var selectedCategory = _categoryFilter?.SelectedItem?.ToString();
                activities = activities.Where(a => a.Category.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply date range filter
            if (_dateFromPicker != null && _dateToPicker != null)
            {
                var fromDate = _dateFromPicker.Value.Date;
                var toDate = _dateToPicker.Value.Date.AddDays(1); // Include the entire "to" day
                activities = activities.Where(a => a.Timestamp >= fromDate && a.Timestamp < toDate).ToList();
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchTextBox?.Text))
            {
                var searchTerm = _searchTextBox.Text.ToLower();
                activities = activities.Where(a =>
                    a.Action.ToLower().Contains(searchTerm) ||
                    a.Details.ToLower().Contains(searchTerm) ||
                    a.Category.ToLower().Contains(searchTerm)).ToList();
            }

            return activities.OrderByDescending(a => a.Timestamp).ToList();
        }

        /// <summary>
        /// Exports activities to a file.
        /// </summary>
        private void ExportActivities(List<ActivityLogEntry> activities, string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();

            if (extension == ".csv")
            {
                ExportToCsv(activities, fileName);
            }
            else
            {
                ExportToText(activities, fileName);
            }
        }

        /// <summary>
        /// Exports activities to CSV format.
        /// </summary>
        private void ExportToCsv(List<ActivityLogEntry> activities, string fileName)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Timestamp,Category,Action,Details");

            foreach (var activity in activities)
            {
                csv.AppendLine($"\"{activity.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{activity.Category}\",\"{activity.Action}\",\"{EscapeCsvField(activity.Details)}\"");
            }

            File.WriteAllText(fileName, csv.ToString());
        }

        /// <summary>
        /// Exports activities to text format.
        /// </summary>
        private void ExportToText(List<ActivityLogEntry> activities, string fileName)
        {
            var text = new StringBuilder();
            text.AppendLine("Cybersecurity Awareness Bot - Activity Log Export");
            text.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            text.AppendLine($"Total Activities: {activities.Count}");
            text.AppendLine(new string('=', 80));
            text.AppendLine();

            foreach (var activity in activities)
            {
                text.AppendLine($"[{activity.Timestamp:yyyy-MM-dd HH:mm:ss}] {activity.Category}: {activity.Action}");
                text.AppendLine($"Details: {activity.Details}");
                text.AppendLine();
            }

            File.WriteAllText(fileName, text.ToString());
        }

        /// <summary>
        /// Escapes CSV field content.
        /// </summary>
        private string EscapeCsvField(string field)
        {
            if (field.Contains("\""))
            {
                field = field.Replace("\"", "\"\"");
            }
            return field;
        }

        /// <summary>
        /// Updates activity statistics display.
        /// </summary>
        private void UpdateStatistics()
        {
            if (_statsLabel == null) return;

            var activities = _activityLogger.GetRecentActivities(int.MaxValue);
            var today = DateTime.Today;

            var totalActivities = activities.Count;
            var todayActivities = activities.Count(a => a.Timestamp.Date == today);
            var chatActivities = activities.Count(a => a.Category.Equals("Chat", StringComparison.OrdinalIgnoreCase));
            var taskActivities = activities.Count(a => a.Category.Equals("Tasks", StringComparison.OrdinalIgnoreCase));
            var quizActivities = activities.Count(a => a.Category.Equals("Quiz", StringComparison.OrdinalIgnoreCase));
            var nlpActivities = activities.Count(a => a.Category.Equals("NLP", StringComparison.OrdinalIgnoreCase));

            _statsLabel.Text = $"📊 Total: {totalActivities} | Today: {todayActivities} | " +
                             $"💬 Chat: {chatActivities} | 📋 Tasks: {taskActivities} | " +
                             $"🧠 Quiz: {quizActivities} | 🔍 NLP: {nlpActivities}";
        }
    }

    /// <summary>
    /// Enhanced ActivityLogger with improved functionality.
    /// </summary>
    public partial class ActivityLogger
    {
        private readonly List<ActivityLogEntry> _activities = new List<ActivityLogEntry>();
        private readonly object _lock = new object();

        public void LogActivity(string category, string action, string details)
        {
            lock (_lock)
            {
                var entry = new ActivityLogEntry
                {
                    Timestamp = DateTime.Now,
                    Category = category,
                    Action = action,
                    Details = details
                };

                _activities.Add(entry);

                // Keep only last 10000 entries to prevent memory issues
                if (_activities.Count > 10000)
                {
                    _activities.RemoveRange(0, 1000);
                }

                Console.WriteLine($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] {category}: {action} - {details}");
            }
        }

        public List<ActivityLogEntry> GetRecentActivities(int count = 50)
        {
            lock (_lock)
            {
                return _activities.TakeLast(count).ToList();
            }
        }

        public void ClearLog()
        {
            lock (_lock)
            {
                _activities.Clear();
            }
        }
    }

    /// <summary>
    /// Represents an activity log entry.
    /// </summary>
    public class ActivityLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}

// End of file: ActivityLogViewer.cs