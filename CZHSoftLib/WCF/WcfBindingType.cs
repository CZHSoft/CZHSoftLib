using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZHSoft.WCF
{
    public enum WcfBindingType
    {
        BasicHttpBinding,
        NetMsmqBinding,
        NetNamedPipeBinding,
        NetPeerTcpBinding,
        NetTcpBinding,
        WS2007HttpBinding,
        WSDualHttpBinding,
        WSFederationHttpBinding,
        WSHttpBinding,
    }
}
