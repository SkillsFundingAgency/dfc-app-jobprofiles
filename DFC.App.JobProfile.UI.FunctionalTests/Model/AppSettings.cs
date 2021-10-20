// <copyright file="AppSettings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DFC.TestAutomation.UI.Settings;
using System;

namespace DFC.App.JobProfile.Model
{
    internal class AppSettings : IAppSettings
    {
        public Uri AppBaseUrl { get; set; }
    }
}
