﻿
namespace RegistrationService.Core.Interfaces
{
    public interface IQueueService<T>
    {
        void Enqueue(T item);
        bool TryDequeue(out T result);
    }
}