using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using Diary.WebUI.Extensions;
using Diary.WebUI.Helpers;
using Diary.WebUI.Models;
using Diary.WebUI.Services;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;


namespace Diary.WebUI.Controllers
{

    [Authorize(Roles = "user")]
    public class RecordsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IDataProtectionProvider _protectionProvider;
        private readonly IEmailService _mail;
        public RecordsController(ApplicationContext context, IWebHostEnvironment hostEnvironment, IDataProtectionProvider protectionProvider, IEmailService mail)
        {
            
            _context = context;
            _hostEnvironment = hostEnvironment;
            _protectionProvider = protectionProvider;
            _mail = mail;
        }
        [HttpGet]
        public IActionResult Index()
        { 
            return View();
        }

        public async Task<IActionResult> AddOrEdit(int id = 0)
        {

            if (id != 0)
            {
                var recordModel = await _context.Records.FindAsync(id);

                if (recordModel == null)
                {
                    return NotFound();
                }
                return PartialView("AddOrEdit", recordModel);
            }

            var model = new Record()
            {
                CreateDate = DateTime.Now.ToUniversalTime().Ticks.ToString()
            };
            return PartialView("AddOrEdit", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id, Title, Text, CreateDate, UserId, ImageFile")] Record recordModel)
        {
            if (ModelState.IsValid)
            {

                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(recordModel.ImageFile.FileName);
                string extension = Path.GetExtension(recordModel.ImageFile.FileName);
                recordModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);

                using (MagickImage image = Compressor.CompressImage(recordModel))
                {
                    var fileStream = new FileStream(path, FileMode.Create);
                    await image.WriteAsync(fileStream);
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                recordModel.UserId = userId;
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                recordModel.Text = recordModel.Text.Encrypter(currentUserId, true, _protectionProvider);

                if (id == 0)
                {
                    recordModel.CreateDate = DateTime.Now.ToUniversalTime().Ticks.ToString();
                    _context.Add(recordModel);
                }
                else
                {
                    _context.Update(recordModel);
                }

                await _context.SaveChangesAsync();
                return View("Index");
            }
            return Redirect("Index");
        }

        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var record = await _context.Records.FindAsync(id);
            var timeNow = DateTime.Now.ToUniversalTime();
            DateTime recordTime = new DateTime(long.Parse(record.CreateDate));
            TimeSpan span = timeNow.Subtract(recordTime);

            if (record != null && span.Days < 2)
            {
                return PartialView("~/Views/Records/_DeleteConfirmation.cshtml", record);
            }
            return PartialView("~/Views/Records/_DeleteMessage.cshtml", record);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Record _record)
        {

            _context.Records.Remove(_record);
            await _context.SaveChangesAsync();
            return View("Index");
        }

        private void PrerareDateTimeModel(Record record)
        {
            if (record != null && record.CreateDate != null)
            {
                record.CreateDate = new DateTime(long.Parse(record.CreateDate)).ToString();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Diary Application.";
            return View();
        }

        [HttpPost]
        public IActionResult GetFilteredData(FilterQuery query)
        {
            var listData = (IEnumerable<Record>)_context.Records;
            foreach (var record in listData)
                PrerareDateTimeModel(record);

            if (!string.IsNullOrEmpty(query.SearchedText))
            {
                listData = listData.Where(i => i.Text.Contains(query.SearchedText));
            }
            if (query.SelectedStartDate != DateTime.MinValue && query.SelectedEndDate != DateTime.MinValue)
            {
                listData = listData.Where(i => DateTime.Compare(DateTime.Parse(i.CreateDate), query.SelectedStartDate) >= 0 && DateTime.Compare(DateTime.Parse(i.CreateDate), query.SelectedEndDate) <= 0);
            }
 
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            listData = listData.Where(x => x.UserId == currentUserId).Select(r => { r.Text = r.Text.Encrypter(currentUserId,false, _protectionProvider); return r; }).ToList();
            var pagedData = Pagination.PagedResult(listData, query.Page, query.PageSize);

            return PartialView("~/Views/Records/_FilteredData.cshtml", pagedData);
        }
    }
}