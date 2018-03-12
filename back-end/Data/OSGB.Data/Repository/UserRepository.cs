﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using OSGB.Common.Classes;
using OSGB.Common.Enums;
using OSGB.Common.Interfaces;
using OSGB.Common.Mappers.Azure.Interfaces;
using OSGB.Data.Common;
using OSGB.Data.Constants;
using User = OSGB.Data.Entity.User;

namespace OSGB.Data.Repository
{
    public class UserRepository : BaseReporsitory<User, string>
    {
        private readonly DocumentClient _documentClient;
        private readonly IDocumentResponseMapper _documentResponseMapper;

        public UserRepository(DocumentClient documentClient, IDocumentResponseMapper documentResponseMapper)
        {
            _documentClient = documentClient;
            CollectionName = Collections.Users.ToString();
            _documentResponseMapper = documentResponseMapper;
        }

        public override async Task<IReturnResult<bool>> Create(User newObject)
        {
            return await Task.FromResult(new ReturnResult<bool> {ResultValue = false});
        }

        public override async Task<IReturnResult<IEnumerable<User>>> ReadAll()
        {
            var t = Task.Run(async () =>
            {
                var query = _documentClient.CreateDocumentQuery<User>(
                        UriFactory.CreateDocumentCollectionUri(Const.DatabaseName, this.CollectionName),
                        new FeedOptions {MaxItemCount = -1, EnableCrossPartitionQuery = true})
                    .AsDocumentQuery();

                var results = new List<User>();

                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<User>());
                }

                return results;
            });

            return new ReturnResult<IEnumerable<User>>
            {
                ResultValue = await t,
                ResultType = ResultType.Success
            };
        }

        public override async Task<IReturnResult<User>> ReadById(string id)
        {
            var result = new ReturnResult<User>();
            await _documentClient.ReadDocumentAsync<User>(
                    UriFactory.CreateDocumentUri(Const.DatabaseName, CollectionName, id))
                .ContinueWith((t) =>
                {
                    if (t.IsFaulted)
                    {
                        result.AddException(t.Exception);
                        result.ResultType = ResultType.Failed;
                    }
                    else
                    {
                        var resultType = _documentResponseMapper.Identify(t.Result.StatusCode).Item1;
                        result.ResultValue = t.Result.Document;
                        result.ResultType = resultType;
                    }
                });
            return result;
        }

        public override async Task<IReturnResult<bool>> Update(User newObject)
        {
            return await Task.FromResult(new ReturnResult<bool> {ResultValue = false});
        }

        public override async Task<IReturnResult<bool>> Delete(User newObject)
        {
            return await Task.FromResult(new ReturnResult<bool> {ResultValue = false});
        }
    }
}