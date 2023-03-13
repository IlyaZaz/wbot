using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace Program
{
	internal class Program
	{
		public static List<string> list = new List<string>();
		private static void Main(string[] args)
		{
			var client = new TelegramBotClient("5820345897:AAG2bXJhWYmYTdvtBcvQU2fphWzq4ktDClM");
			client.StartReceiving(Reply2Message, Error);
			Console.ReadLine();
		}
		private static async Task Reply2Message(ITelegramBotClient botClient, Update update, CancellationToken token)
		{
			var message = update.Message;
			switch (message.Text)
			{
				case "/start":
					await botClient.SendTextMessageAsync(message.Chat.Id, $"Кидай ссылку на товар, чтобы я заработал");
					break;
				default:
					int article = GetArticleValue(message.Text);
					var urlMid = $"https://card.wb.ru/cards/detail?nm={article}";
					int root = GetRootValue(urlMid);
					IfNull(root);
					string feedbackCount = list[list.IndexOf("feedbackCount") + 1];
					string rate1 = list[list.IndexOf("valuationDistribution") + 2];
					string rate2 = list[list.IndexOf("valuationDistribution") + 4];
					string rate3 = list[list.IndexOf("valuationDistribution") + 6];
					string rate4 = list[list.IndexOf("valuationDistribution") + 8];
					string rate5 = list[list.IndexOf("valuationDistribution") + 10];
					await botClient.SendTextMessageAsync(message.Chat.Id, $"Количетсво отзывов {feedbackCount}");
					await botClient.SendTextMessageAsync(message.Chat.Id, $"С 1 звездой {rate1}");
					await botClient.SendTextMessageAsync(message.Chat.Id, $"С 2 звездами {rate2}");
					await botClient.SendTextMessageAsync(message.Chat.Id, $"С 3 звездами {rate3}");
					await botClient.SendTextMessageAsync(message.Chat.Id, $"С 4 звездами {rate4}");
					await botClient.SendTextMessageAsync(message.Chat.Id, $"С 5 звездами {rate5}");
					list.Clear();
					break;
			}
			
		}
		private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
		{
			throw new NotImplementedException();
		}
		private static int GetArticleValue(string Url)
		{
			int article;
			int.TryParse(string.Join("", Url.Where(c => char.IsDigit(c))), out article);
			return article;
		}
		private static int GetRootValue(string url)
		{
			var httpClient = new HttpClient();
			var html = httpClient.GetStringAsync(url);
			var rootList = new List<string>();
			int rootValue;
			foreach (var s in html.Result.Split(',', ':', '{', '}', '[', '"', ']'))
			{
				rootList.Add(s);
			}

			rootList.RemoveAll(s => s == "");

			if (rootList.Contains("root"))
			{
				int index = rootList.IndexOf("root");
				rootValue = Convert.ToInt32(rootList[index + 1]);
				return rootValue;
			}
			return 0;
		}
		private static void FillingList(string url)
		{
			var httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
			var html = httpClient.GetStringAsync(url);
			foreach (var s in html.Result.Split(',', ':', '{', '}', '[', '"', ']'))
			{
				list.Add(s);
			}
			list.RemoveAll(s => s == "");
		}
		private static void IfNull(int root)
		{
			string urlDB1 = $"https://feedbacks1.wb.ru/feedbacks/v1/{root}";
			string urlDB2 = $"https://feedbacks2.wb.ru/feedbacks/v1/{root}";
			FillingList(urlDB1);
			if (list[list.IndexOf("valuationDistribution") + 1] == "null")
			{
				list.Clear();
				FillingList(urlDB2);
			}

		}
	}
}