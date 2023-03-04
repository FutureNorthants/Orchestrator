using Amazon.Lambda.Core;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Metrics;
using AWS.Lambda.Powertools.Tracing;
using Amazon.Lambda.SNSEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Orchestrator;

/// <summary>
/// Learn more about Lambda Powertools at https://awslabs.github.io/aws-lambda-powertools-dotnet/
/// Lambda Powertools Logging: https://awslabs.github.io/aws-lambda-powertools-dotnet/core/logging/
/// Lambda Powertools Tracing: https://awslabs.github.io/aws-lambda-powertools-dotnet/core/tracing/
/// Lambda Powertools Metrics: https://awslabs.github.io/aws-lambda-powertools-dotnet/core/metrics/
/// Metrics Namespace and Service can be defined through Environment Variables https://awslabs.github.io/aws-lambda-powertools-dotnet/core/metrics/#getting-started
/// </summary>
public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    [Logging(LogEvent = true)]
    [Metrics(CaptureColdStart = true)]
    [Tracing]
    public async Task FunctionHandlerAsync(SNSEvent evnt, ILambdaContext context)
    {
        Logger.LogInformation($"Orchestrating...");
        Metrics.AddMetric("Ochestrations", 1, MetricUnit.Count);

        foreach (var record in evnt.Records)
        {
            await ProcessRecordAsync(record, context);
        }
    }

    [Tracing(SegmentName = "ProcessRecordAdync Method")]
    private async Task ProcessRecordAsync(SNSEvent.SNSRecord record, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processed record {record.Sns.Message}");

        // TODO: Do interesting work based on the new message
        await Task.CompletedTask;
    }
}
