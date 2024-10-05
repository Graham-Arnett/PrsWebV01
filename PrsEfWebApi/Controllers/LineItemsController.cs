using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrsEfWebApi.Models;

namespace PrsEfWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineItemsController : ControllerBase
    {
        private readonly PrsdbContext _context;

        public LineItemsController(PrsdbContext context)
        {
            _context = context;
        }

        // GET: api/LineItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LineItem>>> GetLineItems()
        {
            return await _context.LineItems.ToListAsync();
        }

        // GET: api/LineItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LineItem>> GetLineItem(int id) //I had thought adding lineitem here would help, it broke it
        {
            //var lineItem = await _context.LineItems.FindAsync(id);
            var lineItem = await _context.LineItems.Include(r => r.Product)
                .Include(r => r.Request)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (lineItem == null)//how to fix the problem with product being null?
            {
                return NotFound();
            }

            return lineItem;
        }

        // PUT: api/LineItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLineItem(int id, LineItem lineItem)
        {
            if (id != lineItem.Id || lineItem == null)
            {
                return BadRequest("Invalid line item data.");
            }

            _context.Entry(lineItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineItemExists(id))
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

        // POST: api/LineItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LineItem>> PostLineItem(LineItem lineItem)
        {
         
            if(lineItem == null)
            {
                return BadRequest();
            }
            _context.LineItems.Add(lineItem);
            await _context.SaveChangesAsync();
         

            return CreatedAtAction("GetLineItem", new { id = lineItem.Id }, lineItem);
            //if (lineItem == null || lineItem.ProductId <= 0 || lineItem.RequestId <= 0)
            //{
            //    return BadRequest("Invalid line item data.");
            //}
            //_context.LineItems.Add(lineItem);
            //await _context.SaveChangesAsync();

            ////return CreatedAtAction("GetLineItem", new { id = lineItem.Id }, lineItem);
            //return CreatedAtAction(nameof(GetLineItem), new {id = lineItem.Id}, lineItem);
        }

        // DELETE: api/LineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLineItem(int id)
        {
            var lineItem = await _context.LineItems.FindAsync(id);
            if (lineItem == null)
            {
                return NotFound();
            }

            _context.LineItems.Remove(lineItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LineItemExists(int id)
        {
            return _context.LineItems.Any(e => e.Id == id);
        }
    }
}
