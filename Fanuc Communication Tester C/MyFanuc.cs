using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanuc
{
    public class MyFanuc
    {
        public ushort _uHandle;
        public ushort _PortNo;
        public int _TimeOut;
        public string _IpAddress;
        public Boolean _IsConnected;

        public short _MaxAxis;



        public MyFanuc(string strIpAdress, ushort sPortNo, int sTimeOut)
        {
            _IpAddress = strIpAdress;
            _PortNo = sPortNo;
            _TimeOut = sTimeOut;
            _IsConnected = false;
        }

        ~MyFanuc()
        {
            Focas1.cnc_freelibhndl(_uHandle);
        }

        public void Connect()
        {
            short ret;

            ret = Focas1.cnc_allclibhndl3(_IpAddress, _PortNo, _TimeOut, out _uHandle);

            if (ret == Focas1.EW_OK)
            {
                _IsConnected = true;
            }
            else
            {
                _IsConnected = false;
            }

        }

        public void ReadStatus()
        {
            short ret;
            /*
            Focas1.ODBSYS buf = new Focas1.ODBSYS;
            ret = Focas1.cnc_sysinfo(_uHandle, buf);

            if (ret ==  Focas1.EW_OK)
            {
                _MaxAxis = buf.max_axis();

            }
            */
            

        }
    
    }

}
