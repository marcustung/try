﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Try.Protocol;
using Buffer = Microsoft.DotNet.Try.Protocol.Buffer;

namespace Microsoft.DotNet.Try.Project
{
    public static class BufferGenerator
    {
        public static IEnumerable<Buffer> CreateBuffers(File file)
        {
            var viewPorts = file.ExtractViewPorts().ToList();
            if (viewPorts.Count > 0)
            {
                foreach (var viewport in viewPorts)
                {
                    yield return CreateBuffer(viewport.Region.ToString(), viewport.BufferId);
                }
            }
            else
            {
                yield return CreateBuffer(file.Text, file.Name);
            }
        }

        public static Buffer CreateBuffer(string content, BufferId id)
        {
            MarkupTestFile.GetPosition(content, out var output, out var position);

            return new Buffer(
                id,
                output,
                position ?? 0);
        }
    }
}