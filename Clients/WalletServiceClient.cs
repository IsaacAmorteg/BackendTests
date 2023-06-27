﻿using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task9.Extensions;
using Task9.Interfaces;
using Task9.Models.Requests;
using Task9.Models.Responses.Base;

namespace Task9.Clients
{
    public class WalletServiceClient
    {
        //private static readonly Lazy<WalletServiceClient> _lazy = new Lazy<WalletServiceClient>(() => new WalletServiceClient());

       // public static WalletServiceClient Instance => _lazy.Value;

        private readonly HttpClient _client = new HttpClient();
        private readonly string _baseUrl = "https://walletservice-uat.azurewebsites.net";

        public async Task<CommonResponse<decimal>> GetBalance(Int32 userId)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_baseUrl}/Balance/GetBalance?userId={userId}"),
            };
            HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);

            return await response.ToCommonResponse<decimal>();
        }

        public async Task<CommonResponse<Guid>> BalanceCharge(BalanceChargeRequest request)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/Balance/Charge"),
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);

            if (response.IsSuccessStatusCode)
            {
                UserServiceClient.Instance.NotifyUserDeleted(request.userId);
            }

            return await response.ToCommonResponse<Guid>();
        }
       /* private readonly ConcurrentBag<IObserverWithRemove<int>> _observers = new ConcurrentBag<IObserverWithRemove<int>>();
        public IDisposable Subscribe(IObserver<int> observer)
        {
            _observers.Add((IObserverWithRemove<int>)observer);

            return null;
        }
        public void NotifyAllObservers(int userId)
        {
            foreach (IObserverWithRemove<int> observer in _observers)
            {
                observer.OnNext(userId);
            }
        }
        public void NotifyUserWithTransactions(int userId)
        {
            foreach (IObserverWithRemove<int> observer in _observers)
            {
                observer.RemoveUser(userId);
            }
        }
       */
    }
}
