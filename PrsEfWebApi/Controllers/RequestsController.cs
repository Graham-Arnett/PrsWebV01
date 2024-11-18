using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using PrsEfWebApi.Models;

namespace PrsEfWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly PrsdbContext _context;

        public RequestsController(PrsdbContext context)
        {
            _context = context;
        }

        //I had a bunch of commented out code for list review here, but it made it too messy so I just removed it

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            return await _context.Requests.ToListAsync();
        }

        [HttpGet("list-review/{userid}")]
        public async Task<IActionResult> ListReview(int userid)
        {
            //because this is a get command, we don't really edit the data here, we just search for it
            //this is for reviewers to find requests ready to review
            var reviewer = await _context.Users.FindAsync(userid);
            if (reviewer == null || !reviewer.Reviewer) // this is meant to determind if the user even is a reviewer
            {
                return Forbid("You are not authorized."); //forbid as in forbidden, I assume because a normal user should not be allowed to see it
            }
            var requests = await _context.Requests
                .Where(r => r.Status == "REVIEW" && r.UserId != userid) //this is meant to say the person logged in can't review their requests even if they're a reviewer
                .ToListAsync(); //why is it spelled without an h?
            if (requests == null || !requests.Any())
            {
                return NotFound("No requests found");
            }

            return Ok(requests);
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int userid, int id)
        {
            //need a variable for a user that is a reviewer
            //originally it was only int id in here until I realized I needed userid, keeping both for the moment until I fix this

            //var request = await _context.Requests.FindAsync(id);
            var request = await _context.Requests.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return request;
        }


        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
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

        [HttpPut("submit-review/{id}")]
        public async Task<ActionResult<IEnumerable<Request>>> SubmitReview(int id)
        {
            //this will be the put action for submitting a request for review
            //ok, so this method just tells the program that a request is ready for review
            //because of that, how can I define a default status value of review
            //the input listed is id:int not user id like the get
            //same input is needed for the approve and reject functions
            //var request = await _context.Requests.FindAsync(id);
            var checkrequest = await _context.Requests
                .Include(r => r.LineItems)
                .ThenInclude(li => li.Product)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (checkrequest == null)
            {
                return NotFound();
            }
            //update 
           // checkrequest.Status = request.Status;

            //recalc
            checkrequest.UpdateTotal();

            //auto approve if lower than 50
            if (checkrequest.Total < 50.00m)
            {
                checkrequest.Status = "APPROVED"; //it works!
            }
            else
            {
                //I don't think this is right, this might allow it to change an accepted or rejected one to review

                checkrequest.Status = "REVIEW";
            }
            _context.Entry(checkrequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("approve/{id}")]//approve
        public async Task<ActionResult<IEnumerable<Request>>> ApproveRequest(int id)
        {
            //this will be the put action for submitting a request for review
            //ok, so this method just tells the program that a request is ready for review
            //because of that, how can I define a default status value of review
            var requests = await _context.Requests.FindAsync(id);
             if (requests == null)
            {
                return NotFound("No request found.");
            }
            //var userid = GetUserId(); this didn't work, I'm keeping it because I'm still learning and think it might help to have reference
            //var reviewer = await _context.Users
            //    .Where(r => r.userId == userId && r.Reviewer)
            //    .FirstOrDefaultAsync();
            requests.Status = "APPROVED";
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("reject/{id}")]//reject
        public async Task<IActionResult> RejectRequest(int id, RejectionReason rejectionReason)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            request.Status = "REJECTED";
            request.ReasonForRejection = rejectionReason.ReasonForRejection; //the reason you reject, duh
            _context.Requests.Update(request);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(RequestForm requestForm)
        {
            Request request = new Request
            {
                UserId = requestForm.UserId,
                Description = requestForm.Description,
                Justification = requestForm.Justification,
                DateNeeded = requestForm.DateNeeded,
                DeliveryMode = requestForm.DeliveryMode,
                SubmittedDate = DateTime.Now,
                Status="NEW",
                Total = 0.0m
            };
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            
            if (request == null)
            {
                return NotFound();
            }
            request.UpdateTotal();
            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
