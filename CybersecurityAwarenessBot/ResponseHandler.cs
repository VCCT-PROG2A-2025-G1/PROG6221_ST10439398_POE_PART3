// Start of file: ResponseHandler.cs
// Purpose: Handles user input, generates responses, manages memory, and detects sentiment.
// Implements Part 2 requirements: keyword recognition, random responses, conversation flow,
// memory/recall, sentiment detection, error handling, and code optimization.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Handles user input, generates responses, manages memory, and detects sentiment.
    /// </summary>
    public class ResponseHandler
    {
        private readonly Dictionary<string, List<string>> _responses;
        private readonly Dictionary<string, string[]> _keywords;
        private readonly List<string> _conversationHistory;
        private readonly Random _random;
        private string? _userName;
        private string? _favoriteTopic;
        private string? _lastQuestion;
        private readonly Dictionary<string, int> _topicInterestCounts;

        public string? LastTopic { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ResponseHandler class.
        /// Sets up response variations for various cybersecurity topics.
        /// </summary>
        public ResponseHandler()
        {
            _random = new Random();
            _conversationHistory = new List<string>();
            _topicInterestCounts = new Dictionary<string, int>();

            // Expanded keyword recognition for better matching
            _keywords = new Dictionary<string, string[]>
            {
                { "password", new[] { "password", "passwords", "pass", "login", "credential", "authentication", "auth" } },
                { "scam", new[] { "scam", "scams", "fraud", "fraudulent", "fake", "trick", "deception", "con" } },
                { "privacy", new[] { "privacy", "private", "personal", "data", "information", "tracking", "surveillance" } },
                { "firewall", new[] { "firewall", "firewalls", "barrier", "protection", "block", "filter" } },
                { "vpn", new[] { "vpn", "virtual private network", "tunnel", "encrypt", "anonymous", "proxy" } },
                { "phishing", new[] { "phishing", "phish", "email scam", "fake email", "suspicious email", "bait" } },
                { "malware", new[] { "malware", "virus", "trojan", "spyware", "ransomware", "infection", "malicious software" } },
                { "security", new[] { "security", "secure", "safety", "safe", "protect", "protection" } },
                { "hacker", new[] { "hacker", "hackers", "hacking", "cybercriminal", "attacker", "breach" } },
                { "update", new[] { "update", "updates", "patch", "patches", "upgrade", "version" } }
            };

            // Expanded responses with more variety
            _responses = new Dictionary<string, List<string>>
            {
                { "password", new List<string>
                    {
                        "Strong passwords should be at least 12 characters long with a mix of uppercase, lowercase, numbers, and symbols!",
                        "Never reuse passwords across different accounts. Each account deserves its own unique password!",
                        "Consider using a password manager to generate and store complex passwords securely.",
                        "A good passphrase like 'Coffee#Sunrise42!' is both memorable and secure.",
                        "Enable two-factor authentication wherever possible - it's like a second lock on your digital door!",
                        "Avoid using personal information like birthdays or pet names in your passwords."
                    }
                },
                { "scam", new List<string>
                    {
                        "Scammers often create urgency to make you act without thinking. Take a moment to verify!",
                        "If someone asks for personal information via email or phone, always verify their identity first.",
                        "Common red flags: poor grammar, urgent demands, requests for gift cards or wire transfers.",
                        "When in doubt, contact the organization directly using official contact information.",
                        "Remember: legitimate companies won't ask for passwords or sensitive info via email.",
                        "Trust your instincts - if something feels off, it probably is!"
                    }
                },
                { "privacy", new List<string>
                    {
                        "Review your social media privacy settings regularly - oversharing can lead to identity theft!",
                        "Be cautious about what personal information you share online, especially on public platforms.",
                        "Use privacy-focused search engines and browsers to reduce data tracking.",
                        "Read privacy policies to understand how your data is collected and used.",
                        "Consider using encrypted messaging apps for sensitive conversations.",
                        "Limit location sharing on apps and social media to protect your whereabouts."
                    }
                },
                { "firewall", new List<string>
                    {
                        "A firewall acts like a security guard, monitoring and controlling network traffic to your device.",
                        "Make sure your firewall is enabled - it's your first line of defense against cyber threats!",
                        "Both hardware and software firewalls work together to provide layered security.",
                        "Configure your firewall to block unnecessary ports and allow only trusted applications.",
                        "Regularly update your firewall rules to adapt to new threats and security requirements.",
                        "Don't disable your firewall unless absolutely necessary, and always re-enable it quickly."
                    }
                },
                { "vpn", new List<string>
                    {
                        "A VPN encrypts your internet traffic, making it unreadable to potential eavesdroppers.",
                        "Always use a VPN on public Wi-Fi networks like coffee shops or airports - they're often unsecured!",
                        "Choose a reputable VPN provider that doesn't log your activity for maximum privacy.",
                        "VPNs can also help you access geo-restricted content while maintaining security.",
                        "Some VPNs offer additional features like ad blocking and malware protection.",
                        "Remember: a VPN protects your data in transit, but you still need good security practices."
                    }
                },
                { "phishing", new List<string>
                    {
                        "Phishing emails often mimic trusted organizations to steal your credentials or personal information.",
                        "Look for red flags: misspelled URLs, urgent language, or requests for immediate action.",
                        "Never click links in suspicious emails - type URLs directly into your browser instead.",
                        "Check the sender's email address carefully - scammers often use similar-looking domains.",
                        "When in doubt, contact the organization directly through official channels to verify.",
                        "Use email security features like spam filters to catch most phishing attempts automatically."
                    }
                },
                { "malware", new List<string>
                    {
                        "Malware includes viruses, trojans, ransomware, and spyware - all designed to harm your system.",
                        "Keep your antivirus software updated and run regular system scans to catch infections early.",
                        "Avoid downloading software from untrusted sources - stick to official app stores and websites.",
                        "Be cautious with email attachments, even from known contacts, as they can be compromised.",
                        "Keep your operating system and software updated to patch security vulnerabilities.",
                        "Back up your important data regularly - it's your best defense against ransomware attacks."
                    }
                },
                { "security", new List<string>
                    {
                        "Good cybersecurity is like good hygiene - it requires consistent daily practices!",
                        "The three pillars of security are: something you know, something you have, and something you are.",
                        "Regular security updates are crucial - they patch vulnerabilities that criminals exploit.",
                        "Think before you click, share, or download - most security breaches involve human error.",
                        "Use different passwords for different accounts to limit damage if one gets compromised.",
                        "Stay informed about current threats and scams - knowledge is your best defense!"
                    }
                },
                { "hacker", new List<string>
                    {
                        "Hackers use various techniques like social engineering, malware, and exploiting software vulnerabilities.",
                        "Not all hackers are criminals - ethical hackers help companies find and fix security flaws.",
                        "Cybercriminals often target the weakest link in security - which is usually human behavior.",
                        "Common attack methods include phishing, brute force attacks, and exploiting unpatched software.",
                        "The best defense against hackers is a combination of good security practices and updated software.",
                        "If you suspect you've been hacked, change passwords immediately and scan for malware."
                    }
                },
                { "update", new List<string>
                    {
                        "Software updates often include critical security patches - don't delay installing them!",
                        "Enable automatic updates when possible to ensure you're always protected against known threats.",
                        "Update not just your operating system, but also browsers, apps, and security software.",
                        "Many cyberattacks exploit vulnerabilities in outdated software - stay current!",
                        "Set reminders to check for updates regularly, especially for security-critical applications.",
                        "Sometimes updates include new security features - take time to explore and enable them."
                    }
                },
                { "how are you", new List<string>
                    {
                        "I'm doing great and ready to help you stay secure online! How can I assist you today?",
                        "I'm here and fully charged! What cybersecurity topic would you like to explore?",
                        "I'm feeling helpful today! What's on your mind regarding online safety?",
                        "I'm awesome and excited to share some security knowledge with you!",
                        "I'm doing well, thanks for asking! Ready to tackle any cybersecurity questions you have."
                    }
                },
                { "purpose", new List<string>
                    {
                        "I'm here to help you learn about cybersecurity and develop good digital safety habits!",
                        "My mission is to make cybersecurity knowledge accessible and easy to understand for everyone.",
                        "I exist to help you navigate the digital world safely and confidently!",
                        "I'm your personal cybersecurity educator, here to answer questions and share best practices.",
                        "My purpose is to empower you with the knowledge needed to protect yourself online!"
                    }
                },
                { "thanks", new List<string>
                    {
                        "You're very welcome! Remember, cybersecurity is a team effort - we're in this together!",
                        "Happy to help! Feel free to ask me anything else about staying safe online.",
                        "My pleasure! Keep that curiosity about cybersecurity - it's your best defense!",
                        "Glad I could assist! What other security topics would you like to explore?",
                        "You're welcome! Don't hesitate to come back with more questions anytime."
                    }
                }
            };
        }

        /// <summary>
        /// Sets the user's name for personalized responses.
        /// </summary>
        /// <param name="name">The user's name.</param>
        public void SetUserName(string name)
        {
            _userName = name;
        }

        /// <summary>
        /// Generates a response based on the user's question, incorporating sentiment and memory.
        /// Implements keyword recognition, random responses, and sentiment detection.
        /// </summary>
        /// <param name="question">The user's question.</param>
        /// <returns>The generated response string.</returns>
        public string GetResponse(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return "I didn't catch that. Could you please repeat your question?";
            }

            // Store the question in conversation history
            _conversationHistory.Add($"User: {question}");
            string normalizedQuestion = question.ToLower().Trim();

            // Handle memory/recall queries
            if (normalizedQuestion.Contains("last question") || normalizedQuestion.Contains("previous question"))
            {
                if (_lastQuestion != null)
                {
                    string response = $"Your last question was: '{_lastQuestion}'. Would you like me to elaborate on that topic?";
                    _conversationHistory.Add($"Bot: {response}");
                    return response;
                }
                return "This is actually your first question! What would you like to know about cybersecurity?";
            }

            if (normalizedQuestion.Contains("conversation history") || normalizedQuestion.Contains("what did we talk about"))
            {
                if (_conversationHistory.Count > 2)
                {
                    var recentTopics = GetRecentTopics();
                    string response = $"We've discussed: {string.Join(", ", recentTopics)}. What would you like to explore further?";
                    _conversationHistory.Add($"Bot: {response}");
                    return response;
                }
                return "We've just started our conversation! What cybersecurity topic interests you?";
            }

            if (normalizedQuestion.Contains("favorite") || normalizedQuestion.Contains("most interested"))
            {
                if (_favoriteTopic != null)
                {
                    string response = $"You seem most interested in {_favoriteTopic}. That's a great area to focus on! Want to learn more advanced tips?";
                    _conversationHistory.Add($"Bot: {response}");
                    return response;
                }
                return "We haven't explored enough topics yet to determine your favorite. Ask me about passwords, privacy, or security!";
            }

            // Detect sentiment
            string sentiment = DetectSentiment(normalizedQuestion);
            string sentimentResponse = GetSentimentResponse(sentiment);

            // Enhanced keyword recognition
            string? matchedTopic = FindMatchingTopic(normalizedQuestion);

            if (matchedTopic != null)
            {
                // Track topic interest
                if (!_topicInterestCounts.ContainsKey(matchedTopic))
                    _topicInterestCounts[matchedTopic] = 0;
                _topicInterestCounts[matchedTopic]++;

                // Update favorite topic based on interest
                _favoriteTopic = _topicInterestCounts.OrderByDescending(x => x.Value).First().Key;

                LastTopic = GetTopicDisplayName(matchedTopic);

                string response = sentimentResponse + GetRandomResponse(matchedTopic);
                _conversationHistory.Add($"Bot: {response}");
                _lastQuestion = question;
                return response;
            }

            // Handle common conversational inputs
            if (normalizedQuestion.Contains("how are you") || normalizedQuestion.Contains("how's it going"))
            {
                string response = sentimentResponse + GetRandomResponse("how are you");
                _conversationHistory.Add($"Bot: {response}");
                _lastQuestion = question;
                return response;
            }

            if (normalizedQuestion.Contains("purpose") || normalizedQuestion.Contains("what do you do"))
            {
                string response = sentimentResponse + GetRandomResponse("purpose");
                _conversationHistory.Add($"Bot: {response}");
                _lastQuestion = question;
                return response;
            }

            if (normalizedQuestion.Contains("thank") || normalizedQuestion.Contains("thanks"))
            {
                LastTopic = "gratitude";
                string response = sentimentResponse + GetRandomResponse("thanks");
                _conversationHistory.Add($"Bot: {response}");
                _lastQuestion = question;
                return response;
            }

            // Default response with helpful suggestions
            string defaultResponse = GenerateDefaultResponse(sentimentResponse);
            LastTopic = null;
            _conversationHistory.Add($"Bot: {defaultResponse}");
            _lastQuestion = question;
            return defaultResponse;
        }

        /// <summary>
        /// Finds matching topic based on keyword recognition.
        /// </summary>
        /// <param name="question">The normalized question.</param>
        /// <returns>The matching topic or null if no match found.</returns>
        private string? FindMatchingTopic(string question)
        {
            foreach (var topicKeywords in _keywords)
            {
                if (topicKeywords.Value.Any(keyword => question.Contains(keyword)))
                {
                    return topicKeywords.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a random response for the specified topic.
        /// </summary>
        /// <param name="topic">The topic to get a response for.</param>
        /// <returns>A random response string.</returns>
        private string GetRandomResponse(string topic)
        {
            if (_responses.ContainsKey(topic) && _responses[topic].Count > 0)
            {
                return _responses[topic][_random.Next(_responses[topic].Count)];
            }
            return "I'd love to help with that topic, but I'm still learning about it!";
        }

        /// <summary>
        /// Gets the display name for a topic.
        /// </summary>
        /// <param name="topic">The internal topic name.</param>
        /// <returns>The user-friendly topic name.</returns>
        private string GetTopicDisplayName(string topic)
        {
            return topic switch
            {
                "password" => "password security",
                "scam" => "scam prevention",
                "privacy" => "privacy protection",
                "firewall" => "firewall security",
                "vpn" => "VPN usage",
                "phishing" => "phishing awareness",
                "malware" => "malware protection",
                "security" => "general security",
                "hacker" => "hacker prevention",
                "update" => "software updates",
                _ => topic
            };
        }

        /// <summary>
        /// Gets recent topics from conversation history.
        /// </summary>
        /// <returns>List of recent topics discussed.</returns>
        private List<string> GetRecentTopics()
        {
            var topics = new List<string>();
            if (_topicInterestCounts.Any())
            {
                topics.AddRange(_topicInterestCounts.Keys.Select(GetTopicDisplayName).Take(3));
            }
            return topics;
        }

        /// <summary>
        /// Generates a default response when no topic matches.
        /// </summary>
        /// <param name="sentimentResponse">The sentiment-based response prefix.</param>
        /// <returns>A helpful default response.</returns>
        private string GenerateDefaultResponse(string sentimentResponse)
        {
            var suggestions = new[]
            {
                "I can help with passwords, privacy, scams, firewalls, VPNs, malware, and more!",
                "Ask me about cybersecurity topics like password security, avoiding scams, or protecting your privacy.",
                "I'm here to help with online safety! Try asking about firewalls, VPNs, or software updates.",
                "What aspect of cybersecurity interests you? I can discuss everything from basic security to advanced threats!"
            };

            return sentimentResponse + suggestions[_random.Next(suggestions.Length)];
        }

        /// <summary>
        /// Provides a follow-up response based on the last topic discussed.
        /// Implements conversation flow with follow-up questions.
        /// </summary>
        /// <param name="topic">The last topic discussed.</param>
        /// <returns>A follow-up response string.</returns>
        public string GetFollowUpResponse(string? topic)
        {
            if (topic == null)
            {
                return "What other cybersecurity topics would you like to explore?";
            }

            var followUps = new Dictionary<string, string[]>
            {
                { "password security", new[]
                    {
                        "Here's a pro tip: Use a password manager to generate and store unique passwords for every account!",
                        "Did you know? Passphrases like 'Purple$Elephant#Dancing42' are both secure and memorable!",
                        "Enable two-factor authentication everywhere possible - it's like adding a deadbolt to your digital doors!"
                    }
                },
                { "scam prevention", new[]
                    {
                        "Remember: When in doubt, verify independently. Call the organization using official contact info!",
                        "Pro tip: Take screenshots of suspicious messages and report them to help protect others!",
                        "You can report scams to the FTC at ReportFraud.ftc.gov to help stop scammers!"
                    }
                },
                { "privacy protection", new[]
                    {
                        _userName != null ? $"Great question, {_userName}! Consider using privacy-focused search engines like DuckDuckGo." : "Consider using privacy-focused search engines like DuckDuckGo for better privacy!",
                        "Review your social media privacy settings monthly - platforms often change their default settings!",
                        "Use encrypted messaging apps like Signal for sensitive conversations!"
                    }
                },
                { "firewall security", new[]
                    {
                        "Want to check if your firewall is working? Use online firewall testing tools like ShieldsUP!",
                        "Consider both hardware (router) and software firewalls for layered protection!",
                        "Regularly review your firewall logs to spot potential intrusion attempts!"
                    }
                },
                { "VPN usage", new[]
                    {
                        "Pro tip: Some VPNs offer split tunneling, letting you choose which apps use the VPN connection!",
                        "Always choose a VPN with a strict no-logs policy to ensure your privacy!",
                        "Remember: Free VPNs often sell your data - invest in a reputable paid service!"
                    }
                },
                { "phishing awareness", new[]
                    {
                        "Here's a trick: Hover over links without clicking to see the real destination URL!",
                        "Enable email security features in your email client to automatically flag suspicious messages!",
                        "When in doubt, open a new browser tab and navigate to the site directly instead of clicking links!"
                    }
                },
                { "malware protection", new[]
                    {
                        "Keep your software updated - many malware attacks exploit known vulnerabilities!",
                        "Set up automatic backups to cloud storage or external drives to protect against ransomware!",
                        "Consider using browser extensions that block malicious websites and downloads!"
                    }
                },
                { "general security", new[]
                    {
                        "Security is a process, not a product - make it part of your daily digital routine!",
                        "The 3-2-1 backup rule: 3 copies of important data, 2 different media types, 1 offsite backup!",
                        "Stay informed about current threats by following cybersecurity news and advisories!"
                    }
                },
                { "gratitude", new[]
                    {
                        "Keep that security mindset active! Regular learning is key to staying protected!",
                        "Remember: You're now better equipped to spot and avoid cyber threats!",
                        "Share your cybersecurity knowledge with friends and family - security is everyone's responsibility!"
                    }
                }
            };

            if (followUps.ContainsKey(topic))
            {
                var responses = followUps[topic];
                return responses[_random.Next(responses.Length)];
            }

            return "Want to explore another cybersecurity topic? I'm here to help with any questions!";
        }

        /// <summary>
        /// Detects the user's sentiment based on keywords in the question.
        /// Implements sentiment detection requirement.
        /// </summary>
        /// <param name="question">The user's question.</param>
        /// <returns>The detected sentiment as a string.</returns>
        private string DetectSentiment(string question)
        {
            // Worried/Anxious sentiment
            if (question.Contains("worried") || question.Contains("scared") || question.Contains("nervous") ||
                question.Contains("afraid") || question.Contains("concerned") || question.Contains("anxious"))
                return "worried";

            // Curious sentiment
            if (question.Contains("curious") || question.Contains("wondering") || question.Contains("interested") ||
                question.Contains("learn") || question.Contains("know more") || question.Contains("understand"))
                return "curious";

            // Frustrated sentiment
            if (question.Contains("frustrated") || question.Contains("annoyed") || question.Contains("angry") ||
                question.Contains("upset") || question.Contains("mad") || question.Contains("irritated"))
                return "frustrated";

            // Happy/Positive sentiment
            if (question.Contains("happy") || question.Contains("great") || question.Contains("awesome") ||
                question.Contains("excellent") || question.Contains("wonderful") || question.Contains("fantastic"))
                return "happy";

            // Confused sentiment
            if (question.Contains("confused") || question.Contains("don't understand") || question.Contains("unclear") ||
                question.Contains("lost") || question.Contains("help me understand"))
                return "confused";

            return "neutral";
        }

        /// <summary>
        /// Gets an appropriate response based on detected sentiment.
        /// </summary>
        /// <param name="sentiment">The detected sentiment.</param>
        /// <returns>A sentiment-appropriate response prefix.</returns>
        private string GetSentimentResponse(string sentiment)
        {
            return sentiment switch
            {
                "worried" => "I understand your concerns about security - let me help ease your worries! ",
                "curious" => "I love your curiosity about cybersecurity! Let's explore this together. ",
                "frustrated" => "I can sense your frustration - let's work through this step by step. ",
                "happy" => "I'm glad you're feeling positive about learning cybersecurity! ",
                "confused" => "No worries if this seems confusing - I'll explain it clearly! ",
                _ => ""
            };
        }
    }
}

// End of file: ResponseHandler.cs
