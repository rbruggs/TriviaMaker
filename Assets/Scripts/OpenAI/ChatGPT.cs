using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button button;
        
        [SerializeField] private TMP_Text tmpTopic;
        [SerializeField] private TMP_Text tmpQuestion;

        [SerializeField] private GameObject inputPanel;
        [SerializeField] private GameObject answerPanel;        

        [SerializeField] private TMP_Text a1ButtonText;
        [SerializeField] private TMP_Text a2ButtonText;
        [SerializeField] private TMP_Text a3ButtonText;
        [SerializeField] private TMP_Text a4ButtonText;

        private string question;
        private string a1;
        private string a2;
        private string a3;
        private string a4;
        private string correctAnswer;

        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "You are a trivia game host for adults. You will always reply with a question. You will give four answers to pick from based on the topic given by the user. Don't break character. Don't ever mention that you are an AI model." +
            " Here is the template you will respond with. \n" +
            "Question: Here where you place the question.\n" +
            "|A) the first answer\n" +
            "|B) second answer\n" +
            "|C) third answer\n" +
            "|D) and the fourth answer\n" +
            "|B the answer";

        private void Start()
        {
            button.onClick.AddListener(SendReply);
        }

        private void AppendMessage(ChatMessage message)
        {
            string[] splitMessage = message.Content.Split('|');
            tmpQuestion.text = splitMessage[0];
            tmpQuestion.gameObject.SetActive(true);
            answerPanel.SetActive(true);
            a1ButtonText.text = splitMessage[1];
            a2ButtonText.text = splitMessage[2];
            a3ButtonText.text = splitMessage[3];
            a4ButtonText.text = splitMessage[4];
            correctAnswer = splitMessage[5];
            Debug.Log(correctAnswer);
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text                
            };           

            tmpTopic.text = "Topic: " + inputField.text;

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(newMessage);
            button.enabled = false;
            inputField.text = "Pending";
            inputField.enabled = false;
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                messages.Add(message);
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            //button.enabled = true;
            //inputField.enabled = true;
            inputPanel.SetActive(false);
            
        }

        public void CheckAnswer(string answer)
        {
            Debug.Log(correctAnswer.Substring(0, 1));
            if(answer == correctAnswer.Substring(0, 1))
            {
                Debug.Log("Correct");
            }
            else
            {
                Debug.Log("WRONG!");
            }
        }
    }
}
