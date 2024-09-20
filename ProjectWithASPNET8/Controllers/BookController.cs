using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ProjectWithASPNET8.Business;
using ProjectWithASPNET8.Data.VO;
using ProjectWithASPNET8.Hypermidia.Filters;
using ProjectWithASPNET8.Model;

namespace ProjectWithASPNET8.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Route("api/[controller]/v{version:apiVersion}")]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;
        private IBookBusiness _bookBusiness;

        public BookController(ILogger<BookController> logger, IBookBusiness bookBusiness)
        {
            _logger = logger;
            _bookBusiness = bookBusiness;
        }

        //BUSCAR TODOS
        [HttpGet]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Get()
        {
            return Ok(_bookBusiness.FindAll());
        }

        //BUSCAR POR ID
        [HttpGet("{id}")]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Get(long id)
        {
            var book = _bookBusiness.FindById(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        //CREATE
        [HttpPost]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Post([FromBody] BookVO book)
        {
            if (book == null)
            {
                return BadRequest();
            }
            return Ok(_bookBusiness.Create(book));
        }

        //UPDATE
        [HttpPut]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Put([FromBody] BookVO book)
        {
            if (book == null)
            {
                return BadRequest();
            }
            return Ok(_bookBusiness.Update(book));
        }

        //DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            _bookBusiness.Delete(id);
            return NoContent();
        }
    }
}
