using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Patient.Function.Helpers
{
    public static class ApiResponseHelper
    {
        /// <summary>
        /// Processes API response and returns appropriate IActionResult
        /// </summary>
        /// <param name="response">The HTTP response message from the API</param>
        /// <param name="responseBody">The response body as string</param>
        /// <returns>IActionResult with appropriate status and content</returns>
        public static async Task<Microsoft.AspNetCore.Mvc.IActionResult> ProcessApiResponse(HttpResponseMessage response)
        {
            try
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseBody);

                if (response.IsSuccessStatusCode)
                {
                    if (jsonResponse["Code"] != null && jsonResponse["Data"] != null)
                    {
                        return new OkObjectResult(jsonResponse);
                    }
                    else
                    {
                        return new OkObjectResult(new
                        {
                            Code = (int)response.StatusCode,
                            Data = jsonResponse["data"]
                        });
                    }
                }
                else
                {
                    if (jsonResponse["msg"]?.ToString() == "Validation failed" &&
                        jsonResponse["data"] != null &&
                        jsonResponse["data"].Type == JTokenType.Array)
                    {
                        return new BadRequestObjectResult(jsonResponse);
                    }

                    string errorMessage = jsonResponse["error"]?.ToString() ??
                      jsonResponse["Error"]?.ToString() ??
                      jsonResponse["msg"]?.ToString() ??
                      jsonResponse["message"]?.ToString() ??
                      "Unknown error occurred";

                    return new ObjectResult(new
                    {
                        Code = (int)response.StatusCode,
                        Message = errorMessage, 
                    })
                    {
                        StatusCode = (int)response.StatusCode
                    };
                }
            }
            catch (JsonReaderException)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return new ObjectResult(new
                {
                    Code = (int)response.StatusCode,
                    Message = responseBody
                })
                {
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    Code = 500,
                    Message = $"Error processing API response: {ex.Message}"
                })
                {
                    StatusCode = 500
                };
            }
        }

    }
}
