using Benchmark.Services;
using BenchmarkDotNet.Attributes;

namespace Benchmark;

public class BenchmarkHarness
{
    private readonly StudentServiceBenchmark _restClient = new();

    [Benchmark]
    public async Task RestGetStudentPayloadAsync()
    {
        await _restClient.GetAllPayloadAsync();
    }
}