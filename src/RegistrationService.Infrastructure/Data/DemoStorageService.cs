using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RegistrationService.SharedKernel.Interfaces;

namespace RegistrationService.Services
{
    //Demo storage just for the exercise, there is no real persistence
    public class DemoStorageService<T> : IStorageService<T> where T : class, IDataModel
    {
        private readonly ConcurrentDictionary<Guid, T> db;

        public DemoStorageService()
        {
            db = new ConcurrentDictionary<Guid, T>();
        }

        public T Get(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return db.FirstOrDefault(x => x.Key == id).Value;
        }

        public T AddOrUpdate(T license)
        {
            if (license.Id == null)
            {
                throw new ArgumentNullException(nameof(license.Id));
            }

            db.AddOrUpdate(license.Id, license, (k, v) => license);

            return Get(license.Id);
        }


    }
}
