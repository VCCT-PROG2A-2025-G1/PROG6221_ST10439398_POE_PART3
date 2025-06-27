// Start of file: EnhancedNlpHandler.cs
// Purpose: Advanced natural language processing with phrase detection, intent recognition, and entity extraction
// Implements Part 3 requirement for significantly extended keyword detection functionality

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Enhanced NLP system that processes full phrases and sentences for better understanding.
    /// Implements Part 3 requirement for significantly extended keyword detection functionality.
    /// </summary>
    public class EnhancedNlpHandler
    {
        private readonly Dictionary<string, string[]> _phrasePatterns;
        private readonly Dictionary<string, string[]> _intentKeywords;
        private readonly ActivityLogger? _activityLogger;

        public EnhancedNlpHandler(ActivityLogger? activityLogger = null)
        {
            _activityLogger = activityLogger;
            _phrasePatterns = InitializePhrasePatterns();
            _intentKeywords = InitializeIntentKeywords();
        }

        /// <summary>
        /// Processes natural language input and returns structured results.
        /// </summary>
        public NlpResult ProcessMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return new NlpResult { Intent = "unknown", Confidence = 0.0 };

            var result = new NlpResult
            {
                OriginalMessage = message,
                ProcessedAt = DateTime.Now
            };

            // 1. Intent Detection
            result.Intent = DetectIntent(message);
            result.Confidence = CalculateConfidence(message, result.Intent);

            // 2. Entity Extraction
            result.Entities = ExtractEntities(message);

            // 3. Phrase Analysis
            result.DetectedPhrases = DetectPhrases(message);

            // 4. Topic Classification
            result.Topics = ClassifyTopics(message);

            // 5. Action Extraction
            result.SuggestedActions = ExtractActions(message);

            // Log NLP processing
            _activityLogger?.LogActivity("NLP", "Message processed",
                $"Intent: {result.Intent}, Confidence: {result.Confidence:P1}, Topics: {string.Join(", ", result.Topics)}");

            return result;
        }

        /// <summary>
        /// Detects the primary intent of the message.
        /// </summary>
        private string DetectIntent(string message)
        {
            var lowerMessage = message.ToLower();

            // Task-related intents
            if (Regex.IsMatch(lowerMessage, @"\b(add|create|set up|make|new)\s+(task|reminder|todo)"))
                return "create_task";

            if (Regex.IsMatch(lowerMessage, @"\b(remind me|set reminder|don't forget)"))
                return "set_reminder";

            // Quiz intents
            if (Regex.IsMatch(lowerMessage, @"\b(start|begin|take|do)\s+(quiz|test|challenge)"))
                return "start_quiz";

            if (Regex.IsMatch(lowerMessage, @"\b(how did i do|quiz results|my score)"))
                return "quiz_results";

            // Information seeking
            if (Regex.IsMatch(lowerMessage, @"\b(what is|how do|can you explain|tell me about)"))
                return "information_request";

            if (Regex.IsMatch(lowerMessage, @"\b(help|assist|support|guide)"))
                return "help_request";

            // Memory/recall
            if (Regex.IsMatch(lowerMessage, @"\b(what did we|remember when|last time|previously)"))
                return "memory_recall";

            // Security assessment
            if (Regex.IsMatch(lowerMessage, @"\b(am i safe|is this secure|should i trust|is it safe)"))
                return "security_assessment";

            return "general_chat";
        }

        /// <summary>
        /// Calculates confidence score for intent detection.
        /// </summary>
        private double CalculateConfidence(string message, string intent)
        {
            if (!_intentKeywords.ContainsKey(intent))
                return 0.5; // Default confidence

            var keywords = _intentKeywords[intent];
            var lowerMessage = message.ToLower();
            var matchCount = keywords.Count(keyword => lowerMessage.Contains(keyword));

            return Math.Min(1.0, (double)matchCount / keywords.Length + 0.3);
        }

        /// <summary>
        /// Extracts entities (people, places, things, times) from the message.
        /// </summary>
        private List<NlpEntity> ExtractEntities(string message)
        {
            var entities = new List<NlpEntity>();

            // Time entities
            var timePatterns = new[]
            {
                (@"\b(tomorrow|today|yesterday)\b", "TIME"),
                (@"\b\d{1,2}:\d{2}\s*(am|pm)?\b", "TIME"),
                (@"\bin\s+\d+\s+(minutes?|hours?|days?|weeks?)\b", "TIME"),
                (@"\b(next|this|last)\s+(week|month|year|monday|tuesday|wednesday|thursday|friday|saturday|sunday)\b", "TIME")
            };

            foreach (var (pattern, entityType) in timePatterns)
            {
                var matches = Regex.Matches(message, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    entities.Add(new NlpEntity
                    {
                        Type = entityType,
                        Value = match.Value,
                        StartIndex = match.Index,
                        Length = match.Length
                    });
                }
            }

            // Security-related entities
            var securityPatterns = new[]
            {
                (@"\b\w+@\w+\.\w+\b", "EMAIL"),
                (@"\b(?:https?://)?(?:www\.)?[\w\-]+\.[\w\-]+(?:\.[\w\-]+)*(?:/[\w\-._~:/?#[\]@!$&'()*+,;=%]*)?", "URL"),
                (@"\b(?:\d{1,3}\.){3}\d{1,3}\b", "IP_ADDRESS"),
                (@"\b[A-F0-9]{2}(?:[:-][A-F0-9]{2}){5}\b", "MAC_ADDRESS")
            };

            foreach (var (pattern, entityType) in securityPatterns)
            {
                var matches = Regex.Matches(message, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    entities.Add(new NlpEntity
                    {
                        Type = entityType,
                        Value = match.Value,
                        StartIndex = match.Index,
                        Length = match.Length
                    });
                }
            }

            return entities;
        }

        /// <summary>
        /// Detects meaningful phrases in the message.
        /// </summary>
        private List<string> DetectPhrases(string message)
        {
            var phrases = new List<string>();
            var lowerMessage = message.ToLower();

            foreach (var category in _phrasePatterns)
            {
                foreach (var pattern in category.Value)
                {
                    if (lowerMessage.Contains(pattern))
                    {
                        phrases.Add($"{category.Key}: {pattern}");
                    }
                }
            }

            return phrases;
        }

        /// <summary>
        /// Classifies the message into cybersecurity topics.
        /// </summary>
        private List<string> ClassifyTopics(string message)
        {
            var topics = new List<string>();
            var lowerMessage = message.ToLower();

            var topicKeywords = new Dictionary<string, string[]>
            {
                { "Password Security", new[] { "password", "passphrase", "login", "credential", "authentication", "2fa", "two-factor" } },
                { "Network Security", new[] { "firewall", "vpn", "wifi", "network", "router", "encryption" } },
                { "Email Security", new[] { "phishing", "spam", "email", "attachment", "suspicious message" } },
                { "Malware Protection", new[] { "virus", "malware", "ransomware", "trojan", "antivirus", "infection" } },
                { "Privacy Protection", new[] { "privacy", "tracking", "data collection", "personal information", "social media" } },
                { "Social Engineering", new[] { "scam", "fraud", "social engineering", "manipulation", "pretexting" } },
                { "Data Security", new[] { "backup", "data loss", "recovery", "encryption", "secure storage" } },
                { "Mobile Security", new[] { "phone", "mobile", "app", "smartphone", "tablet", "android", "ios" } }
            };

            foreach (var topic in topicKeywords)
            {
                if (topic.Value.Any(keyword => lowerMessage.Contains(keyword)))
                {
                    topics.Add(topic.Key);
                }
            }

            return topics.Any() ? topics : new List<string> { "General Security" };
        }

        /// <summary>
        /// Extracts suggested actions from the message.
        /// </summary>
        private List<string> ExtractActions(string message)
        {
            var actions = new List<string>();
            var lowerMessage = message.ToLower();

            var actionPatterns = new Dictionary<string, string[]>
            {
                { "Create Task", new[] { "remind me", "don't forget", "add task", "create reminder", "set up reminder" } },
                { "Take Quiz", new[] { "test me", "quiz", "challenge", "assess my knowledge" } },
                { "Learn More", new[] { "tell me more", "explain", "how does", "what is", "learn about" } },
                { "Check Security", new[] { "am i safe", "is this secure", "check my security", "scan for", "verify" } },
                { "Update Settings", new[] { "change settings", "update", "configure", "enable", "disable" } }
            };

            foreach (var actionType in actionPatterns)
            {
                if (actionType.Value.Any(pattern => lowerMessage.Contains(pattern)))
                {
                    actions.Add(actionType.Key);
                }
            }

            return actions;
        }

        /// <summary>
        /// Initializes phrase patterns for detection.
        /// </summary>
        private Dictionary<string, string[]> InitializePhrasePatterns()
        {
            return new Dictionary<string, string[]>
            {
                { "Password Concerns", new[] {
                    "forgot my password",
                    "password not working",
                    "can't remember password",
                    "locked out of account",
                    "password too weak",
                    "same password everywhere"
                }},
                { "Security Threats", new[] {
                    "suspicious email",
                    "weird text message",
                    "strange phone call",
                    "unusual activity",
                    "someone hacked",
                    "computer acting weird"
                }},
                { "Task Requests", new[] {
                    "remind me to",
                    "don't forget to",
                    "set up a reminder",
                    "add task for",
                    "need to remember",
                    "schedule reminder"
                }},
                { "Learning Requests", new[] {
                    "how do i",
                    "what should i do",
                    "best way to",
                    "safest method",
                    "protect myself from",
                    "stay safe from"
                }}
            };
        }

        /// <summary>
        /// Initializes intent-specific keywords.
        /// </summary>
        private Dictionary<string, string[]> InitializeIntentKeywords()
        {
            return new Dictionary<string, string[]>
            {
                { "create_task", new[] { "task", "reminder", "todo", "remember", "don't forget", "add", "create" } },
                { "start_quiz", new[] { "quiz", "test", "challenge", "assessment", "evaluate", "check knowledge" } },
                { "information_request", new[] { "what", "how", "when", "where", "why", "explain", "tell me" } },
                { "help_request", new[] { "help", "assist", "support", "guide", "confused", "lost" } },
                { "memory_recall", new[] { "remember", "recall", "previous", "last time", "before", "history" } },
                { "security_assessment", new[] { "safe", "secure", "trust", "suspicious", "dangerous", "risky" } }
            };
        }
    }

    /// <summary>
    /// Represents the result of NLP processing.
    /// </summary>
    public class NlpResult
    {
        public string OriginalMessage { get; set; } = string.Empty;
        public string Intent { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public List<NlpEntity> Entities { get; set; } = new List<NlpEntity>();
        public List<string> DetectedPhrases { get; set; } = new List<string>();
        public List<string> Topics { get; set; } = new List<string>();
        public List<string> SuggestedActions { get; set; } = new List<string>();
        public DateTime ProcessedAt { get; set; }
    }

    /// <summary>
    /// Represents an extracted entity from text.
    /// </summary>
    public class NlpEntity
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int StartIndex { get; set; }
        public int Length { get; set; }
    }
}

// End of file: EnhancedNlpHandler.cs