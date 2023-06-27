using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task9.Interfaces
{
    public interface IObserverWithRemove<T> : IObserver<T>
    {
        void RemoveUser(int userId);
    }

}
