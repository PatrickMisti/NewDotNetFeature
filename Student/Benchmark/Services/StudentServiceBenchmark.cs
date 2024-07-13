using System.Net.Http.Headers;
using System.Net.Http.Json;
using Student.Dtos;

namespace Benchmark.Services;

public class StudentServiceBenchmark
{
    private static readonly HttpClient _client = new();

    public async Task<IList<StudentDto>> GetAllPayloadAsync()
    {
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return await _client.GetFromJsonAsync<IList<StudentDto>>($"http://localhost:5023/api/student/all");
    }
}