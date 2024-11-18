using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var lineItem = await _context.LineItems.Include(li => li.Product)
                .Include(li => li.Request)
                .Include(li => li.Product.Vendor)
                .FirstOrDefaultAsync(li => li.Id == id);
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
        [HttpGet("request/{requestid}")]
        public async Task<ActionResult<IEnumerable<LineItem>>> GetLineItemsByRequestId(int requestid)
        {
            //we are trying to get all the lineitems for a particular request here
            var lineItems = await _context.LineItems //I hope this works
                .Where(li => li.RequestId == requestid)
                .Include(li => li.Product)
                .Include(li => li.Request)
                .Include(li => li.Product.Vendor)
                .ToListAsync();
           /* if (lineItems == null || !lineItems.Any())
            {
                return NotFound();
            }*/
            return Ok(lineItems);
        }
        private void nullifyAndSetId(LineItem lineItem) 
        { 
            Console.WriteLine("LI Nullify: LI: " + lineItem.ToString());

            if (lineItem.Request != null) { if (lineItem.RequestId == 0) { lineItem.RequestId = lineItem.Request.Id; } 
                lineItem.Request = null; } if (lineItem.Product != null)

            { if (lineItem.ProductId == 0)

                { lineItem.ProductId = lineItem.Product.Id; } 

                lineItem.Product = null; }

        }


        // POST: api/LineItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LineItem>> PostLineItem(LineItem lineItem)
        {
            nullifyAndSetId (lineItem);
            if(lineItem == null)
            {
                return BadRequest();
            }
            _context.LineItems.Add(lineItem);
            await _context.SaveChangesAsync();
            
            //find the request(s)
            var request = await _context.Requests
                  //.Include(li => li.Product)
                //.Include(li => li.Request)
                //.Include(li => li.Product.Vendor)
                //.Include(li => li.Product)
                //.Include(li => li.Request)//this and the line above are new from testing
                .Include(li => li.LineItems)
                //.Include(li => li.Id == lineItem.RequestId)
                //.Include(li => li.Id == lineItem.ProductId)
                .ThenInclude(li => li.Product) 
                .FirstOrDefaultAsync(li => li.Id == lineItem.RequestId);
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
            //var lineItem = await _context.LineItems.FindAsync(id);
            var lineItem = await _context.LineItems //I am not sure this makes sense now that I "think" about it
                .Include(li => li.Request) //but my intent is to find the lineitem and associated request
                .FirstOrDefaultAsync(li => li.Id == id);
            if (lineItem == null)
            {
                return NotFound();
            }
            //ok so we have to find the request the lineitem is part of. Not sure how I forgot to do that

            //var request = await _context.Requests all this commented out code is going to look unprofessional, but I need the reference
            //    .Include(r => r.LineItems)
            //    .FirstOrDefaultAsync(r => r.Id == lineItem.RequestId);
            //_context.LineItems.Remove(lineItem);

            //reference request
            var request = lineItem.Request;

            //maybe I could subtract lineitem total from the actual total?
            var product = await _context.Products.FindAsync(lineItem.ProductId);
            if (product == null) 
            {
                return NotFound();
            }

            //lets try and calculate here
            var totalSubtract = product.Price * lineItem.Quantity;

            //update total, I'm seeing if doing it BEFORE removing works 
            request.Total -= totalSubtract; //should I use update total or the literal variable total here? I'll try both

            //delete lineitem
            _context.LineItems.Remove(lineItem);

            //update total
            await _context.SaveChangesAsync(); // we are getting closer
            //request.UpdateTotal();//however, it now updates to zero, but thats still closer than not updating

            //save
             await _context.SaveChangesAsync();          
            return NoContent();

        }

        private bool LineItemExists(int id)
        {
            return _context.LineItems.Any(e => e.Id == id);
        }
    }
}
