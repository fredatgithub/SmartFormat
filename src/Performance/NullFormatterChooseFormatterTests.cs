﻿using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SmartFormat.Core.Parsing;
using SmartFormat.Extensions;

namespace SmartFormat.Performance
{
    /*
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET Core SDK=5.0.203
  [Host]        : .NET Core 5.0.6 (CoreCLR 5.0.621.22011, CoreFX 5.0.621.22011), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.6 (CoreCLR 5.0.621.22011, CoreFX 5.0.621.22011), X64 RyuJIT

Job=.NET Core 5.0  Runtime=.NET Core 5.0

|           Method |     Mean |   Error |  StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------- |---------:|--------:|--------:|-------:|------:|------:|----------:|
| ChooseFormatTest | 854.0 ns | 7.29 ns | 6.82 ns | 0.1411 |     - |     - |    1184 B |
|   NullFormatTest | 553.5 ns | 3.02 ns | 2.67 ns | 0.1030 |     - |     - |     864 B |
    */

    [SimpleJob(RuntimeMoniker.Net50)]
    [MemoryDiagnoser]
    public class NullFormatterChooseFormatterTests
    {
        // Members get initialized in the Setup method
        private SmartFormatter _smartNullFormatter = null!, _smartChooseFormatter = null!;
        private Format _nullFormatCache = null!, _chooseFormatCache = null!;

        [GlobalSetup]
        public void Setup()
        {
            _smartNullFormatter = new SmartFormatter();
            _smartNullFormatter.AddExtensions(new DefaultSource());
            _smartNullFormatter.AddExtensions(new NullFormatter(), new DefaultFormatter());

            _smartChooseFormatter = new SmartFormatter();
            _smartChooseFormatter.AddExtensions(new DefaultSource());
            _smartChooseFormatter.AddExtensions(new ChooseFormatter(), new DefaultFormatter());

            _nullFormatCache = _smartNullFormatter.Parser.ParseFormat("{0:isnull:nothing}");
            _chooseFormatCache = _smartChooseFormatter.Parser.ParseFormat("{0:choose(null):nothing|}");
        }

        [Benchmark]
        public void ChooseFormatTest()
        {
            var result = _smartChooseFormatter.Format(_chooseFormatCache, "", new List<object?> {null});
        }

        [Benchmark]
        public void NullFormatTest()
        {
            var result = _smartNullFormatter.Format(_nullFormatCache, "", new List<object?> {null});
        }
    }
}