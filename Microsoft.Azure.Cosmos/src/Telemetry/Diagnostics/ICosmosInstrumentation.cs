﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Telemetry.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Tracing;

    internal interface ICosmosInstrumentation : IDisposable
    {
        public void MarkFailed(Exception ex);

        public void AddAttribute(string key, object value);
    }
}