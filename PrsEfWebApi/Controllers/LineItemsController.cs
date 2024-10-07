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

            //_context.Entry(lineItem).State = EntityState.Modified;

            var checkLineItem = await _context.LineItems
                .Include(li => li.Request)
                .ThenInclude(r => r.LineItems)
                .ThenInclude(li  => li.Product)
                .FirstOrDefaultAsync(li => li.Id == id);
            if (checkLineItem == null)
            {
                return NotFound();
            }
            
            //update properties
           checkLineItem.Quantity = lineItem.Quantity;
            checkLineItem.ProductId = lineItem.ProductId;

            //save lineitem
            _context.Entry(checkLineItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //recalc total
            var request = checkLineItem.Request;
            request.UpdateTotal(); 

            //save changes to request
            _context.Entry(request).State = EntityState.Modified;
            await _context.SaveChangesAsync();

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
            
            //find the request(s)
            var request = await _context.Requests
                .Include(r => r.LineItems)
                .ThenInclude(li => li.Product)
                .FirstOrDefaultAsync(r => r.Id == lineItem.RequestId);
            if (request == null)
            {
                return NotFound("The relevant request was not found");
            }

            //now we're trying to do the total
            request.UpdateTotal();

            //save request total changes
            _context.Entry(request).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return CreatedAtAction("GetLineItem", new { id = lineItem.Id }, lineItem);
            
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
