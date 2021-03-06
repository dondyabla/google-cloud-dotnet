﻿// Copyright 2016 Google Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.using Microsoft.VisualStudio.TestTools.UnitTesting;


using Google.Cloud.Trace.V1;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using TraceProto = Google.Cloud.Trace.V1.Trace;

namespace Google.Cloud.Diagnostics.Common.Tests
{
    public class GrpcTraceConsumerTest
    {
        private static readonly string ProjectId = "pid";

        private Traces GetTraces() => new Traces
        { Traces_ = { new TraceProto() { ProjectId = ProjectId } } };

        [Fact]
        public void Receive()
        {
            Traces traces = GetTraces();

            var mockClient = new Mock<TraceServiceClient>();
            mockClient.Setup(c => c.PatchTracesAsync(ProjectId, traces, null));
            var taskClient = Task.FromResult(mockClient.Object);
            var consumer = new GrpcTraceConsumer(taskClient);

            consumer.Receive(traces.Traces_);
            mockClient.VerifyAll();
        }

        [Fact]
        public void Receive_EmptyTracesIgnored()
        {
            var mockClient = new Mock<TraceServiceClient>();
            var taskClient = Task.FromResult(mockClient.Object);
            var consumer = new GrpcTraceConsumer(taskClient);

            consumer.Receive(new List<TraceProto>());
            mockClient.Verify(c => c.PatchTracesAsync(It.IsAny<string>(), It.IsAny<Traces>(), null), Times.Never());
        }
    }
}