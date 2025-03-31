using FirebaseAdmin.Messaging;

namespace URLS.Application.ViewModels.Firebase;

public class PushResponse
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public string Message { get; set; }
    public List<SendResponse> Responses { get; set; }

    public PushResponse(BatchResponse response)
    {
        SuccessCount = response.SuccessCount;
        FailureCount = response.FailureCount;
        Responses = response.Responses.ToList();
    }

    public PushResponse(List<BatchResponse> responses)
    {
        responses.ForEach(response =>
        {
            SuccessCount += response.SuccessCount;
            FailureCount += response.FailureCount;
            Responses.AddRange(response.Responses);
        });
    }

    public PushResponse()
    {
        SuccessCount = 0;
        FailureCount = 0;
        Responses = [];
    }
}
