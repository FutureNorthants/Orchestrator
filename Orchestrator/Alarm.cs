using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator
{
    // Alarm myDeserializedClass = JsonConvert.DeserializeObject<Alarm>(myJsonResponse);
    public class Alarm
    {
        public String? AlarmName { get; set; }
        public String? AlarmDescription { get; set; }
        public String? AWSAccountId { get; set; }
        public DateTime AlarmConfigurationUpdatedTimestamp { get; set; }
        public String? NewStateValue { get; set; }
        public DateTime StateChangeTime { get; set; }
        public String? Region { get; set; }
        public String? AlarmArn { get; set; }
        public String? OldStateValue { get; set; }
        public List<String>? OKActions { get; set; }
        public List<String>? AlarmActions { get; set; }
        public List<String>? InsufficientDataActions { get; set; }
        public Trigger? Trigger { get; set; }
    }

    public class Trigger
    {
        public String? MetricName { get; set; }
        public String? Namespace { get; set; }
        public String? StatisticType { get; set; }
        public String? Statistic { get; set; }
        public String? Unit { get; set; }
        public List<String>? Dimensions { get; set; }
        public int Period { get; set; }
        public int EvaluationPeriods { get; set; }
        public int DatapointsToAlarm { get; set; }
        public String? ComparisonOperator { get; set; }
        public int Threshold { get; set; }
        public String? TreatMissingData { get; set; }
        public String? EvaluateLowSampleCountPercentile { get; set; }
    }
}
