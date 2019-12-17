using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Betfair
{
	public class AuthWebss
	{
        public static string SessionToken;
		private readonly Regex _tmxIdRegex;

        private readonly Regex _tokenRegex;

		private readonly HttpClient _httpClient;

		/// <summary>
		/// Авторизация и получение SessionToken через  Web страницу
		public AuthWebss()
		{
			_tmxIdRegex = new Regex("name=\"tmxId\" value=\"(?<tmxId>.*?)\"");
			_tokenRegex = new Regex("name=\"productToken\" value=\"(?<token>.*?)\"");
			_httpClient = new HttpClient();
		}

		/// <summary>
		/// Получить исходный код где авторизация
		/// </summary>
		/// <returns></returns>
		private async Task<string> GetPageAuth()
		{
			var httpResponse = await _httpClient.GetAsync("https://www.betfair.com/sport");

			return await httpResponse.Content.ReadAsStringAsync();
		}

		/// <summary>
		/// Парсим значения для дальнейшей авторизации
		/// </summary>
		private async Task<string> GetTmxId()
		{
			var responses = await GetPageAuth();
			var tmxId = _tmxIdRegex.Match(responses).Groups["tmxId"].Value;
			return tmxId;
		}

		/// <summary>
		/// Получаем страницу авторизации
		/// </summary>
		private async Task<string> GetAuthInfo(string username, string password, string tmxId)
		{
			var httpResponse = await _httpClient.PostAsync("https://identitysso.betfair.com/api/login",
														   new FormUrlEncodedContent(new[]
														   {
															   new KeyValuePair<string, string>("product", "sportsbook"),
															   new KeyValuePair<string, string>("redirectMethod", "POST"),
															   new KeyValuePair<string, string>("url", "https://www.betfair.com/sport/login/success"),
															   new KeyValuePair<string, string>("submitForm", "true"),
															   new KeyValuePair<string, string>("tmxId", tmxId),
															   new KeyValuePair<string, string>("username", username),
															   new KeyValuePair<string, string>("password", password),
														   }));

			return await httpResponse.Content.ReadAsStringAsync();
		}

		/// <summary>
		/// Получаем getSessionToken для вызова в дальнейшем апи.
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetSessionToken(string username, string password)
		{
			var tmxId = await GetTmxId();
			var responseAuth = await GetAuthInfo(username, password, tmxId);

			var match = _tokenRegex.Match(responseAuth);

			if (match.Success)
			{
				var sessionToken = match.Groups["token"].Value;

				if (sessionToken.Length > 0)
				{
                    SessionToken = sessionToken;
					return sessionToken;
				}
			}
            SessionToken = null;
            return null;
		}
	}
}