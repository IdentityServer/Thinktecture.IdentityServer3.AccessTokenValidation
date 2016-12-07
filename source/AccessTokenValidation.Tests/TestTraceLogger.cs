using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using IdentityServer3.AccessTokenValidation;
using Xunit;

namespace AccessTokenValidation.Tests
{
    public class TestTraceLogger: IDisposable
    {
        private readonly SimpleTraceListener _traceListener;
        public TestTraceLogger()
        {
            _traceListener = new SimpleTraceListener();
            Trace.Listeners.Add(_traceListener);
        }
        public void Dispose()
        {
            Trace.Listeners.Remove(_traceListener);
        }

        public class SimpleTraceListener: TraceListener
        {
            public IEnumerable<string> RawMessages => _rawMessages;
            private readonly List<string> _rawMessages = new List<string>();
            public IEnumerable<string> LineMessages => _lineMessages;
            private readonly List<string> _lineMessages = new List<string>();
            public override void Write(string message)
            {
                _rawMessages.Add(message);
            }

            public override void WriteLine(string message)
            {
                _lineMessages.Add(message);
            }
        }

        [Fact]
        public void WriteCore_ShouldWriteToTrace()
        {
            // Arrange
            var sut = new TraceLogger();
            var eventType = TraceEventType.Information;
            var eventId = 42;
            var state = new { foo = "bar" };
            var exception = new Exception("Something bad happened )':");
            Func<object, Exception, string> formatter = (s, e) => $"{e.Message} / { s.ToString() }";
            var expected = $"[${eventId}] {eventType} :: ${formatter(state, exception)}";

            // Act
            var result = sut.WriteCore(
                TraceEventType.Information, 
                eventId,
                state,
                exception,
                formatter
            );

            // Assert
            result.Should().BeTrue();
            _traceListener.RawMessages.Should().BeEmpty();
            _traceListener.LineMessages.Should().NotBeEmpty();
            var lineMessage = _traceListener.LineMessages.Single();
            lineMessage.Should().Be(expected);
        }

    }
}