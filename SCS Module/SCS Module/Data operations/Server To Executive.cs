using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS_Module
{
    class ServerToExecutive
    {
        public RequestType responseType;
        public Equipment equipment = null;
        public List<Equipment> equipments = null;
        public List<InterfaceType> interfaceTypes = null;

        public bool success = true;
    }
}
