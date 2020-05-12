﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.Cosmos.Diagnostics;

    /// <summary>
    /// This represents the core diagnostics object used in the SDK.
    /// This object gets created on the initial request and passed down
    /// through the pipeline appending information as it goes into a list
    /// where it is lazily converted to a JSON string.
    /// </summary>
    internal sealed class CosmosDiagnosticsContextCore : CosmosDiagnosticsContext
    {
        /// <summary>
        /// Detailed view of all the operations.
        /// </summary>
        private List<CosmosDiagnosticsInternal> ContextList { get; }

        private static readonly string DefaultUserAgentString;

        private readonly CosmosDiagnosticScope overallScope;

        private bool IsDefaultUserAgent = true;

        static CosmosDiagnosticsContextCore()
        {
            // Default user agent string does not contain client id or features.
            UserAgentContainer userAgentContainer = new UserAgentContainer();
            CosmosDiagnosticsContextCore.DefaultUserAgentString = userAgentContainer.UserAgent;
        }

        public CosmosDiagnosticsContextCore()
        {
            this.StartUtc = DateTime.UtcNow;
            this.ContextList = new List<CosmosDiagnosticsInternal>();
            this.Diagnostics = new CosmosDiagnosticsCore(this);
            this.overallScope = new CosmosDiagnosticScope("Overall");
        }

        public override DateTime StartUtc { get; }

        public override int TotalRequestCount { get; protected set; }

        public override int FailedRequestCount { get; protected set; }

        public override string UserAgent { get; protected set; } = CosmosDiagnosticsContextCore.DefaultUserAgentString;

        public override CosmosDiagnostics Diagnostics { get; }

        public override TimeSpan GetClientElapsedTime()
        {
            return this.overallScope.GetElapsedTime();
        }

        public override bool IsComplete()
        {
            return this.overallScope.IsComplete();
        }

        public override IDisposable GetOverallScope()
        {
            return this.overallScope;
        }

        public override IDisposable CreateScope(string name)
        {
            CosmosDiagnosticScope scope = new CosmosDiagnosticScope(name);

            this.ContextList.Add(scope);
            return scope;
        }

        public override IDisposable CreateRequestHandlerScopeScope(RequestHandler requestHandler)
        {
            RequestHandlerScope requestHandlerScope = new RequestHandlerScope(requestHandler);
            this.ContextList.Add(requestHandlerScope);
            return requestHandlerScope;
        }

        public override void AddDiagnosticsInternal(PointOperationStatistics pointOperationStatistics)
        {
            if (pointOperationStatistics == null)
            {
                throw new ArgumentNullException(nameof(pointOperationStatistics));
            }

            this.AddRequestCount((int)pointOperationStatistics.StatusCode);

            this.ContextList.Add(pointOperationStatistics);
        }

        public override void AddDiagnosticsInternal(StoreResponseStatistics storeResponseStatistics)
        {
            if (storeResponseStatistics.StoreResult != null)
            {
                this.AddRequestCount((int)storeResponseStatistics.StoreResult.StatusCode);
            }

            this.ContextList.Add(storeResponseStatistics);
        }

        public override void AddDiagnosticsInternal(AddressResolutionStatistics addressResolutionStatistics)
        {
            this.ContextList.Add(addressResolutionStatistics);
        }

        public override void AddDiagnosticsInternal(CosmosClientSideRequestStatistics clientSideRequestStatistics)
        {
            this.ContextList.Add(clientSideRequestStatistics);
        }

        public override void AddDiagnosticsInternal(FeedRangeStatistics feedRangeStatistics)
        {
            this.ContextList.Add(feedRangeStatistics);
        }

        public override void AddDiagnosticsInternal(QueryPageDiagnostics queryPageDiagnostics)
        {
            if (queryPageDiagnostics == null)
            {
                throw new ArgumentNullException(nameof(queryPageDiagnostics));
            }

            if (queryPageDiagnostics.DiagnosticsContext != null)
            {
                this.AddSummaryInfo(queryPageDiagnostics.DiagnosticsContext);
            }

            this.ContextList.Add(queryPageDiagnostics);
        }

        public override void AddDiagnosticsInternal(CosmosDiagnosticsContext newContext)
        {
            this.AddSummaryInfo(newContext);

            this.ContextList.AddRange(newContext);
        }

        public override void SetSdkUserAgent(string userAgent)
        {
            this.IsDefaultUserAgent = false;
            this.UserAgent = userAgent;
        }

        public override void Accept(CosmosDiagnosticsInternalVisitor cosmosDiagnosticsInternalVisitor)
        {
            cosmosDiagnosticsInternalVisitor.Visit(this);
        }

        public override TResult Accept<TResult>(CosmosDiagnosticsInternalVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public override IEnumerator<CosmosDiagnosticsInternal> GetEnumerator()
        {
            // Using a for loop with a yield prevents Issue #1467 which causes
            // ThrowInvalidOperationException if a new diagnostics is getting added
            // while the enumerator is being used.
            for (int i = 0; i < this.ContextList.Count; i++)
            {
                yield return this.ContextList[i];
            }
        }

        private void AddRequestCount(int statusCode)
        {
            this.TotalRequestCount++;
            if (statusCode < 200 || statusCode > 299)
            {
                this.FailedRequestCount++;
            }
        }

        private void AddSummaryInfo(CosmosDiagnosticsContext newContext)
        {
            if (Object.ReferenceEquals(this, newContext))
            {
                return;
            }

            if (this.IsDefaultUserAgent && newContext.UserAgent != null)
            {
                this.SetSdkUserAgent(newContext.UserAgent);
            }

            this.TotalRequestCount += newContext.TotalRequestCount;
            this.FailedRequestCount += newContext.FailedRequestCount;
        }
    }
}