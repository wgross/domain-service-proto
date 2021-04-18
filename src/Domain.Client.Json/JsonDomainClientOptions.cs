using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Client.Json
{
    public class JsonDomainClientOptions
    {
        public const string SectionName = "JsonDomainClient";

        public Uri DomainService { get; set; }

    }
}
