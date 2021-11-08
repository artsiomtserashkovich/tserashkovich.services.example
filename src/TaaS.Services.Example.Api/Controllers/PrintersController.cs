using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaaS.Services.Example.Api.Storage.Persistence.Dao;
using TaaS.Services.Example.Api.Storage.Persistence.Model;
using TaaS.Services.Example.Api.Storage.ViewModels.Read;
using TaaS.Services.Example.Api.Storage.ViewModels.Write;

namespace TaaS.Services.Example.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrintersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPrintersDao _dao;

        public PrintersController(IPrintersDao dao, IMapper mapper)
        {
            _dao = dao;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPrinters()
        {
            var printers = new List<ViewPrinter>();
            await foreach (var printer in _dao.ReadAll())
            {
                printers.Add(_mapper.Map<ViewPrinter>(printer));
            }

            return Ok(printers);
        }

        //api/printers/213123
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrinter([Required] Guid id)
        {
            var printer = await _dao.Read(id);

            if (printer == null)
            {
                return NotFound();
            }
            
            var viewPrinter = _mapper.Map<ViewPrinter>(printer);

            return Ok(viewPrinter);
        }
        
        //api/printers
        [HttpPost]
        public async Task<IActionResult> CreatePrinter(WritePrinter writePrinter)
        {
            var printer = _mapper.Map<Printer>(writePrinter);
            var id = await _dao.Create(printer);
            
            return Ok(new { id });
        }
        
        //api/printers/1231
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPrinter([Required] Guid id, WritePrinter writePrinter)
        {
            if (!await _dao.IsExist(id))
            {
                return NotFound();
            }
            
            var printer = _mapper.Map(writePrinter, new Printer{ Id = id});
            await _dao.Edit(printer);
            
            return Ok();
        }
        
        //api/printers/123
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrinter([Required]Guid id)
        {
            if (!await _dao.IsExist(id))
            {
                return NotFound();
            }
            
            await _dao.Delete(id);
            
            return Ok();
        }
    }
}