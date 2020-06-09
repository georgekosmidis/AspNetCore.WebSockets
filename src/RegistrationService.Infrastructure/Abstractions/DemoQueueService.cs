using RegistrationService.Core.Interfaces;
using System.Collections.Concurrent;

namespace RegistrationService.Infrastructure.Abstractions
{
    //Demo implemantation of a queue, there is no persistence
    public abstract class DemoQueueService<T> : IQueueService<T>
    {
        private ConcurrentQueue<T> queque;
        public DemoQueueService() => queque = new ConcurrentQueue<T>();
        public void Enqueue(T item) => queque.Enqueue(item);
        public bool TryDequeue(out T result) => queque.TryDequeue(out result);

    }
}
