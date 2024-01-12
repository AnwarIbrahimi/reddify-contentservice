using AutoMapper;
using ContentService.AsyncDataServices;
using ContentService.Data;
using ContentService.DTO;
using ContentService.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace ContentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IContentRepo _repository;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public ContentsController(IConfiguration configuration, IContentRepo repository, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _configuration = configuration;
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

            var factory = new ConnectionFactory
            {
                Uri = new Uri(_configuration["RabbitMQ:Url"])
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var rabbitMQService = new RabbitMQHelper(channel);
                rabbitMQService.SendMessage($"New content created: {contentCreateDto.Name}");

                // Process the message immediately in the database
                ProcessMessageLocally(contentReadDto);
            }

            return CreatedAtRoute(nameof(GetContentById), new { Id = contentReadDto.Id }, contentReadDto);

            //try
            //{
            //    var contentPublishedDto = _mapper.Map<ContentPublishedDTO>(contentReadDto);
            //    contentPublishedDto.Event = "Content_Published";
            //    _messageBusClient.PublishNewContent(contentPublishedDto);
            //}
            //catch (Exception ex)
            //{

            //    Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            //}
        }

        private void ProcessMessageLocally(ContentReadDTO contentReadDTO)
        {
            // Process the message (e.g., create a user in the database)
            Console.WriteLine($" [x] Received 'New contents created: {contentReadDTO.Name}'");

            // Save the user to the database
            var pictureModel = _mapper.Map<Content>(contentReadDTO);
            _repository.CreateContent(pictureModel);
            _repository.SaveChanges();
        }
    }
}
