///////////////////////////////////////////////////////////////////
//
// Librogram: Reference application for social management of reads
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


using Microsoft.AspNetCore.Mvc.Filters;
using Youbiquitous.Librogram.Resources;
using Youbiquitous.Librogram.Shared.Exceptions;

namespace Youbiquitous.Librogram.App.Common.Security
{
    public class EnsureRoleAttribute : ActionFilterAttribute
    {
        public EnsureRoleAttribute(params string[] roles)
        {
            Roles = roles;
        }

        public string[] Roles { get; set; }

        /// <summary>
        /// Check performed before the controller method is invoked
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Locked out? Some further code here
            //var loggedUser = filterContext.HttpContext.User.Logged();

            // Method can execute regardless of roles
            if (Roles.Length == 0)
                return;

            var shouldThrow = true;
            foreach (var expectedRole in Roles)
            {
                var hasMatchingRole = filterContext.HttpContext.User.IsInRole(expectedRole);
                if (hasMatchingRole)
                {
                    shouldThrow = false;
                    break;
                }
            }

            if (shouldThrow)
            {
                throw new InvalidRoleException(AppMessages.Err_UnauthorizedOperation);
            }
        }
    }
}