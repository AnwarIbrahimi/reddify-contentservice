using ContentService.DTO;

namespace ContentService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishNewContent(ContentPublishedDTO contentPublishedDTO);
    }
}
