﻿using DFC.App.JobProfile.Data.Models;
using dfc_content_pkg_netcore.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IPagesApiDataModel
    {
        ContentLinksModel? ContentLinks { get; set; }

        IList<ApiContentItemModel> ContentItems { get; set; }
    }
}