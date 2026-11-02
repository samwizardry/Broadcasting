using FastEndpoints;

namespace Broadcasting.Streaming.Endpoints;

public class Streaming : Group
{
    public Streaming()
    {
        Configure("streaming", ep =>
        {
            ep.Description(r =>
            {
                r.ProducesProblemDetails();
            });
        });
    }
}
