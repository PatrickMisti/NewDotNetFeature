// See https://aka.ms/new-console-template for more information

using Benchmark;
using BenchmarkDotNet.Running;

Console.WriteLine("Start Benchmarks");

BenchmarkRunner.Run<BenchmarkHarness>();

Console.ReadLine();
