using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFanuc
{
    public partial class Form1 : Form
    {

        ushort uHandleNo;
        bool isConnected = false;
        Stopwatch sw = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            short ret_value;

            if (!isConnected)
            {
                
                lblStatus.Text = "Connecting..";
                lblStatus.Refresh();

                ret_value = Focas1.cnc_allclibhndl3(txtIP_Address.Text, 8193, 5, out uHandleNo);

                if ( ret_value == Focas1.EW_OK)
                {
                    lblStatus.Text = "Connected..";
                    btnConnect.Text = "Disconnect";
                    isConnected = true;
                    timer1.Enabled = true;
                }
                else
                {
                    lblStatus.Text = String.Format("Error CODE {0} {1}", ret_value, Enum.GetName(typeof(Focas1.focas_ret), ret_value));
                    lblStatus.Refresh();
                    System.Media.SystemSounds.Exclamation.Play();

                    isConnected = false;
                    timer1.Enabled = false;
                }
            }
            else
            {
                btnConnect.Text = "Connect";
                lblStatus.Text = "-";

                Focas1.cnc_freelibhndl(uHandleNo);
               
                isConnected = false;
                timer1.Enabled = false;
            }
        }

        private void FANUC_ReadSystemInformation()
        {
            sw.Reset();
            sw.Start();

            string ResultTest = "";
            string[] strModeSelection = new string[11]
            {
                "MDI", "MEMORY", "***", "EDIT", "HANDLE", "JOG", "Teach in JOG", "Teach in HANDLE", "INC FEED", "REFERENCE", "REMOTE"
            };

            string[] strAutoOperation = new string[5]
            {
                "RESET","STOP","HOLD","START","MSTR"
            };

            short ret;

            Focas1.ODBST buf = new Focas1.ODBST();
            Focas1.cnc_statinfo(uHandleNo, buf);

            lblModeSelection.Text = strModeSelection[buf.aut];
            lblAutoOperation.Text = strAutoOperation[buf.run];

            //ODBACT ( Spindle Rotation read )
            Focas1.ODBACT buf_ODBACT = new Focas1.ODBACT();
            Focas1.cnc_acts(uHandleNo, buf_ODBACT);
            lblSpindleSpeed.Text = buf_ODBACT.data.ToString() + " rpm";

            //Diagnosis
            Focas1.ODBDIAGIF buf_diag = new Focas1.ODBDIAGIF();
            Focas1.cnc_rddiaginfo(uHandleNo, 0403, 1, buf_diag);
            lblDiagnosisNum.Text = buf_diag.info_no.ToString();
            lblDiagnosisVal.Text = buf_diag.info.info1.diag_no.ToString();
            lblDiagnosisType.Text = buf_diag.info.info1.diag_type.ToString();

            //Diagnosis Value
            Focas1.ODBDGN buf_real = new Focas1.ODBDGN();
            Focas1.cnc_diagnoss(uHandleNo, 0403, 1, 8, buf_real);
            lblDIAGVal.Text = buf_real.u.idata.ToString();
            lblDIAGVal2.Text = buf_real.u.ldata.ToString();

            //Diagnosis All Axis
            //ret = Focas1.cnc_diagnoss(uHandleNo, 3711, 1, 4+(4*4), buf_real);
            
            Focas1.ODBDGN buf_real2 = new Focas1.ODBDGN();
            //ret = Focas1.cnc_diagnoss(uHandleNo, 3712, 3, 4 + (4 * 4), buf_real2);
            ret = Focas1.cnc_diagnoss(uHandleNo, 0308, 32, (4+4)*32, buf_real2);
            if (ret == Focas1.EW_OK)
            {
                lblDiagValX.Text = buf_real2.u.ldatas[0].ToString();
                lblDiagValY.Text = buf_real2.u.ldatas[1].ToString();
                lblDiagValZ.Text = buf_real2.u.ldatas[2].ToString();
                lblDiagValB.Text = buf_real2.u.ldatas[3].ToString();
            }
            else
            {
                lblDiagValX.Text = string.Format("{0} : {1}",ret, Enum.GetName(typeof(Focas1.focas_ret), ret));
            }

            // Spindle Feed,Speed
            Focas1.ODBSPEED buf_ODBSPEED = new Focas1.ODBSPEED();
            Focas1.cnc_rdspeed(uHandleNo, -1, buf_ODBSPEED);
            lblSpindleFeedrate.Text = buf_ODBSPEED.actf.data.ToString();
            lblSpindleSpeedrate.Text = buf_ODBSPEED.acts.data.ToString();

            // Servo Motor Temperature
            Focas1.ODBDGN buf_ODBDGNTEMP = new Focas1.ODBDGN();
            /*
            Focas1.cnc_diagnoss(uHandleNo, 0308, 1,(4 + 4), buf_ODBDGNTEMP);
            lblServoMotorX.Text = buf_ODBDGNTEMP.u.idata.ToString();
            Focas1.cnc_diagnoss(uHandleNo, 0308, 2, (4 + 4), buf_ODBDGNTEMP);
            lblServoMotorY.Text = buf_ODBDGNTEMP.u.idata.ToString();
            Focas1.cnc_diagnoss(uHandleNo, 0308, 3, (4 + 4), buf_ODBDGNTEMP);
            lblServoMotorZ.Text = buf_ODBDGNTEMP.u.idata.ToString();
            Focas1.cnc_diagnoss(uHandleNo, 0308, 4, (4 + 4), buf_ODBDGNTEMP);
            lblServoMotorB.Text = buf_ODBDGNTEMP.u.idata.ToString();
            */
            Focas1.cnc_diagnoss(uHandleNo, 0308, 4, (4 + 4) * 4, buf_ODBDGNTEMP);
            //string TesMulti = string.Format("{0}-{1}-{2}-{3}", buf_ODBDGNTEMP.u.ldatas[0], buf_ODBDGNTEMP.u.ldatas[1], buf_ODBDGNTEMP.u.ldatas[2], buf_ODBDGNTEMP.u.ldatas[3]);


            // Pulse Coder Temp
            Focas1.ODBDGN buf_ODBDGNPULSE = new Focas1.ODBDGN();
            Focas1.cnc_diagnoss(uHandleNo, 0309, 1, (4 + 4), buf_ODBDGNPULSE);
            lblPulseCoderX.Text = buf_ODBDGNPULSE.u.idata.ToString();
            Focas1.cnc_diagnoss(uHandleNo, 0309, 2, (4 + 4), buf_ODBDGNPULSE);
            lblPulseCoderY.Text = buf_ODBDGNPULSE.u.idata.ToString();
            Focas1.cnc_diagnoss(uHandleNo, 0309, 3, (4 + 4), buf_ODBDGNPULSE);
            lblPulseCoderZ.Text = buf_ODBDGNPULSE.u.idata.ToString();
            Focas1.cnc_diagnoss(uHandleNo, 0309, 4, (4 + 4), buf_ODBDGNPULSE);
            lblPulseCoderB.Text = buf_ODBDGNPULSE.u.idata.ToString();
            
            // Spindle Motor Temperature
            Focas1.ODBDGN buf_ODBDGNMOTOR = new Focas1.ODBDGN();
            Focas1.cnc_diagnoss(uHandleNo, 0403, 1, (4 + 4) * 8, buf_ODBDGNMOTOR);
            lblSpindleMotor.Text = buf_ODBDGNMOTOR.u.idata.ToString();

            // Spindle Load
            Focas1.ODBDGN buf_ODBDGNLOAD = new Focas1.ODBDGN();
            Focas1.cnc_diagnoss(uHandleNo, 0410, 1, (4 + 4) * 8, buf_ODBDGNLOAD);
            lblSpindleLoad.Text = buf_ODBDGNLOAD.u.idata.ToString();

            // DC Link Voltage 
            Focas1.ODBDGN buf_ODBDGNDC = new Focas1.ODBDGN();
            Focas1.cnc_diagnoss(uHandleNo, 0752, -1, (4 + 8) * 8, buf_ODBDGNDC);
            lblLinkVoltageX.Text = buf_ODBDGNDC.u.idatas[0].ToString();
            lblLinkVoltageY.Text = buf_ODBDGNDC.u.idatas[1].ToString();
            lblLinkVoltageZ.Text = buf_ODBDGNDC.u.idatas[2].ToString();
            lblLinkVoltageB.Text = buf_ODBDGNDC.u.idatas[3].ToString();

            // Fan Speed 1
            Focas1.ODBDGN buf_ODBDGNFAN1 = new Focas1.ODBDGN();
            Focas1.cnc_diagnoss(uHandleNo, 01002, -1, (4 + 4) * 8, buf_ODBDGNFAN1);
            lblFanSpeed1.Text = buf_ODBDGNFAN1.u.idatas[0].ToString();

            // Fan Speed 2
            Focas1.ODBDGN buf_ODBDGNFAN2 = new Focas1.ODBDGN();
            Focas1.cnc_diagnoss(uHandleNo, 01003, -1, (4 + 4) * 8, buf_ODBDGNFAN2);
            lblFanSpeed2.Text = buf_ODBDGNFAN2.u.idatas[0].ToString();

            // Spindle Load Max Current
            Focas1.ODBDGN buf_ODBDGNMAXCURRENT = new Focas1.ODBDGN();
            Focas1.cnc_diagnoss(uHandleNo, 01581, 1, (4 + 4) * 8, buf_ODBDGNMAXCURRENT);
            lblSpindleLoadMax.Text = buf_ODBDGNMAXCURRENT.u.idata.ToString();

            // SV Int Cooling Fan1 Speed
            Focas1.ODBDGN buf_ODBDGNSV = new Focas1.ODBDGN();
            Focas1.cnc_diagnoss(uHandleNo, 01711, -1, (4 + 8) * 8, buf_ODBDGNSV);
            lblSVCoolingFan1X.Text = buf_ODBDGNSV.u.idatas[0].ToString();
            lblSVCoolingFan1Y.Text = buf_ODBDGNSV.u.idatas[1].ToString();
            lblSVCoolingFan1Z.Text = buf_ODBDGNSV.u.idatas[2].ToString();
            lblSVCoolingFan1B.Text = buf_ODBDGNSV.u.idatas[3].ToString();
            //ResultTest = buf_ODBDGNSV.u.idata.ToString();

            // Info
            Focas1.ODBSYS buf_ODBSYSINFO = new Focas1.ODBSYS();
            Focas1.cnc_sysinfo(uHandleNo, buf_ODBSYSINFO);
            lblInfoCncType.Text = string.Format("Series {0}{1}i", buf_ODBSYSINFO.cnc_type[0], buf_ODBSYSINFO.cnc_type[1]);
            lblInfoMtType.Text = string.Format("Type {0}{1}", buf_ODBSYSINFO.mt_type[0], buf_ODBSYSINFO.mt_type[1]);
            lblInfoSeries.Text = string.Format("{0}{1}{2}{3}", buf_ODBSYSINFO.series[0], buf_ODBSYSINFO.series[1], buf_ODBSYSINFO.series[2], buf_ODBSYSINFO.series[3]);
            lblInfoVersion.Text = string.Format("{0}{1}{2}{3}",buf_ODBSYSINFO.version[0], buf_ODBSYSINFO.version[1], buf_ODBSYSINFO.version[2], buf_ODBSYSINFO.version[3]);
            lblInfoAxes.Text = string.Format("{0}{1}", buf_ODBSYSINFO.axes[0],buf_ODBSYSINFO.axes[1]);
            
            // Position Absolute
            Focas1.ODBAXIS buf_ODBAXISABSOLUTE = new Focas1.ODBAXIS();
            Focas1.cnc_absolute(uHandleNo, 1, (4 + 4), buf_ODBAXISABSOLUTE);
            lblPositionAbsoluteX.Text = buf_ODBAXISABSOLUTE.data[0].ToString();
            Focas1.cnc_absolute(uHandleNo, 2, (4 + 4) * 3, buf_ODBAXISABSOLUTE);
            lblPositionAbsoluteY.Text = buf_ODBAXISABSOLUTE.data[0].ToString();
            Focas1.cnc_absolute(uHandleNo, 3, (4 + 4) * 3, buf_ODBAXISABSOLUTE);
            lblPositionAbsoluteZ.Text = buf_ODBAXISABSOLUTE.data[0].ToString();
            Focas1.cnc_absolute(uHandleNo, 4, (4 + 4) * 3, buf_ODBAXISABSOLUTE);
            lblPositionAbsoluteB.Text = buf_ODBAXISABSOLUTE.data[0].ToString();
            //ResultTest = string.Format("{0}{1}{2}{3} AXIS:{4}", buf_ODBAXISABSOLUTE.data[4], buf_ODBAXISABSOLUTE.data[5], buf_ODBAXISABSOLUTE.data[6], buf_ODBAXISABSOLUTE.data[7], buf_ODBAXISABSOLUTE.type);

            // Position Relative
            Focas1.ODBAXIS buf_ODBAXISRELATIVE = new Focas1.ODBAXIS();
            Focas1.cnc_relative(uHandleNo, 1, (4 + 4), buf_ODBAXISRELATIVE);
            lblPositionRelativeX.Text = buf_ODBAXISRELATIVE.data[0].ToString();
            Focas1.cnc_relative(uHandleNo, 2, (4 + 4), buf_ODBAXISRELATIVE);
            lblPositionRelativeY.Text = buf_ODBAXISRELATIVE.data[0].ToString();
            Focas1.cnc_relative(uHandleNo, 3, (4 + 4), buf_ODBAXISRELATIVE);
            lblPositionRelativeZ.Text = buf_ODBAXISRELATIVE.data[0].ToString();
            Focas1.cnc_relative(uHandleNo, 4, (4 + 4), buf_ODBAXISRELATIVE);
            lblPositionRelativeB.Text = buf_ODBAXISRELATIVE.data[0].ToString();

            // Position Distance
            Focas1.ODBAXIS buf_ODBAXISDISTANCE = new Focas1.ODBAXIS();
            Focas1.cnc_distance(uHandleNo, 1, (4 + 4), buf_ODBAXISDISTANCE);
            lblPositionDistanceX.Text = buf_ODBAXISDISTANCE.data[0].ToString();
            Focas1.cnc_distance(uHandleNo, 2, (4 + 4), buf_ODBAXISDISTANCE);
            lblPositionDistanceY.Text = buf_ODBAXISDISTANCE.data[0].ToString();
            Focas1.cnc_distance(uHandleNo, 3, (4 + 4), buf_ODBAXISDISTANCE);
            lblPositionDistanceZ.Text = buf_ODBAXISDISTANCE.data[0].ToString();
            Focas1.cnc_distance(uHandleNo, 4, (4 + 4), buf_ODBAXISDISTANCE);
            lblPositionDistanceB.Text = buf_ODBAXISDISTANCE.data[0].ToString();
            
            // Spindle Torque
            short x = 1;
            Focas1.ODBSPLOAD buf_ODBSPLOAD = new Focas1.ODBSPLOAD();
            Focas1.cnc_rdspmeter(uHandleNo, -1, ref x, buf_ODBSPLOAD);
            lblSpindleTorqueLoad.Text = buf_ODBSPLOAD.spload1.spload.data.ToString();
            lblSpindleTorqueMotor.Text = buf_ODBSPLOAD.spload1.spspeed.data.ToString();
            ResultTest = string.Format("{0} - {1} - {2} - {3}",buf_ODBSPLOAD.spload1.spload.data, buf_ODBSPLOAD.spload2.spload.data, buf_ODBSPLOAD.spload3.spload.data, buf_ODBSPLOAD.spload4.spload.data);

            // Servo Motor Load
            short a = 1;
            Focas1.ODBSVLOAD buf_ODBSVLOAD = new Focas1.ODBSVLOAD();
            Focas1.cnc_rdsvmeter(uHandleNo, ref a, buf_ODBSVLOAD);
            lblServoLoadX.Text = buf_ODBSVLOAD.svload1.data.ToString();
            a = 2;
            Focas1.cnc_rdsvmeter(uHandleNo, ref a, buf_ODBSVLOAD);
            lblServoLoadY.Text = buf_ODBSVLOAD.svload1.data.ToString();
            a = 3;
            Focas1.cnc_rdsvmeter(uHandleNo, ref a, buf_ODBSVLOAD);
            lblServoLoadZ.Text = buf_ODBSVLOAD.svload1.data.ToString();
            a = 4;
            Focas1.cnc_rdsvmeter(uHandleNo, ref a, buf_ODBSVLOAD);
            lblServoLoadB.Text = buf_ODBSVLOAD.svload1.data.ToString();

            // TEST
            sw.Stop();
            ResultTest = sw.ElapsedMilliseconds.ToString() + " - " + ResultTest;
            textBox2.Text = ResultTest;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isConnected)
            {
                Focas1.cnc_freelibhndl(uHandleNo);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            FANUC_ReadSystemInformation();
        }

        private void btnClass_Click(object sender, EventArgs e)
        {
            
            
        }
    }

     
}
