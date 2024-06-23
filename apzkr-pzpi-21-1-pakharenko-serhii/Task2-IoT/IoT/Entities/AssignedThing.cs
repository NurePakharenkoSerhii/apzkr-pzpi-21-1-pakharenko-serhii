using Newtonsoft.Json;

namespace IoT.Entities;

public class AssignedThing
{
    [JsonProperty("assignmentId")]
    public int assignmentId { get; init; }

    [JsonProperty("orderId")]
    public int orderId { get; init; }

    [JsonProperty("amount")]
    public int amount { get; init; }

    [JsonProperty("frequency")]
    public int frequency { get; init; }

    [JsonProperty("duration")]
    public int duration { get; init; }

    [JsonProperty("dateAssignedUTC")]
    public DateTime dateAssignedUTC { get; init; }
}
