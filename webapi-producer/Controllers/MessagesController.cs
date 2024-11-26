using Confluent.Kafka;
using message_contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi_producer.Services;

namespace webapi_producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IProducerService _producer;

        public MessagesController(IProducerService producer)
        {
            _producer = producer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">Request Body sample: {"key": "314", "value": { "id": "314", "description": "The PI number", "price": 314.159} }</param>
        /// <returns>value from message: type of MessageModel</returns>
        [HttpPost("send", Name = "send")] // request
        public async Task<IActionResult> SendMessageAsync([FromBody] Message<string, MessageContent> message)
        {
            try
            {
                var result = await _producer.SendMessageAsync(message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
