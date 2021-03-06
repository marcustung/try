// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace MLS.Agent.CommandLine
{
    public class DemoOptions
    {
        public DemoOptions(DirectoryInfo output)
        {
            Output = output;
        }

        public DirectoryInfo Output { get; }
    }
}