using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace CZHSoft.WCF
{
    public class WcfServer
    {
        private ServiceHost serviceHost;
        private string uriAddress = string.Empty;

        public delegate void SendStatusMSG(string strMsg);
        public event SendStatusMSG OnSendStatusMSG;

        public delegate void DoConnect(string uri);
        public event DoConnect OnDoConnect;

        public delegate void DoDisconnect(string uri);
        public event DoDisconnect OnDoDisconnect;

        public bool WcfStart(string uri, Type serviceType, string serviceName,Type interfaceType,WcfBindingType bindingType)
        {
            // Step 1 of the address configuration procedure: Create a URI to serve as the base address.
            //Uri baseAddress = new Uri("http://198.168.0.253:1111/ServiceModelSamples/Service");
            Uri baseAddress = new Uri(uri);

            // Step 2 of the hosting procedure: Create ServiceHost
            //ServiceHost selfHost = new ServiceHost(typeof(TestService), baseAddress);
            serviceHost = new ServiceHost(serviceType, baseAddress);

            try
            {
                // Step 3 of the hosting procedure: Add a service endpoint.
                if (bindingType == WcfBindingType.BasicHttpBinding)
                {
                    serviceHost.AddServiceEndpoint(
                        //typeof(ITest),
                        interfaceType,
                        new BasicHttpBinding(),
                        //"TestService"
                        serviceName);
                }
                else if (bindingType == WcfBindingType.WSHttpBinding)
                {
                    serviceHost.AddServiceEndpoint(
                        //typeof(ITest),
                        interfaceType,
                        new WSHttpBinding(),
                        //"TestService"
                        serviceName);
                }


                // Step 4 of the hosting procedure: Enable metadata exchange.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                serviceHost.Description.Behaviors.Add(smb);

                // Step 5 of the hosting procedure: Start (and then stop) the service.
                serviceHost.Open();

                uriAddress = uri;

                if (OnSendStatusMSG != null)
                {
                    OnSendStatusMSG("The service is ready.");
                }

                if (OnDoConnect != null)
                {
                    OnDoConnect(uriAddress);
                }

                return true;
            }
            catch (CommunicationException ce)
            {
                if (OnSendStatusMSG != null)
                {
                    OnSendStatusMSG(string.Format("An exception occurred: {0}", ce.Message));
                }

                serviceHost.Abort();

                if (OnSendStatusMSG != null)
                {
                    OnSendStatusMSG("The service is abort.");
                }

                Console.WriteLine("WcfStart error!");

                return false;
            }
        }

        public void WcfClose()
        {
            if (serviceHost != null)
            {
                // Step 6 :Close the ServiceHostBase to shutdown the service.
                serviceHost.Close();

                if (OnSendStatusMSG != null)
                {
                    OnSendStatusMSG("The service had closed.");
                }

                if (OnDoDisconnect != null)
                {
                    OnDoDisconnect(uriAddress);
                    uriAddress = string.Empty;
                }
            }
        }
    }
}
