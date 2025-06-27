// Start of file: ResponseHandlerTests.cs
// Purpose: Unit tests for the ResponseHandler class, covering Part 2 requirements:
// keyword recognition, random responses, conversation flow, memory/recall, sentiment detection.

using NUnit.Framework;
using CybersecurityAwarenessBot;
using System.Reflection;

namespace CybersecurityAwarenessBot.Tests
{
    /// <summary>
    /// Unit tests for the ResponseHandler class.
    /// </summary>
    [TestFixture]
    public class ResponseHandlerTests
    {
        private ResponseHandler? _responder;

        /// <summary>
        /// Sets up the test environment before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _responder = new ResponseHandler();
        }

        /// <summary>
        /// Tests keyword recognition for the "password" topic.
        /// </summary>
        [Test]
        public void GetResponse_PasswordKeyword_ReturnsPasswordAdvice()
        {
            // Arrange
            var responder = new ResponseHandler();

            // Act
            string question = "Tell me about password safety";
            string response = responder.GetResponse(question.ToLower());

            // Assert
            Assert.IsTrue(response.Contains("passwords", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Tests keyword recognition for the "scam" topic.
        /// </summary>
        [Test]
        public void GetResponse_ScamKeyword_ReturnsScamAdvice()
        {
            // Arrange
            var responder = new ResponseHandler();

            // Act
            string question = "What is a scam?";
            string response = responder.GetResponse(question.ToLower());

            // Assert
            Assert.IsTrue(response.Contains("scam", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Tests handling of unknown inputs with a default response.
        /// </summary>
        [Test]
        public void GetResponse_UnknownInput_ReturnsDefaultResponse()
        {
            // Arrange
            var responder = new ResponseHandler();

            // Act
            string question = "Random question";
            string response = responder.GetResponse(question.ToLower());

            // Assert
            Assert.IsTrue(response.Contains("I didn't quite understand", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Tests sentiment detection for a "worried" sentiment.
        /// </summary>
        [Test]
        public void DetectSentiment_WorriedKeyword_ReturnsWorried()
        {
            // Arrange
            var responder = new ResponseHandler();
            string question = "I'm worried about scams";
            MethodInfo? methodInfo = typeof(ResponseHandler).GetMethod("DetectSentiment", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(methodInfo, "DetectSentiment method should exist.");

            if (methodInfo == null) // Explicit null check for the compiler
            {
                throw new InvalidOperationException("DetectSentiment method should exist.");
            }

            // Act
            object? result = methodInfo.Invoke(responder, new object[] { question.ToLower() });
            string? sentiment = result as string;

            // Assert
            Assert.IsNotNull(sentiment, "Sentiment should not be null.");
            Assert.AreEqual("worried", sentiment);
        }

        /// <summary>
        /// Tests memory/recall by retrieving the last question asked.
        /// </summary>
        [Test]
        public void GetResponse_LastQuestion_ReturnsPreviousQuestion()
        {
            // Arrange
            var responder = new ResponseHandler();
            string firstQuestion = "Tell me about passwords";
            string firstQuestionLower = firstQuestion.ToLower(); // Match Program.cs behavior

            // Act
            responder.GetResponse(firstQuestionLower);
            string response = responder.GetResponse("What was my last question?".ToLower());

            // Assert
            Assert.IsTrue(response.Contains(firstQuestionLower, StringComparison.OrdinalIgnoreCase));
        }
    }
}

// End of file: ResponseHandlerTests.cs