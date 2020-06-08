using RegistrationService.SharedKernel.Interfaces;
using System.Collections.Concurrent;
using System.Linq;

namespace RegistrationService.Services
{
    //Demo implemantation of a queue, there is no persistence
    public class DemoQueueService<T> : IQueueService<T>
    {
        private ConcurrentQueue<T> queque;
        public DemoQueueService() => queque = new ConcurrentQueue<T>();
        public void Enqueue(T item) => queque.Enqueue(item);
        public bool TryDequeue(out T result) => queque.TryDequeue(out result);

    }
}
