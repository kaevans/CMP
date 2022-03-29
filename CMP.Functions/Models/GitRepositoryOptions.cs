﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Functions.Models
{
    public class GitRepositoryOptions
    {
        public const string SectionName = "AzureDevOps";
        public string PersonalAccessToken { get; set; }
        public string CollectionUri { get; set; }
        public string Organization { get; set; }
        public string Project { get; set; }
        public string Repository { get; set; }

        public string GetOrganizationUri()
        {
            return string.Format(CollectionUri, Organization);
        }
    }
}
