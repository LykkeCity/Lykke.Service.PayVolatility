using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lykke.Service.PayVolatility.Filters
{
    public class ValidateActionParametersFilterAttribute : ActionFilterAttribute
    {
        private readonly ILog _log;

        public ValidateActionParametersFilterAttribute(ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;

            var arguments = new Dictionary<string, object>();
            if (descriptor != null)
            {
                var parameters = descriptor.MethodInfo.GetParameters();

                foreach (var parameter in parameters)
                {
                    context.ActionArguments.TryGetValue(parameter.Name, out var argument);
                    arguments.Add(parameter.Name, argument);
                    EvaluateValidationAttributes(parameter, argument, context.ModelState);
                }
            }

            if (!context.ModelState.IsValid)
            {
                var errorResponse = GetErrorResponse(context.ModelState);
                _log.Warning(descriptor.ActionName, "Model is invalid.", null, new
                {
                    errorResponse,
                    arguments
                });
                context.Result = new BadRequestObjectResult(errorResponse);
            }

            base.OnActionExecuting(context);
        }

        private void EvaluateValidationAttributes(ParameterInfo parameter, object argument, ModelStateDictionary modelState)
        {
            var validationAttributes = parameter.CustomAttributes;

            foreach (var attributeData in validationAttributes)
            {
                var attributeInstance = CustomAttributeExtensions.GetCustomAttribute(parameter, attributeData.AttributeType);

                var validationAttribute = attributeInstance as ValidationAttribute;

                if (validationAttribute != null)
                {
                    var isValid = validationAttribute.IsValid(argument);
                    if (!isValid)
                    {
                        modelState.AddModelError(parameter.Name, validationAttribute.FormatErrorMessage(parameter.Name));
                    }
                }
            }
        }

        private ErrorResponse GetErrorResponse(ModelStateDictionary modelState)
        {
            var errorResponse = ErrorResponse.Create("Model is invalid.");
            foreach (var entity in modelState)
            {
                foreach (var error in entity.Value.Errors)
                {
                    if (error.Exception != null)
                    {
                        errorResponse.AddModelError(entity.Key, error.Exception);
                    }
                    else
                    {
                        errorResponse.AddModelError(entity.Key, error.ErrorMessage);
                    }
                }
            }

            return errorResponse;
        }
    }
}
