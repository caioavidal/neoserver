﻿using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using NeoServer.Benchmarks.Collections;
using NeoServer.Benchmarks.Tasks;
using System;

namespace NeoServer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugBuildConfig());
            //var summary = BenchmarkRunner.Run<BlockCopyVsSpan>();
            //var summary = BenchmarkRunner.Run<JobQueueBenchmark>();
            //var summary = BenchmarkRunner.Run<SchedulerQueueBenchmark>();
            //var summary = BenchmarkRunner.Run<ArrayPoolVsDynamicArrayBenchmark>();
            //var summary = BenchmarkRunner.Run<InstanceVsStaticBenchmark>();
            //var summary = BenchmarkRunner.Run<StringCalculationBenchmark>();
            var summary = BenchmarkRunner.Run<SchedulerBenchmark>();
            
            Console.ReadKey();
        }
    }
}
