using AutoMapper;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.PayVolatility.Core.Services;
using Lykke.Service.PayVolatility.Models;
using LykkePay.Common.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.Controllers
{
    [Route("api/[controller]/[action]")]
    public class VolatilityController : Controller
    {
        private readonly IVolatilityService _volatilityService;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public VolatilityController(IVolatilityService volatilityService,
            IMapper mapper, ILogFactory logFactory)
        {
            _volatilityService = volatilityService;
            _mapper = mapper;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Returns volatilities of the specified date.
        /// </summary>
        /// <param name="date">Date of volatilities.</param>
        /// <returns code="200">Volatilities.</returns>
        /// <returns code="404">Volatilities for specified date is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        [HttpGet]
        [SwaggerOperation("GetDailyVolatilities")]
        [ProducesResponseType(typeof(IEnumerable<VolatilityModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> GetDailyVolatilities(DateTime date)
        {
            var volatilities = await _volatilityService.GetAsync(date);

            if (!volatilities.Any())
            {
                return NotFound();
            }

            var models = _mapper.Map<IEnumerable<VolatilityModel>>(volatilities);
            return Ok(models);
        }

        /// <summary>
        /// Returns volatility.
        /// </summary>
        /// <param name="date">Date of volatility.</param>
        /// <param name="assetPairId">Identifier of the asset pair.</param>
        /// <returns code="200">Volatility.</returns>
        /// <returns code="404">Volatility is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        [HttpGet]
        [SwaggerOperation("GetDailyVolatility")]
        [ProducesResponseType(typeof(VolatilityModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> GetDailyVolatility(DateTime date, 
            [Required, RowKey]string assetPairId)
        {
            var volatility = await _volatilityService.GetAsync(date, assetPairId);

            if (volatility == null)
            {
                return NotFound();
            }

            var models = _mapper.Map<VolatilityModel>(volatility);
            return Ok(models);
        }
    }
}
