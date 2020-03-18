﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Resource.CosmosExceptions
{
    using System;
    using System.Net;
    using Microsoft.Azure.Cosmos.Resource.CosmosExceptions.Http.BadRequest;
    using Microsoft.Azure.Cosmos.Resource.CosmosExceptions.Http.Conflict;
    using Microsoft.Azure.Cosmos.Resource.CosmosExceptions.Http.Forbidden;
    using Microsoft.Azure.Cosmos.Resource.CosmosExceptions.Http.Gone;
    using Microsoft.Azure.Cosmos.Resource.CosmosExceptions.Http.InternalServerError;
    using Microsoft.Azure.Cosmos.Resource.CosmosExceptions.Http.NotFound;
    using Microsoft.Azure.Cosmos.Resource.CosmosExceptions.Http.RequestEntityTooLarge;
    using Microsoft.Azure.Cosmos.Resource.CosmosExceptions.Http.RequestTimeout;
    using Microsoft.Azure.Cosmos.Resource.CosmosExceptions.Http.ServiceUnavailable;

    internal static class CosmosHttpExceptionFactory
    {
        public static CosmosHttpException Create(
            HttpStatusCode httpStatusCode,
            int? subStatusCode = null,
            string message = null,
            Exception innerException = null)
        {
            CosmosHttpException exception;
            switch (httpStatusCode)
            {
                case HttpStatusCode.BadRequest:
                    exception = BadRequestExceptionFactory.Create(
                        subStatusCode,
                        message,
                        innerException);
                    break;

                case HttpStatusCode.Conflict:
                    exception = ConflictExceptionFactory.Create(
                        subStatusCode,
                        message,
                        innerException);
                    break;

                case HttpStatusCode.Forbidden:
                    exception = ForbiddenExceptionFactory.Create(
                        subStatusCode,
                        message,
                        innerException);
                    break;

                case HttpStatusCode.Gone:
                    exception = GoneExceptionFactory.Create(
                        subStatusCode,
                        message,
                        innerException);
                    break;

                case HttpStatusCode.InternalServerError:
                    exception = InternalServerErrorExceptionFactory.Create(
                        subStatusCode,
                        message,
                        innerException);
                    break;

                case HttpStatusCode.NotFound:
                    exception = NotFoundExceptionFactory.Create(
                        subStatusCode,
                        message,
                        innerException);
                    break;

                case HttpStatusCode.RequestEntityTooLarge:
                    exception = RequestEntityTooLargeExceptionFactory.Create(
                        subStatusCode,
                        message,
                        innerException);
                    break;

                case HttpStatusCode.RequestTimeout:
                    exception = RequestTimeoutExceptionFactory.Create(
                        subStatusCode,
                        message,
                        innerException);
                    break;

                case HttpStatusCode.ServiceUnavailable:
                    exception = ServiceUnavailableExceptionFactory.Create(
                        subStatusCode,
                        message,
                        innerException);
                    break;

                default:
                    exception = new UnknownCosmosHttpException(
                        httpStatusCode,
                        subStatusCode.GetValueOrDefault(0),
                        message,
                        innerException);
                    break;
            }

            return exception;
        }
    }
}
