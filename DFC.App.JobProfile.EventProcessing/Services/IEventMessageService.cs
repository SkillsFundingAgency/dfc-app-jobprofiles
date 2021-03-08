﻿using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.EventProcessing.Services
{
    public interface IEventMessageService<TModel>
        where TModel : class, IContentPageModel
    {
        Task<IList<TModel>> GetAllCachedCanonicalNamesAsync();

        Task<HttpStatusCode> CreateAsync(TModel upsertDocumentModel);

        Task<HttpStatusCode> UpdateAsync(TModel upsertDocumentModel);

        Task<HttpStatusCode> DeleteAsync(Guid id);
    }
}