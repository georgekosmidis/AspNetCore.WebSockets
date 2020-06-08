using System;
using System.Collections.Generic;

namespace RegistrationService.SharedKernel.Interfaces
{
    public interface IStorageService<T> where T : class, IDataModel
    {
        T Get(Guid Id);
        T AddOrUpdate(T license);
    }
}
