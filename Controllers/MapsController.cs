using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        private readonly IMapDataAccess _mapRepo; 

        public MapsController(IMapDataAccess mapRepo)
        {
            _mapRepo = mapRepo;
        }

        /// <summary>
        /// Retrieves all maps.
        /// </summary>
        /// <returns>A list of all maps</returns>
        [HttpGet(), Authorize(Policy = "UserOnly")]
        public IEnumerable<Models.Map> GetAllMaps() { return _mapRepo.GetMaps(); }

        /// <summary>
        /// Retrieves all square maps.
        /// </summary>
        /// <returns>A list of all square maps</returns>
        [HttpGet("square"), Authorize(Policy = "UserOnly")]
        public IEnumerable<Models.Map> GetAllSquareMaps() { return _mapRepo.GetMaps().Where(map => map.Columns == map.Rows); }

        /// <summary>
        /// Retrieves a specific map by its ID.
        /// </summary>
        /// <param name="id">The ID of the map to retrieve</param>
        /// <returns>The map with the specified ID</returns>
        /// <response code="200">Returns the map with the specified ID</response>
        /// <response code="404">If no map with the specified ID is found</response>
        [HttpGet("{id}", Name = "GetMap"), Authorize(Policy = "UserOnly")]
        public IActionResult GetMapsById(int id)
        {
            var maps = _mapRepo.GetMaps();
            var map = maps.FirstOrDefault(m => m.Id == id);
            if (map == null)
            {
                return NotFound();
            }
            return Ok(_mapRepo.GetMaps().Where(map => map.Id == id));
        }

        /// <summary>
        /// Creates a new map.
        /// </summary>
        /// <param name="newMap">The new map to be created</param>
        /// <returns>The newly created map</returns>
        /// <response code="201">Returns the newly created map</response>
        /// <response code="400">If the map is null</response>
        /// <response code="409">If a map with the same ID already exists</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost(), Authorize(Policy = "AdminOnly")]
        public IActionResult AddMap(Models.Map newMap)
        {
            if (newMap == null)
            {
                return BadRequest();
            }

            if (_mapRepo.GetMaps().Contains(newMap))
            {
                return Conflict();
            }

            newMap.CreatedDate = DateTime.Now;
            newMap.ModifiedDate = DateTime.Now;
            _mapRepo.InsertMaps(newMap);
            newMap.Id = _mapRepo.GetMaps().Max(map => map.Id);

            return CreatedAtRoute("GetRobotCommand", new { id = newMap.Id }, newMap);
        }

        /// <summary>
        /// Updates an existing map.
        /// </summary>
        /// <param name="id">The ID of the map to update</param>
        /// <param name="updatedMap">The updated map</param>
        /// <returns>No content</returns>
        /// <response code="204">Indicates that the map was successfully updated</response>
        /// <response code="404">If no map with the specified ID is found</response>
        [HttpPut("{id}"), Authorize(Policy = "AdminOnly")]
        public IActionResult UpdateMap(int id, robot_controller_api.Models.Map updatedMap)
        {
            if (_mapRepo.GetMaps().FirstOrDefault(map => map.Id == id) == null)
            {
                return NotFound();
            }

            if (updatedMap == null)
            {
                return BadRequest();
            }

            if (updatedMap.Description == null)
            {
                updatedMap.Description = "N/A";            }

            updatedMap.ModifiedDate = DateTime.Now;
            updatedMap.Id = id;
            _mapRepo.UpdateMaps(updatedMap);
            return NoContent();
        }

        /// <summary>
        /// Deletes a map by its ID.
        /// </summary>
        /// <param name="id">The ID of the map to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">Indicates that the map was successfully deleted</response>
        /// <response code="404">If no map with the specified ID is found</response>
        [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteMap(int id)
        {
            var maps = _mapRepo.GetMaps();
            var map = maps.FirstOrDefault(m => m.Id == id);
            if (map == null)
            {
                return NotFound();
            }

            _mapRepo.DeleteMaps(id);
            return NoContent();
        }

        /// <summary>
        /// Checks if the given coordinates are valid for a specific map.
        /// </summary>
        /// <param name="id">The ID of the map</param>
        /// <param name="x">The x-coordinate</param>
        /// <param name="y">The y-coordinate</param>
        /// <returns>True if the coordinates are valid, otherwise false</returns>
        /// <response code="200">Returns true if the coordinates are valid</response>
        /// <response code="400">If the format of the coordinates is invalid</response>
        /// <response code="404">If no map with the specified ID is found</response>
        [HttpGet("{id}/{x}-{y}"), Authorize(Policy = "UserOnly")]
        public IActionResult CheckCoordinate(int id, string x, string y)
        {
            if (!int.TryParse(x, out int xValue) || !int.TryParse(y, out int yValue))
            {
                return BadRequest("Invalid format for x or y. Both x and y must be integers.");
            }

            var maps = _mapRepo.GetMaps();
            var map = maps.FirstOrDefault(m => m.Id == id);

            if (map == null)
            {
                return NotFound();
            }

            if (xValue < 0 || xValue > map.Columns || yValue < 0 || yValue > map.Rows)
            {
                return NotFound(false);
            }
            else
            {
                return Ok(true);
            }
        }
    }
}
