using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

//****************************************
//中华人民共和国交通运输行业标准
//JT/T 808-2011
//标识位、消息头、消息体、校验码、标识位
//1.标识位:0x7e;
//冲突转义:0x7e->0x7d+0x02\0x7d->0x7d+0x01

//****************************************
namespace CZHSoft.GPS.Protocol
{
    public class ProtocolHelper
    {
        public delegate void protocolAnalysis0200Delegate(string ip,string phoneNo,
            string alarm, string state, string lat, string lng, string altitude, string speed, string angle,string date);
        public event protocolAnalysis0200Delegate OnProtocolAnalysis0200;

        public delegate void protocolAnalysis0001Delegate(string ip,string phoneNo,
            string serialNo,string msgid,string msgRes);
        public event protocolAnalysis0001Delegate OnProtocolAnalysis0001;

        public delegate void protocolAnalysis0002Delegate(string ip,string phoneNo);
        public event protocolAnalysis0002Delegate OnProtocolAnalysis0002;

        /// <summary>
        /// 1.msgid
        /// 2.bodyLength
        /// 3.encType
        /// 4.subpackage
        /// 5.phoneNo
        /// 6.serialNo
        /// </summary>
        private List<string> MsgHead(byte[] head)
        {
            if (head != null)
            {
                if (head.Length < 12)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            List<string> returnList=new List<string>();
            //0-1
            string msgid = BitConverter.ToString(head, 0, 2).Replace("-", "");
            returnList.Add(msgid);
            //2-3
            byte[] msgBodyPro = new byte[2];
            Array.Copy(head, 2, msgBodyPro, 0, msgBodyPro.Length);
            Array.Reverse(msgBodyPro, 0, msgBodyPro.Length);
            List<int> msgBodyProList = new List<int>();
            for (int i = 0; i < msgBodyPro.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    msgBodyProList.Add(GetTargetBit(j, msgBodyPro[i]));
                }
            }
            // body Length
            int msgBodyLength = 0;
            for (int i = 0; i < 10; i++)
            {
                msgBodyLength += (int)Math.Pow(2, i) * msgBodyProList[i];
            }
            returnList.Add(msgBodyLength.ToString());
            //enc type
            string encryptType = string.Empty;
            for (int i = 10; i < 13; i++)
            {
                encryptType += msgBodyProList[i].ToString();
            }
            returnList.Add(encryptType);
            //subpackage or not
            int isSubpackage = msgBodyProList[13];
            returnList.Add(isSubpackage.ToString());
            //4-9
            byte[] phoneNo = new byte[6];
            Array.Copy(head, 4, phoneNo, 0, phoneNo.Length);
            string phoneString = BCD2Str(phoneNo);
            returnList.Add(phoneString);
            //10-11
            byte[] serialNo = new byte[2];
            Array.Copy(head, 10, serialNo, 0, serialNo.Length);
            Array.Reverse(serialNo, 0, serialNo.Length);
            int serialNoInt = BitConverter.ToUInt16(serialNo, 0);
            returnList.Add(serialNoInt.ToString());

            return returnList;
        }

        public void ProtocolAnalysis(byte[] data, string ip)
        {
            if (data[0] != 0x7e)
            {
                return;
            }

            //check code
            byte checkCode = data[1];
            for (int i = 2; i < data.Length - 2; i++)
            {
                checkCode ^= data[i];
            }

            if (checkCode != data[data.Length - 2])
            {
                return;
            }

            byte[] head = new byte[12];
            Array.Copy(data, 1, head, 0, head.Length);
            List<string> headList=MsgHead(head);
            
            if (headList != null && headList.Count>=6)
            {
                int msgBodyLength = int.Parse(headList[1]);
                byte[] msgBody = new byte[msgBodyLength];
                Array.Copy(data, 13, msgBody, 0, msgBody.Length);

                if (headList[0].Equals("0200"))
                {
                    Console.WriteLine("ProtocolAnalysis Get 0200");

                    MsgBody0200(msgBody, ip, headList[4]);

                }
                else if (headList[0].Equals("0001"))
                {
                    Console.WriteLine("ProtocolAnalysis Get 0001");

                    MsgBody0001(msgBody, ip, headList[4]);

                }
                else if (headList[0].Equals("0002"))
                {
                    Console.WriteLine("ProtocolAnalysis Get 0002");

                    MsgBody0002(msgBody, ip, headList[4]);
                }
            }
            else
            {
                return;
            }

        }

        private bool MsgBody0001(byte[] msgBody,string ip,string phoneNo)
        {
            if (msgBody.Length != 5)
            {
                return false;
            }

            //0-1 sn
            byte[] serialNo = new byte[2];
            Array.Copy(msgBody, 0, serialNo, 0, serialNo.Length);
            Array.Reverse(serialNo, 0, serialNo.Length);

            int serialNoInt = BitConverter.ToUInt16(serialNo, 0);

            //2-3 msgid
            byte[] msgId = new byte[2];
            Array.Copy(msgBody, 2, msgId, 0, msgId.Length);
            Array.Reverse(msgId, 0, msgId.Length);

            //4 res
            byte msgRes = msgBody[4];

            if (OnProtocolAnalysis0001 != null)
            {
                OnProtocolAnalysis0001(ip,phoneNo,
                    serialNoInt.ToString(), "", "");
            }

            return true;
        }

        private bool MsgBody0002(byte[] msgBody, string ip, string phoneNo)
        {
            if (OnProtocolAnalysis0002 != null)
            {
                OnProtocolAnalysis0002(ip,phoneNo);
            }

            return true;
        }

        private bool MsgBody0200(byte[] msgBody,string ip,string phoneNo)
        {
            //0-3
            byte[] alarm = new byte[4];
            Array.Copy(msgBody, 0, alarm, 0, alarm.Length);
            Array.Reverse(alarm, 0, alarm.Length);
            List<int> alarmList = new List<int>();
            for (int i = 0; i < alarm.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    alarmList.Add(GetTargetBit(j, alarm[i]));
                }
            }
            //4-7
            byte[] state = new byte[4];
            Array.Copy(msgBody, 4, state, 0, state.Length);
            Array.Reverse(state, 0, state.Length);
            List<int> stateList = new List<int>();
            for (int i = 0; i < state.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    stateList.Add(GetTargetBit(j, state[i]));
                }
            }
            //8-11
            byte[] lat = new byte[4];
            Array.Copy(msgBody, 8, lat, 0, lat.Length);
            Array.Reverse(lat, 0, lat.Length);
            double latDouble = (double)(BitConverter.ToUInt32(lat, 0)) / 1000000;
            //12-15
            byte[] lng = new byte[4];
            Array.Copy(msgBody, 12, lng, 0, lng.Length);
            Array.Reverse(lng, 0, lng.Length);
            double lngDouble = (double)(BitConverter.ToUInt32(lng, 0)) / 1000000;
            //16-17
            byte[] altitude = new byte[2];
            Array.Copy(msgBody, 16, altitude, 0, altitude.Length);
            Array.Reverse(altitude, 0, altitude.Length);
            int altitudeInt = BitConverter.ToUInt16(altitude, 0);
            //18-19
            byte[] speed = new byte[2];
            Array.Copy(msgBody, 18, speed, 0, speed.Length);
            Array.Reverse(speed, 0, speed.Length);
            int speedInt = BitConverter.ToUInt16(speed, 0);
            //20-21
            byte[] angle = new byte[2];
            Array.Copy(msgBody, 20, angle, 0, angle.Length);
            Array.Reverse(angle, 0, angle.Length);
            int angleInt = BitConverter.ToUInt16(angle, 0);
            //22-27 BCD
            byte[] dateTime = new byte[6];
            Array.Copy(msgBody, 22, dateTime, 0, dateTime.Length);
            string dateTimeString = BCD2Str(dateTime);

            if (OnProtocolAnalysis0200 != null)
            {
                OnProtocolAnalysis0200(ip,phoneNo,
                    "", "",
                    latDouble.ToString(), lngDouble.ToString(),
                    altitudeInt.ToString(), speedInt.ToString(),
                    angleInt.ToString(), dateTimeString);
            }

            return true;
        }

        public int GetTargetBit(int bitPosition, byte sourceByte)
        {
            byte baseNum = (byte)(Math.Pow(2, bitPosition + 1) / 2);
            return GetTargetBit(bitPosition, sourceByte, baseNum);
        }

        private int GetTargetBit(int bitPosition, byte sourceByte, byte baseNum)
        {
            if (bitPosition > 7 || bitPosition < 0) return -1000;
            return (sourceByte & baseNum) == baseNum ? 1 : 0;
        }

        public string BCD2Str(byte[] bytes)
        {
            StringBuilder temp = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
            {
                temp.Append((byte)((bytes[i] & 0xf0) >> 4));
                temp.Append((byte)(bytes[i] & 0x0f));
            }
            return temp.ToString().Substring(0, 1).Equals("0") ? temp.ToString().Substring(1) : temp.ToString();
        }

    }
}
