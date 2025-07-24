using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace BackToTheDawnTrainer;

public interface IIl2CppLocator
{
	Dictionary<string, long> MethodRVAs { get; }

	bool CheckRequiresDump(DateTime? oldTime, [NotNullWhen(true)] out DateTime? newTime);

	Task DumpAsync(CancellationToken token);

	Task LocateAsync(CancellationToken token);
}
