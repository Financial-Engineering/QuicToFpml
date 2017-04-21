#if DISPATCH_DLL
using System;
using Quic.Integration.QuicDispatch;

namespace Quic.Dispatch
{
    class EngineProgress : IEngineProgress
    {
        public string Message { get; set; }
        public double CompletionRatio { get; set; }

        public void Report(string message, double completionRatio)
        {
            Message = message;
            CompletionRatio = completionRatio;
        }

        public bool CheckForAbort()
        {
            return false;
        }

        public Exception GetError()
        {
            return new Exception(Message);
        }
    }
}
#endif
