using AutoMapper;
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

        public ContentsController(IContentRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
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

        [HttpPost]
        public ActionResult<ContentReadDTO> CreateContent(ContentCreateDTO contentCreateDto)
        {
            var contentModel = _mapper.Map<Content>(contentCreateDto);
            _repository.CreateContent(contentModel);
            _repository.SaveChanges();

            var contentReadDto = _mapper.Map<ContentReadDTO>(contentModel);

            return CreatedAtRoute(nameof(GetContentById), new { Id = contentReadDto.Id }, contentReadDto);
        }
    }
}
