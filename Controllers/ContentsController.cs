using AutoMapper;
using ContentService.AsyncDataServices;
using ContentService.Data;
using ContentService.DTO;
using ContentService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly IContentRepo _repository;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public ContentsController(IContentRepo repository, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<ContentReadDTO>> GetContents()
         {
            Console.WriteLine("--> Getting Contents....");

            var contentItem = _repository.GetAllContents();

            return Ok(_mapper.Map<IEnumerable<ContentReadDTO>>(contentItem));   
        }

        [HttpGet("{id}", Name = "GetContentById")]
        public ActionResult<ContentReadDTO> GetContentById(int id)
        {
            var contentItem = _repository.GetContentById(id);
            if (contentItem != null)
            {
                return Ok(_mapper.Map<ContentReadDTO>(contentItem));
            }

            return NotFound();
        }

        [HttpPost("post")]
        public ActionResult<ContentReadDTO> CreateContent(ContentCreateDTO contentCreateDto)
        {
            var contentModel = _mapper.Map<Content>(contentCreateDto);
            _repository.CreateContent(contentModel);
            _repository.SaveChanges();

            var contentReadDto = _mapper.Map<ContentReadDTO>(contentModel);

            try
            {
                var contentPublishedDto = _mapper.Map<ContentPublishedDTO>(contentReadDto);
                contentPublishedDto.Event = "Content_Published";
                _messageBusClient.PublishNewContent(contentPublishedDto);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetContentById), new { Id = contentReadDto.Id }, contentReadDto);
        }
    }
}
