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
using Lykke.Service.PayVolatility.Core.Domain;
using Lykke.Service.PayVolatility.Core.Settings;

namespace Lykke.Service.PayVolatility.Controllers
{
    [Route("api/[controller]/[action]")]
    public class VolatilityController : Controller
    {
        private readonly IVolatilityService _volatilityService;
        private readonly VolatilityServiceSettings _volatilityServiceSettings;
        private readonly IMapper _mapper;
        private readonly ILog _log;
        private readonly int CalculationDurationInMinutes = 10; 

        public VolatilityController(IVolatilityService volatilityService,
            VolatilityServiceSettings volatilityServiceSettings,
            IMapper mapper, ILogFactory logFactory)
        {
            _volatilityService = volatilityService;
            _volatilityServiceSettings = volatilityServiceSettings;
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
        public async Task<IActionResult> GetDailyVolatilities(DateTime? date = null)
        {
            IEnumerable<IVolatility> volatilities;
            if (date.HasValue)
            {
                volatilities = await _volatilityService.GetAsync(date.Value);
            }
            else
            {
                DateTime lastDate = GetLastDate();
                volatilities = await _volatilityService.GetAsync(lastDate);
                if (!volatilities.Any())
                {
                    volatilities = await _volatilityService.GetAsync(lastDate.AddDays(-1));
                }
            }

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
        public async Task<IActionResult> GetDailyVolatility(DateTime? date, 
            [Required, RowKey]string assetPairId)
        {
            IVolatility volatility;
            if (date.HasValue)
            {
                volatility= await _volatilityService.GetAsync(date.Value, assetPairId);
            }
            else
            {
                DateTime lastDate = GetLastDate();
                volatility= await _volatilityService.GetAsync(lastDate, assetPairId);
                if (volatility == null)
                {
                    volatility= await _volatilityService.GetAsync(lastDate.AddDays(-1), assetPairId);
                }
            }

            if (volatility == null)
            {
                return NotFound();
            }

            var models = _mapper.Map<VolatilityModel>(volatility);
            return Ok(models);
        }

        private DateTime GetLastDate()
        {
            var calculationTime = DateTime.UtcNow.Date.Add(_volatilityServiceSettings.CalculateTime.TimeOfDay)
                .AddMinutes(CalculationDurationInMinutes);

            if (DateTime.UtcNow > calculationTime)
            {
                return DateTime.UtcNow.Date.AddDays(-1);
            }
            else
            {
                return DateTime.UtcNow.Date.AddDays(-2);
            }
        }
    }
}
