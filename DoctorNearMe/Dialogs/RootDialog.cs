using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace DoctorNearMe.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var returnMessage = string.Empty;

            FirstBotobject StLUIS = await GetEntityFromLUIS(activity.Text);
            if (StLUIS.intents.Length > 0)
            {
                switch (StLUIS.intents[0].intent)
                {
                    case "greeting":
                        returnMessage = "hello!";
                        break;
                    default:
                        returnMessage = "Sorry, I am not getting you...";
                        break;
                }
            }

            // return our reply to the user
            await context.PostAsync(returnMessage);

            context.Wait(MessageReceivedAsync);
        }

        private static async Task<FirstBotobject> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            FirstBotobject Data = new FirstBotobject();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/195fc44b-d6b1-44f3-a781-c1e0eeb0bac7?subscription-key=a29db6ef606c41a08704e7bfee474055&timezoneOffset=0&verbose=true&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<FirstBotobject>(JsonDataResponse);
                }
            }
            return Data;
        }
    }
}