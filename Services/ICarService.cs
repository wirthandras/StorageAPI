﻿using StorageAPI.Model;
using StorageAPI.Responses;

namespace StorageAPI.Services
{
    public interface ICarService
    {
        public Task Save(StorageRequest request);


        public Task<KeyWordResponse> GetKeyWords();
        public Task<LatestChangesResponse> LatestChanges(int limit = 10, string search = "");

        public Task ReCheckCars();
    }
}
