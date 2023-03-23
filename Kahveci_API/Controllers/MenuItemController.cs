using Kahveci_API.Data;
using Kahveci_API.Dto;
using Kahveci_API.Entities;
using Kahveci_API.Services;
using Kahveci_API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace Kahveci_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IBlobService _blobService;
        private readonly ApiResponse _apiResponse;

        public MenuItemController(AppDbContext db, IBlobService blobService)
        {
            _db = db;
            _apiResponse = new ApiResponse();
            _blobService = blobService;
        }
        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            _apiResponse.Result = _db.MenuItems;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }
        [HttpGet("{id:int}", Name = "GetMenuItem")]
        public async Task<IActionResult> GetItemById(int id)
        {
            if (id == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                return BadRequest(_apiResponse);
            }
            MenuItem menuItem = _db.MenuItems.FirstOrDefault(x => x.Id == id);
            if (menuItem == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                return NotFound(_apiResponse);
            }
            _apiResponse.Result = menuItem;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateItem([FromForm] MenuItemCreateDto createDto) //Fotoğraf da ekleyeceğin için FromBody yerine FromForm
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (createDto.File == null || createDto.File.Length == 0)
                    {
                        _apiResponse.StatusCode = HttpStatusCode.NotFound;
                        _apiResponse.IsSuccess = false;
                        return BadRequest();
                    }

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(createDto.File.FileName)}";

                    MenuItem menuItemToCreate = new()  //Automapper ekle
                    {
                        Name = createDto.Name,
                        Price = createDto.Price,
                        Category = createDto.Category,
                        SpecialTag = createDto.SpecialTag,
                        Description = createDto.Description,
                        Image = await _blobService.UploadBlob(fileName, SD.SD_STORAGE_CONTAINER, createDto.File),
                    };
                    _db.MenuItems.Add(menuItemToCreate);
                    _db.SaveChanges();
                    _apiResponse.Result = menuItemToCreate;
                    _apiResponse.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetMenuItem", new { id = menuItemToCreate.Id }, _apiResponse);

                }
                else
                {
                    _apiResponse.IsSuccess = false;
                }
            }
            catch (Exception e)
            {

                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { e.ToString() };
            }
            return _apiResponse;
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateItem(int id, [FromForm] MenuItemUpdateDto updateDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (updateDto == null || id != updateDto.Id)
                    {
                        _apiResponse.StatusCode = HttpStatusCode.NotFound;
                        _apiResponse.IsSuccess = false;
                        return BadRequest();
                    }

                    MenuItem menuItemFromDb = await _db.MenuItems.FirstOrDefaultAsync(x => x.Id == id);
                    if (menuItemFromDb == null)
                    {
                        _apiResponse.StatusCode = HttpStatusCode.NotFound;
                        _apiResponse.IsSuccess = false;
                        return BadRequest();
                    }
                    menuItemFromDb.Name = updateDto.Name;
                    menuItemFromDb.Price = updateDto.Price; 
                    menuItemFromDb.Description = updateDto.Description;
                    menuItemFromDb.Category = updateDto.Category;
                    menuItemFromDb.SpecialTag = updateDto.SpecialTag;

                    if (updateDto.File!= null && updateDto.File.Length > 0)
                    {
                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(updateDto.File.FileName)}";
                        await _blobService.DeleteBlob(menuItemFromDb.Image.Split('/').Last(), SD.SD_STORAGE_CONTAINER);
                        menuItemFromDb.Image = await _blobService.UploadBlob(fileName, SD.SD_STORAGE_CONTAINER, updateDto.File);
                    }
                    
                    _db.MenuItems.Update(menuItemFromDb);
                    _db.SaveChanges();
                    _apiResponse.StatusCode = HttpStatusCode.NoContent;
                    return Ok(_apiResponse);

                }
                else
                {
                    _apiResponse.IsSuccess = false;
                }
            }
            catch (Exception e)
            {

                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { e.ToString() };
            }
            return _apiResponse;
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse>> DeleteItem(int id)
        {
            try
            {
                if (id == 0)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsSuccess = false;
                    return BadRequest();
                }

                MenuItem menuItemFromDb = await _db.MenuItems.FirstOrDefaultAsync(x => x.Id == id);
                if (menuItemFromDb == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsSuccess = false;
                    return BadRequest();
                }

                await _blobService.DeleteBlob(menuItemFromDb.Image.Split('/').Last(), SD.SD_STORAGE_CONTAINER);
                int milseconds = 1000;
                Thread.Sleep(milseconds);

                _db.MenuItems.Remove(menuItemFromDb);
                _db.SaveChanges();
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                return Ok(_apiResponse);
 
            }
            catch (Exception e)
            {

                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { e.ToString() };
            }
            return _apiResponse;
        }
    }
}

