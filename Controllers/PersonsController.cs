using HallOfFame_Test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace HallOfFame_Test.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly ILogger<PersonsController> _logger;
        private readonly ApplicationContext _db;
        
        public PersonsController(ILogger<PersonsController> logger, ApplicationContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IActionResult Persons()
        {
            try
            {
                List<Person> persons = _db.Persons.Include(p => p.Skills).ToList();
                return Ok(persons);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                return StatusCode(500);
            }
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Person))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Person(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id cannot negative!");
                }
                
                Log.Logger.Debug($"Get person by id: {id}");
                Person? person = _db.Persons.Include(p => p.Skills).FirstOrDefault(x => x.Id == id);
                return person == null ? NotFound() : Ok(person);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                return StatusCode(500);
            }
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreatePerson(Person person)
        {
            try
            {
                _db.Persons.Add(person);
                _db.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                return StatusCode(500);
            }
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePerson(long id, Person person)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id cannot negative!");
                }
                
                Log.Logger.Debug($"Update person by id: {id}");
                Person findPerson = _db.Persons.Include(p => p.Skills).FirstOrDefault(x => x.Id == id);
                if (findPerson == null)
                {
                    return NotFound("Person not found!");
                }

                // Проходимся по каждому скиллу
                foreach (var skill in person.Skills)
                {
                    var findSkill = findPerson.Skills.FirstOrDefault(s => s.Name == skill.Name);
                    // Если скилл найден, то меняем ему уровень
                    // Если нет - добавляем
                    if (findSkill != null)
                    {
                        findSkill.Level = skill.Level;
                    }
                    else
                    {
                        findPerson.Skills.Add(skill);
                    }
                }

                findPerson.Name = person.Name;
                findPerson.DisplayName = person.DisplayName;

                _db.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                return StatusCode(500);
            }
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeletePerson(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id cannot negative!");
                }
                
                Person? findPerson = _db.Persons.FirstOrDefault(x => x.Id == id);
                if (findPerson == null)
                {
                    return NotFound("Person not found!");
                }

                _db.Persons.Remove(findPerson);
                _db.SaveChanges();
                    
                return Ok();
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                return StatusCode(500);
            }
        }
    }
}