using System.Diagnostics;
using OpenTelemetry;

namespace Hosting.OpenTelemetry;

public class ActivityIdProcessor
    : BaseProcessor<Activity>
{
    public override void OnEnd(Activity data)
    {
        data.SetTag("span.activity_id", data.Id);
    }
}