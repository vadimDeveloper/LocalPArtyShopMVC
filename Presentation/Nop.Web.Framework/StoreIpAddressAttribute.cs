using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;

namespace Nop.Web.Framework
{
    public class StoreIpAddressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            if (filterContext?.HttpContext?.Request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //only GET requests
            if (!string.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

            //update IP address
            var currentIpAddress = webHelper.GetCurrentIpAddress();
            if (string.IsNullOrEmpty(currentIpAddress)) return;
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var customer = workContext.CurrentCustomer;
            if (currentIpAddress.Equals(customer.LastIpAddress, StringComparison.InvariantCultureIgnoreCase)) return;
            var customerService = EngineContext.Current.Resolve<ICustomerService>();
            customer.LastIpAddress = currentIpAddress;
            customerService.UpdateCustomer(customer);
        }
    }
}
