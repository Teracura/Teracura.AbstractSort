using Shouldly;
using Teracura.AbstractSort.Logic.Configurations;
using Teracura.AbstractSort.Logic.Sorting;

namespace Teracura.AbstractSort.Tests;

public class VersionStringSortTests
{
    private static List<string> GetSampleVersions() =>
    [
        "2.0.0.0", "1.1.2.0", "0.0.0.1", "1.0.0.0", "1.2.2.1", "1.2.1.3", "1.2.1.2", "1.2.1.1", "1.2.1.0", "2.10.2.0"
    ];

    [Fact]
    public void Should_Sort_Version_String_Ascending()
    {
        var list = GetSampleVersions();
        var config = new SortConfig<string>.Builder()
            .SortBy(x => x)
            .Mode(SortMode.Version)
            .MutateOriginal()
            .Ascending(true)
            .Build();

        var result = list.Sort(config);

        list.ShouldBe([
                "0.0.0.1",
                "1.0.0.0",
                "1.1.2.0",
                "1.2.1.0",
                "1.2.1.1",
                "1.2.1.2",
                "1.2.1.3",
                "1.2.2.1",
                "2.0.0.0",
                "2.10.2.0"
            ]
        );
    }

    [Fact]
    public void Should_Sort_Version_String_Descending()
    {
        var list = GetSampleVersions();
        var config = new SortConfig<string>.Builder()
            .SortBy(x => x)
            .Mode(SortMode.Version)
            .MutateOriginal()
            .Ascending(false)
            .Build();

        var result = list.Sort(config);

        list.ShouldBe([
            "2.10.2.0",
            "2.0.0.0",
            "1.2.2.1",
            "1.2.1.3",
            "1.2.1.2",
            "1.2.1.1",
            "1.2.1.0",
            "1.1.2.0",
            "1.0.0.0",
            "0.0.0.1"
        ]);
    }

    [Fact]
    public void Should_Sort_Version_String_And_Return_Queue()
    {
        var list = GetSampleVersions();
        var config = new SortConfig<string>.Builder()
            .SortBy(x => x)
            .Mode(SortMode.Version)
            .Ascending(true)
            .ReturnType(ReturnType.Queue)
            .Build();

        var result = list.Sort(config);

        result.ShouldBeOfType<Queue<string>>();
        result.ShouldBe(new Queue<string>([
            "0.0.0.1",
            "1.0.0.0",
            "1.1.2.0",
            "1.2.1.0",
            "1.2.1.1",
            "1.2.1.2",
            "1.2.1.3",
            "1.2.2.1",
            "2.0.0.0",
            "2.10.2.0"
        ]));
    }

    [Fact]
    public void Should_Sort_Version_String_And_Return_Stack()
    {
        var list = GetSampleVersions();
        var config = new SortConfig<string>.Builder()
            .SortBy(x => x)
            .Mode(SortMode.Version)
            .Ascending(true)
            .ReturnType(ReturnType.Stack)
            .Build();

        var result = list.Sort(config);

        result.ShouldBeOfType<Stack<string>>();
        (result).ShouldBe(new Stack<string>([
            "0.0.0.1",
            "1.0.0.0",
            "1.1.2.0",
            "1.2.1.0",
            "1.2.1.1",
            "1.2.1.2",
            "1.2.1.3",
            "1.2.2.1",
            "2.0.0.0",
            "2.10.2.0"
        ]));
    }

    [Fact]
    public void Should_Sort_Version_String_Ignoring_Letters()
    {
        List<string> list =
        [
            "1.0.2.0a",
            "1.0.2.0",
            "1.0.10.0",
            "1.0.10.1b",
            "1.0.10.1-beta",
        ];
        var config = new SortConfig<string>.Builder()
            .SortBy(x => x)
            .Mode(SortMode.Version)
            .Ascending(true)
            .MutateOriginal()
            .Build();

        var result = list.Sort(config);

        // '1.0.2.0a' should be treated as '1.0.2.0'
        list.ShouldBe([
                "1.0.2.0a",
                "1.0.2.0",
                "1.0.10.0",
                "1.0.10.1b",
                "1.0.10.1-beta"
            ]
        );
    }

    [Fact]
    public void Should_Sort_Version_String_Ignoring_Case()
    {
        List<string> list =
        [
            "1.0.0.0B",
            "1.0.0.0a",
            "1.0.0.0"
        ];
        var config = new SortConfig<string>.Builder()
            .SortBy(x => x)
            .Mode(SortMode.Version)
            .CaseSensitive(false)
            .Ascending(true)
            .MutateOriginal()
            .Build();

        var result = list.Sort(config);

        list.ShouldBe([
                "1.0.0.0B",
                "1.0.0.0a",
                "1.0.0.0"
            ]
        );
    }
}