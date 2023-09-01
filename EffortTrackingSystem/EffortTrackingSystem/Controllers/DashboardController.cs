using Common.Models;
using EffortTrackingSystem.Attributes;
using EffortTrackingSystem.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    /// <summary>
    /// Controller for the user dashboard.
    /// </summary>
    [CommonAuthorize]
    public class DashboardController : BaseController
    {
        /// <summary>
        /// Displays the user dashboard.
        /// </summary>
        /// <returns>Dashboard view.</returns>
        public ActionResult Index()
        {
            //throw new Exception();
            ViewBag.LoggedIn = TempData["LoggedIn"] as string;
            int userId = GetCurrentUserId();

            AssignTask model = _assignTaskDataAccess.GetPresentTaskForUser(userId);
            List<Effort> previousEfforts = GetLastWeekEffortsForUser(userId);
            ViewBag.PreviousEfforts = previousEfforts;

            return View(model);
        }

        /// <summary>
        /// Gets the current user's ID from the session.
        /// </summary>
        /// <returns>The current user's ID.</returns>
        private int GetCurrentUserId()
        {
            return (int)Session["Id"];
        }

        /// <summary>
        /// Retrieves approved efforts from the last week for the user.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <returns>List of approved efforts from the last week.</returns>
        private List<Effort> GetLastWeekEffortsForUser(int userId)
        {
            DateTime today = DateTime.Now.Date;
            int numberOfDaysToSubtract = Convert.ToInt32(ConfigurationManager.AppSettings["ToGetLastWeekStartDate"]);
            int numberOfDaysToAdd = Convert.ToInt32(ConfigurationManager.AppSettings["ToGetLastWeekEndDate"]);
            DateTime lastWeekStart = today.AddDays(-(int)today.DayOfWeek - numberOfDaysToSubtract);
            DateTime lastWeekEnd = lastWeekStart.AddDays(numberOfDaysToAdd);

            return _effortDataAccess.GetApprovedEffortsOfUser(userId)
                .Where(e => e.SubmittedDate >= lastWeekStart && e.SubmittedDate <= lastWeekEnd)
                .ToList();
        }
    }
}
