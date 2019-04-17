﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.DotNet.Try.Protocol;
using Microsoft.DotNet.Try.Protocol.Tests;
using Xunit;
using Buffer = Microsoft.DotNet.Try.Protocol.Buffer;

namespace Microsoft.DotNet.Try.Project.Tests
{
    public class CodeMergeTransformerTests
    {
        [Fact]
        public void When_workspace_is_null_then_the_transformer_throw_exception()
        {
            var processor = new CodeMergeTransformer();
            Func<Task> extraction = () => processor.TransformAsync(null);
            extraction.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async void Files_are_merged_in_order()
        {
            var processor = new CodeMergeTransformer();
            var workspace = new Workspace(
                workspaceType: "console",
                files: new[]
                {
                    new File("fileA.cs", "first line;", 0),
                    new File("fileA.cs", "third line;", 2),
                    new File("fileA.cs", "second line;", 1),

                    new File("fileB.cs", "first line;", 2),
                    new File("fileB.cs", "third line;", 0),
                    new File("fileB.cs", "second line;", 1)
                }
            );
            var processed = await processor.TransformAsync(workspace);

            processed.Should().NotBeNull();

            var fileA = processed.Files.Single(f => f.Name == "fileA.cs");
            fileA.Text.Should()
                .Match(@"first line;
second line;
third line;".EnforceLF());

            var fileB = processed.Files.Single(f => f.Name == "fileB.cs");
            fileB.Text.Should()
                .Match(@"third line;
second line;
first line;".EnforceLF());
        }

        [Fact]
        public async void buffers_are_merged_in_order()
        {
            var processor = new CodeMergeTransformer();
            var workspace = new Workspace(
                workspaceType: "console",
                buffers: new[]
                {
                    new Buffer(new BufferId("fileA.cs", "regionA"), "first line;", order:0),
                    new Buffer(new BufferId("fileA.cs", "regionA"), "third line;", order:2),
                    new Buffer(new BufferId("fileA.cs", "regionA"), "second line;", order:1),

                    new Buffer(new BufferId("fileA.cs", "regionB"), "fourth line;", order:0),
                    new Buffer(new BufferId("fileA.cs", "regionB"), "sixth line;", order:2),
                    new Buffer(new BufferId("fileA.cs", "regionB"), "fifth line;", order:1),

                    new Buffer("fileB.cs", "first line;", order:2),
                    new Buffer("fileB.cs", "third line;", order:0),
                    new Buffer("fileB.cs", "second line;", order:1)
                }
            );
            var processed = await processor.TransformAsync(workspace);

            processed.Should().NotBeNull();

            var bufferARegionA = processed.Buffers.Single(f => f.Id == new BufferId("fileA.cs", "regionA"));
            bufferARegionA.Content.Should()
                .Match(@"first line;
second line;
third line;".EnforceLF());



            var bufferARegionB = processed.Buffers.Single(f => f.Id == new BufferId("fileA.cs", "regionB"));
            bufferARegionB.Content.Should()
                .Match(@"fourth line;
fifth line;
sixth line;".EnforceLF());

            var bufferB = processed.Buffers.Single(f => f.Id == "fileB.cs");
            bufferB.Content.Should()
                .Match(@"third line;
second line;
first line;".EnforceLF());
        }

    }
}