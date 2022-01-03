﻿using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Mock.Core.Items
{
    public class MockCoreUrl : ICoreUrl
    {
        public MockCoreUrl(ICore sourceCore, Uri uri, string label)
        {
            Label = label;
            Url = uri;
            SourceCore = sourceCore;
        }

        public string Label { get; set; }

        public Uri Url { get; set; }

        public UrlType Type { get; set; }

        public ICore SourceCore { get; set; }

        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}