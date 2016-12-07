using System;
using System.Diagnostics;
using Microsoft.Owin.Logging;

namespace IdentityServer3.AccessTokenValidation
{
    /// <summary>
    /// Fallback logger for when OWIN hands in a null logger factory
    /// </summary>
    public class TraceLogger: ILogger
    {
        /// <inheritdoc />
        public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            Trace.WriteLine($"[${eventId}] {eventType} :: ${formatter(state, exception)}");
            return true;
        }
    }
}