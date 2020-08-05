﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IApiDataProcessorService
    {
        Task<TApiModel> GetAsync<TApiModel>(HttpClient? httpClient, Uri url)
            where TApiModel : class;

        Task<HttpStatusCode> PostAsync<TModel>(HttpClient? httpClient, Uri url, TModel model)
            where TModel : class;

        Task<HttpStatusCode> DeleteAsync(HttpClient? httpClient, Uri url);
    }
}
