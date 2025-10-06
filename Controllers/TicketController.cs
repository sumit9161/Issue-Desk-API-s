using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ticket_System.DTOs;
using Ticket_System.Models.Enums;
using Ticket_System.Services.Interfaces;

namespace Ticket_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;

        public TicketsController(ITicketService ticketService, IUserService userService)
        {
            _ticketService = ticketService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
        {
            try
            {
                string email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new Exception("Email not found in token");

                var result = await _ticketService.CreateTicketAsync(dto, email);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("User not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("admin/all")]
        [Authorize]
        public async Task<IActionResult> GetAllTicketsForAdmin()
        {
            try
            {
                string email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new Exception("Email not found in token");

                var tickets = await _ticketService.GetAllTicketsForAdminAsync(email);
                return Ok(tickets);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            try
            {
                string email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new Exception("Email not found in token");
                string userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

                var ticketDto = await _ticketService.GetTicketByIdAsync(id, email, userRole);
                return Ok(ticketDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                if (ex.Message == "User not found")
                    return Unauthorized(ex.Message);
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetTicketsByLoggedInUser()
        {
            try
            {
                string email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _ticketService.GetTicketsByUserAsync(email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("team")]
        [Authorize]
        public async Task<IActionResult> GetTicketsForTeam()
        {
            try
            {
                string email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _ticketService.GetTicketsForTeamAsync(email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("status")]
        [Authorize]
        public async Task<IActionResult> GetTicketStatuses()
        {
            try
            {
                string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var ticketStatuses = await _ticketService.GetTicketStatusesAsync(userEmail);
                return Ok(ticketStatuses);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("team/{team}/users")]
        [Authorize]
        public async Task<IActionResult> GetUsersByTeam(Team team)
        {
            var users = await _userService.GetUsersByTeamAsync(team);
            return Ok(users);
        }


       

        [HttpGet("user/assigned")]
        [Authorize]
        public async Task<IActionResult> GetAssignedTicketsByUser()
        {
            try
            {
                string email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _ticketService.GetAssignedTicketsByUserAsync(email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("user/requested")]
        [Authorize]
        public async Task<IActionResult> GetRequestedTicketsByUser()
        {
            try
            {
                string email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _ticketService.GetRequestedTicketsByUserAsync(email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }


        [HttpPut("self/update")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateOwnAssignedTicket([FromBody] UpdateOwnTicketDto dto)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _ticketService.UpdateOwnTicketAsync(dto, email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPut("admin/update")]
        [Authorize]
        public async Task<IActionResult> AdminUpdateTicketStatus([FromBody] UpdateOwnTicketDto dto)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _ticketService.AdminUpdateTicketAsync(dto, email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                if (ex.Message.Contains("not authorized"))
                    return Forbid(ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("tickets/{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateTicketStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var result = await _ticketService.UpdateTicketStatusAsync(id, dto, email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                if (ex.Message == "User not found")
                    return Unauthorized(ex.Message);
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}