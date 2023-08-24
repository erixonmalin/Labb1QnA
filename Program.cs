using Azure;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.TextAnalytics;
using System;

namespace question_answering
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri qnaEndpoint = new Uri("https://languageserverqnamaker.cognitiveservices.azure.com/");
            AzureKeyCredential qnaCredential = new AzureKeyCredential("e1eb069b6aef4c28bfbb1a0c393a5178");
            string qnaProjectName = "HelpFAQ";
            string qnaDeploymentName = "production";

            QuestionAnsweringClient qnaClient = new QuestionAnsweringClient(qnaEndpoint, qnaCredential);
            QuestionAnsweringProject qnaProject = new QuestionAnsweringProject(qnaProjectName, qnaDeploymentName);
            AnswersOptions qnaOptions = new AnswersOptions();
            //qnaOptions.ConfidenceThreshold = 0.85;

            Uri textAnalyticsEndpoint = new Uri("https://cogniservernlpwesteurope.cognitiveservices.azure.com/");
            AzureKeyCredential textAnalyticsCredential = new AzureKeyCredential("234ceec776184d9c8c69d10eec9d6c32");
            TextAnalyticsClient textAnalyticsClient = new TextAnalyticsClient(textAnalyticsEndpoint, textAnalyticsCredential);

            bool continueChatting = true;

            while (continueChatting)
            {
                Console.Write("Enter your question: ");
                string question = Console.ReadLine();

                Response<AnswersResult> qnaResponse = qnaClient.GetAnswers(question, qnaProject, qnaOptions);

                foreach (KnowledgeBaseAnswer answer in qnaResponse.Value.Answers)
                {
                    Console.WriteLine($"\nQ: {question}");
                    Console.WriteLine($"A: {answer.Answer}");
                    Console.WriteLine($"({answer.Confidence})\n");
                }

                AnalyzeSentiment(textAnalyticsClient, question);

                Console.Write("Do you want to ask another question or exit? (enter/exit): ");
                string userChoice = Console.ReadLine();

                if (userChoice.ToLower() == "exit")
                {
                    continueChatting = false;
                    Console.WriteLine("Exiting the chatbot...");
                }
            }
        }

        static void AnalyzeSentiment(TextAnalyticsClient client, string text)
        {
            DocumentSentiment documentSentiment = client.AnalyzeSentiment(text);

            Console.WriteLine($"Sentiment for input: {documentSentiment.Sentiment}");
            Console.WriteLine($"Positive score: {documentSentiment.ConfidenceScores.Positive}");
            Console.WriteLine($"Negative score: {documentSentiment.ConfidenceScores.Negative}");
            Console.WriteLine($"Neutral score: {documentSentiment.ConfidenceScores.Neutral}");
            Console.WriteLine();
        }
    }
}
