using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Streams.Experiment.Domain.Services;

namespace Streams.Experiment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssignUserClubsController : ControllerBase
    {
        private readonly ILogger<AssignUserClubsController> _logger;
        private readonly UserClubAssignmentService _userClubAssignmentService;

        public AssignUserClubsController(
            ILogger<AssignUserClubsController> logger,
            UserClubAssignmentService userClubAssignmentService)
        {
            _logger = logger;
            _userClubAssignmentService = userClubAssignmentService;
        }

        [HttpPost("{id:Guid}")]
        public async Task<AssignmentSummary> Post(Guid id)
        {
            return await _userClubAssignmentService.Assign(id);
        }
    }
}