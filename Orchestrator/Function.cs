using Amazon.Lambda.Core;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Metrics;
using AWS.Lambda.Powertools.Tracing;
using Amazon.Lambda.SNSEvents;
using Amazon.EC2.Model;
using Amazon.EC2;
using System.Text.Json;
using System.Linq.Expressions;

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
        Logger.LogInformation($"Processed record {record.Sns.Message}");
        Alarm? alarm = JsonSerializer.Deserialize<Alarm>(record.Sns.Message);
        Metrics.AddMetric("Alarm Raised", 1, MetricUnit.Count);
        switch (alarm?.AlarmName)
        {
            case "CXM2Veolia":
                if (alarm.NewStateValue.Equals("ALARM"))
                {
                    await RebootInstances("i-0ce23d0464728520f");
                }
                break;
            default:
                Logger.LogWarning($"Unknown AlarmName : " + alarm?.AlarmName);
                Metrics.AddMetric("Unknown Alarm", 1, MetricUnit.Count);
                break;
        }
        //await Task.CompletedTask;
    }
    [Tracing(SegmentName = "CXM2VeoliaHandler Method")]
    public async Task CXM2VeoliaHandler(Alarm alarm)
    {
        switch (alarm.NewStateValue)
            {
            case "ALARM":
                await RebootInstances("i-0ce23d0464728520f");
                break;
            case "OK":
                Metrics.AddMetric("Alarm Resolved", 1, MetricUnit.Count);
                break;
            default:
                Logger.LogWarning($"Unknown AlarmState : " + alarm.AlarmName + "/" + alarm.NewStateValue);
                Metrics.AddMetric("Unknown AlarmState", 1, MetricUnit.Count);
                break;
            }
    }

    [Tracing(SegmentName = "RebootInstances Method")]
    public async Task RebootInstances(string ec2InstanceId)
    {
        AmazonEC2Client client = new AmazonEC2Client();

        var request = new RebootInstancesRequest
        {
            InstanceIds = new List<string> { ec2InstanceId },
        };

        var response = await client.RebootInstancesAsync(request);
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            Logger.LogInformation($"{ec2InstanceId} successfully rebooted.");
            Metrics.AddMetric("Rebooted", 1, MetricUnit.Count);
        }
        else
        {
            Logger.LogError($"{ec2InstanceId} could not be rebooted");
            Metrics.AddMetric("Reboot failure", 1, MetricUnit.Count);
        }
    }
}
