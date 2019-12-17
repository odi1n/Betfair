using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Betfair
{
    public class AuthWeb
    {
        public static string SessionToken { get; set; }
        private readonly Regex _tmxIdRegex;

        private readonly Regex _tokenRegex;

        private Requests _request;


        /// <summary>
        /// Авторизация и получение SessionToken через  Web страницу
        public AuthWeb()
        {
            _tmxIdRegex = new Regex("name=\"tmxId\" value=\"(?<tmxId>.*?)\"");
            _tokenRegex = new Regex("name=\"productToken\" value=\"(?<token>.*?)\"");
            _request = new Requests();
        }

        /// <summary>
		/// Получить исходный код где авторизация
		/// </summary>
		/// <returns></returns>
		private async Task<string> GetPageAuth()
        {
            var httpResponse = await _request.getRequest("https://www.betfair.com/sport");

            return httpResponse.ToString();
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
            RequestParams Params = new RequestParams();
            Params["product"] = "sportsbook";
            Params["redirectMethod"] = "POST";
            Params["url"] = "https://www.betfair.com/sport/login/success";
            Params["submitForm"] = "true";
            Params["tmxId"] = tmxId;
            Params["username"] = username;
            Params["password"] = password;

            var response = await _request.postRequest("https://identitysso.betfair.com/api/login", Params);
            return response.ToString();
        }

        /// <summary>
		/// Получаем getSessionToken для вызова в дальнейшем апи.
		/// </summary>
		/// <returns></returns>
		public async Task<string> AuthBetfair(string username, string password)
        {
            _request = new Requests();
            var tmxId = await GetTmxId();
            var responseAuth = await GetAuthInfo(username, password, tmxId);

            var match = _tokenRegex.Match(responseAuth);

            if ( match.Success )
            {
                var sessionToken = match.Groups["token"].Value;

                if ( sessionToken.Length > 0 )
                {
                    SessionToken = sessionToken;
                    return sessionToken;
                }
            }
            SessionToken = null;
            return SessionToken;
        }

        public static string GetSessionsToken()
        {
            return SessionToken;
        }
    }
}
