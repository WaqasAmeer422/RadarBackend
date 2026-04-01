using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using VisualSoft.Surveillance.Radar.Api.Infrastructure;
using VisualSoft.Surveillance.Radar.Api.Models;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Services;

namespace VisualSoft.Surveillance.Radar.Api.Controllers
{
    [Route("api/radar")]
    [ApiController]
    public class TransactionController : BaseController
    {
        private readonly IRadarTransactionService _radarTransactionLogService;
        private readonly IMapper _mapper;

        public TransactionController(IRadarTransactionService radarTransactionLogService, IMapper mapper)
        {
            _radarTransactionLogService = radarTransactionLogService;
            _mapper = mapper;
        }
 
        //--------------
        [HttpGet("all")]
        [Authorize(Policy = Constants.Permissions.VIEW_TRANSACTION)]
        [ProducesResponseType(typeof(IEnumerable<TransactionRadarResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var result = await _radarTransactionLogService.GetAllTransactions();
            return Ok(_mapper.Map<List<TransactionRadarResponse>>(result));
        }

        [HttpGet("all/paginations")]
        [Authorize(Policy = Constants.Permissions.VIEW_TRANSACTION)]
        [ProducesResponseType(typeof(IEnumerable<TransactionRadarResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] PaginationParametersRequest request)
        {
            var (result, totalCount) = await _radarTransactionLogService.GetAllTransactionsPagedItems(_mapper.Map<PaginationParameters>(request));

            // Include pagination metadata in the response headers (recommended for RESTful APIs)
            var paginationMetadata = new PaginationMetaDataResponse
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };

            Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<List<TransactionRadarResponse>>(result));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Constants.Permissions.VIEW_TRANSACTION)]
        [ProducesResponseType(typeof(TransactionRadarResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _radarTransactionLogService.GetTransactionById(id);

            return Ok(_mapper.Map<TransactionRadarResponse>(result));
        }

        [HttpGet]
        [Authorize(Policy = Constants.Permissions.VIEW_TRANSACTION)]
        [ProducesResponseType(typeof(IEnumerable<TransactionRadarResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] TransactionFilterRequest? criteria)
        {
            var result = await _radarTransactionLogService.GetTransactionByCriteria(_mapper.Map<TransactionFilterRequestDto>(criteria));

            return Ok(_mapper.Map<List<TransactionRadarResponse>>(result));
        }

        //[HttpGet("navigate")]
        //[Authorize(Policy = Constants.Permissions.VIEW_TRANSACTION)]
        //[ProducesResponseType(typeof(TransactionRadarResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Get([FromQuery] NavigateTransactionFilterRequest? criteria)
        //{
        //    var result = await _radarTransactionLogService.NavigateTransactionByCriteria(_mapper.Map<NavigateTransactionFilterDto>(criteria));

        //    return Ok(_mapper.Map<TransactionRadarResponse>(result));
        //}

        //[HttpGet("paginations")]
        //[Authorize(Policy = Constants.Permissions.VIEW_TRANSACTION)]
        //[ProducesResponseType(typeof(IEnumerable<TransactionRadarResponse>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Get([FromQuery] TransactionFilterRequest? criteria, [FromQuery] PaginationParametersRequest paginations)
        //{
        //    var (result, totalCount) = await _radarTransactionLogService.GetTransactionByCriteriaPagedItems(_mapper.Map<TransactionFilterRequestDto>(criteria),
        //       _mapper.Map<PaginationParameters>(paginations));

        //    // Include pagination metadata in the response headers (recommended for RESTful APIs)
        //    var paginationMetadata = new PaginationMetaDataResponse
        //    {
        //        PageNumber = paginations.PageNumber,
        //        PageSize = paginations.PageSize,
        //        TotalCount = totalCount,
        //        TotalPages = (int)Math.Ceiling((double)totalCount / paginations.PageSize)
        //    };

        //    Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));

        //    return Ok(_mapper.Map<List<TransactionRadarResponse>>(result));
        //}

    } 
}
