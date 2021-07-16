using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System.Security.Claims;
using Diary.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Diary.WebUI.Extensions;

namespace Diary.WebUI.Controllers.SupplierAPI
{
    [Authorize(Roles = "user")]
    [Route("api/[controller]")]
    [ApiController]
    public class RecordsApiController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public RecordsApiController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/RecordsApi
        [HttpGet]
        public IEnumerable<RecordDto> GetRecords()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var records = _context.Records.Where(x => x.UserId == userId).ToList();
            var recordDtos = records.Select(s => PrepareRecordDto(s)).ToList();

            return recordDtos;
        }

        // GET: api/RecordsApi/5
        [HttpGet("{id}")]
        public ActionResult<RecordDto> GetRecord(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var record = _context.Records
                .Where(x => x.UserId == userId && x.Id == Convert.ToInt32(id.Base64crypter(false)))
                .FirstOrDefault();

            if (record == null)
            {
                return NotFound();
            }

            var recordDto = PrepareRecordDto(record);

            return recordDto;
        }

        // PUT: api/RecordsApi/5
        [HttpPut]
        public async Task<IActionResult> PutRecord(Record record)
        {
            if (record?.Id == null)
            {
                return BadRequest();
            }

            _context.Entry(record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordExists(record.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RecordsApi
        [HttpPost]
        public async Task<ActionResult<Record>> PostRecord(Record record)
        {
            _context.Records.Add(record);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecord", new { id = record.Id }, record);
        }

        // DELETE: api/RecordsApi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Record>> DeleteRecord(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var record = _context.Records
                .Where(x => x.UserId == userId && x.Id == Convert.ToInt32(id.Base64crypter(false)))
                .FirstOrDefault();
            if (record == null)
            {
                return NotFound();
            }

            _context.Records.Remove(record);
            await _context.SaveChangesAsync();
            return record;
        }

        private bool RecordExists(int id)
        {
            return _context.Records.Any(e => e.Id == id);
        }

        private RecordDto PrepareRecordDto(Record model) 
        {
            return new RecordDto
            {
                Id = Convert.ToString(model.Id).Base64crypter(true),
                Title = model.Title,
                Text = model.Text,
                CreateDate = model.CreateDate,
                ImageName = model.ImageName,
                ImageFile = model.ImageFile,
                UserId = model.UserId
            };
        }

    }
}
