#if DISPATCH_DLL
using System.Collections.Generic;
using Quic.Integration.QuicDispatch;

namespace Quic.Dispatch
{
    class EngineAudit : IEngineAudit
    {
        public Dictionary<string, string> Message { get; set; }

        public EngineAudit()
        {
            Message = new Dictionary<string, string>();
        }

        public int GetAuditLevel()
        {
            return 3;
        }

        public void Audit(string message)
        {
            Message.Add("Audit", message);
        }

        public void AuditError(string message)
        {
            Message.Add("Error", message);
        }

        public void AuditWarning(string message)
        {
            Message.Add("Warning", message);
        }

        public void AuditRemark(string message)
        {
            Message.Add("Remark", message);
        }
    }
}
#endif
