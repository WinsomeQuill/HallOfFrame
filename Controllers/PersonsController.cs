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
        
        public PersonsController(ILogger<PersonsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Persons()
        {
            try
            {
                using (ApplicationContext db = new())
                {
                    List<Person> persons = db.Persons.Include(p => p.skills).ToList();
                    return Ok(persons);
                }
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
                using (ApplicationContext db = new())
                {
                    Person? person = db.Persons.Include(p => p.skills).FirstOrDefault(x => x.id == id);
                    return person == null ? NotFound() : Ok(person);
                }
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
                using (ApplicationContext db = new())
                {
                    db.Persons.Add(person);
                    db.SaveChanges();
                    return Ok();
                }
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
                using (ApplicationContext db = new ApplicationContext())
                {
                    Person findPerson = db.Persons.Include(p => p.skills).FirstOrDefault(x => x.id == id);
                    if (findPerson == null)
                    {
                        return NotFound("Person not found!");
                    }

                    // Проходимся по каждому скиллу
                    foreach (var skill in person.skills)
                    {
                        var findSkill = findPerson.skills.FirstOrDefault(s => s.name == skill.name);
                        // Если скилл найден, то меняем ему уровень
                        // Если нет - добавляем
                        if (findSkill != null)
                        {
                            findSkill.level = skill.level;
                        }
                        else
                        {
                            findPerson.skills.Add(skill);
                        }
                    }

                    findPerson.name = person.name;
                    findPerson.displayName = person.displayName;

                    db.SaveChanges();

                    return Ok();
                }
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
                
                using (ApplicationContext db = new())
                {
                    Person? findPerson = db.Persons.FirstOrDefault(x => x.id == id);
                    if (findPerson == null)
                    {
                        return NotFound("Person not found!");
                    }

                    db.Persons.Remove(findPerson);
                    db.SaveChanges();
                    
                    return Ok();
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                return StatusCode(500);
            }
        }
    }
}