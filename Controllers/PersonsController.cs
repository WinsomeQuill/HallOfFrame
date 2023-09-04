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
        public IActionResult Person(long id)
        {
            try
            {
                Log.Logger.Debug($"Get person by id: {id}");
                using (ApplicationContext db = new())
                {
                    Person? person = db.Persons.Include(p => p.skills).FirstOrDefault(x => x.id == id);
                    return person == null ? NotFound() : Ok(person);
                }
            }
            catch (Exception)
            {
                Log.Logger.Error(e.Message);
                return StatusCode(500);
            }
        }
        
        [HttpPost]
        public IActionResult CreatePerson(Person person)
        {
            try
            {
                using (ApplicationContext db = new())
                {
                    db.Persons.Add(person);
                    db.SaveChanges();
                    return StatusCode(200);
                }
            }
            catch (Exception)
            {
                Log.Logger.Error(e.Message);
                return StatusCode(500);
            }
        }
        
        [HttpPut("{id}")]
        public IActionResult UpdatePerson(long id, Person person)
        {
            try
            {
                Log.Logger.Debug($"Update person by id: {id}");
                using (ApplicationContext db = new ApplicationContext())
                {
                    Person findPerson = db.Persons.Include(p => p.skills).FirstOrDefault(x => x.id == id);
                    if (findPerson == null)
                    {
                        return StatusCode(404, "Person not found!");
                    }

                    foreach (var skill in person.skills)
                    {
                        var findSkill = findPerson.skills.FirstOrDefault(s => s.name == skill.name);
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

                    return StatusCode(200);
                }
            }
            catch (Exception)
            {
                Log.Logger.Error(e.Message);
                return StatusCode(500);
            }
        }
        
        [HttpDelete("{id}")]
        public IActionResult DeletePerson(long id)
        {
            try
            {
                using (ApplicationContext db = new())
                {
                    Person? findPerson = db.Persons.FirstOrDefault(x => x.id == id);
                    if (findPerson == null)
                    {
                        return StatusCode(404, "Person not found!");
                    }

                    db.Persons.Remove(findPerson);
                    db.SaveChanges();
                    
                    return StatusCode(200);
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