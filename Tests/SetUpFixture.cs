using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task9.Clients;
using Task9.Interfaces;

namespace Task9.Tests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        private TestDataObserver _observer;
        private readonly UserServiceClient _client = UserServiceClient.Instance;
       
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _observer = new TestDataObserver();
            _client.Subscribe(_observer);          
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {

            var tasks = _observer.GetUsersToDelete()
                        .Select(user => _client.DeleteUser(user));

            await Task.WhenAll(tasks);

        }
    }

    public class TestDataObserver : IObserverWithRemove<int>
    {
        private readonly ConcurrentBag<int> _storage = new ConcurrentBag<int>();
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(int value)
        {
            _storage.Add(value);
        }

        public IEnumerable<int> GetUsersToDelete()
        {
            return _storage.ToArray();
        }

        public void RemoveUser(int userId)
        {
            _storage.TryTake(out userId);
        }
    }
}


