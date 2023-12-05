using AutoMapper;
using ContentService.DTO;
using ContentService.Models;

namespace ContentService.Profiles
{
    public class ContentsProfile : Profile
    {
        public ContentsProfile()
        {
            // Source -> Target
            CreateMap<Content, ContentReadDTO>();
            CreateMap<ContentCreateDTO, Content>();
            CreateMap<ContentReadDTO, ContentPublishedDTO>();
        }
    }
}
