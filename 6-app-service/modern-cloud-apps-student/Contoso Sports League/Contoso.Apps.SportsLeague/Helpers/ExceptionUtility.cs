using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Contoso.Apps.SportsLeague.Web.Helpers
{
    // Create our own utility for exceptions.
    public sealed class ExceptionUtility
    {
        // All methods are static, so this can be private
        private ExceptionUtility()
        { }

        // Log an Exception to Application Insights.
        public static void LogException(Exception exc, string source)
        {
            TelemetryHelper.TrackException(exc);
        }

    }
}