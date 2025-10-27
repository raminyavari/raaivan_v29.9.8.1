using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.Web.Services;

namespace RaaiVanIntermediate
{
    public static class CoreNetwork
    {
        private static RaaiVanCoreNetwork.CoreNetworkSoapClient _ServiceHandler;
        private static RaaiVanCoreNetwork.CoreNetworkSoapClient ServiceHandler
        {
            get
            {
                if (_ServiceHandler == null)
                    _ServiceHandler = new RaaiVanCoreNetwork.CoreNetworkSoapClient("CoreNetworkSoap", "http://127.0.0.1/Services/CoreNetwork.asmx");

                return _ServiceHandler;
            }
        }

        public static bool AddNodeType(string nodeTypeId, string typeName, string description)
        {
            return ServiceHandler.AddNodeType(nodeTypeId, typeName, description);
        }

        public static bool AddNode(string nodeId, string nodeType, string name, string description, string parentNodeId)
        {   
            return ServiceHandler.AddNode(nodeId, nodeType, name, description, parentNodeId);
        }

        public static bool ModifyNode(string nodeId, string nodeTypeId, string name, string description)
        {
            return ServiceHandler.ModifyNode(nodeId, nodeTypeId, name, description);
        }

        public static bool MoveNode(string nodeId, string parentNodeId, string nodeTypeId)
        {
            return ServiceHandler.MoveNode(nodeId, parentNodeId, nodeTypeId);
        }
    }
}
