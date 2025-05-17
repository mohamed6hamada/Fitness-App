using Fitness_App.DAL.Models;
using Stripe.Checkout;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness_App.BL.Interfaces;
using Fitness_App.BL.ViewModels;

namespace Fitness_App.BL.Servecies
{
    public class StripeService
    {
        public string CreateCheckoutSession(SubscriptionPlan plan, string successUrl, string cancelUrl)
        {
            if (plan == null)
                throw new ArgumentNullException(nameof(plan));

            if (plan.Price <= 0)
                throw new ArgumentException("Invalid plan price");

            if (string.IsNullOrEmpty(plan.Name))
                throw new ArgumentException("Plan name is required");


            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(plan.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = plan.Name,
                    },
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return session.Url;
        }


        
    }

}
