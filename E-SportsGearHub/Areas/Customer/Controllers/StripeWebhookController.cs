using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using ESports_DataAccess.Repository.IRepository;
using ESports_Utility;

namespace E_SportsGearHub.Areas.Customer.Controllers
{
    [Route("stripe-webhook")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public StripeWebhookController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _configuration["Stripe:WebhookSecret"]
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    var orderHeader = await _unitOfWork.OrderHeader.GetAsync(x => x.SessionId == session.Id);

                    if (orderHeader != null)
                    {
                        orderHeader.PaymentStatus = Sd.PaymentStatusApproved;
                        orderHeader.OrderStatus = Sd.StatusApproved;
                        orderHeader.PaymentIntentId = session.PaymentIntentId;

                        _unitOfWork.OrderHeader.Update(orderHeader);
                        await _unitOfWork.SaveAsync();
                    }
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
