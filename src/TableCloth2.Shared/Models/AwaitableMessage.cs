namespace TableCloth2.Shared.Models;

public sealed record AwaitableMessage<TInput, TOutput>(TInput Content, TaskCompletionSource<TOutput> CompletionSource);

public sealed record AwaitableMessage<TInput>(TInput Content, TaskCompletionSource CompletionSource);

public sealed record AwaitableMessage(TaskCompletionSource CompletionSource);
