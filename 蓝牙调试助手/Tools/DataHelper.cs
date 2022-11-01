using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 蓝牙调试助手.Tools
{
    public class DataHelper
    {
        /// <summary>
        /// 低字节在前
        /// </summary>
        /// <param name="pDataBytes"></param>
        /// <returns></returns>
        public static byte[] CRC16LH(byte[] pDataBytes)
        {
            ushort crc = 0xffff;
            ushort polynom = 0xA001;

            for (int i = 0; i < pDataBytes.Length; i++)
            {
                crc ^= pDataBytes[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x01) == 0x01)
                    {
                        crc >>= 1;
                        crc ^= polynom;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            byte[] result = BitConverter.GetBytes(crc);
            return result;
        }



        /// <summary>
        /// 验证数据，并 返回数据
        /// </summary>
        /// <param name="inData">源数据</param>
        /// <param name="outData">返回数据</param>
        /// <returns>成功与失败</returns>
        public static bool asdasd(byte[] inData,uint length, out byte[] outData)
        {
            outData = inData.Take((int)length).ToArray();//截取数据

            byte[] crc1 = inData.Skip((int)length).Take(2).ToArray();

            byte[] crc = CRC16LH(outData);//计算crc

            if(crc1[0] == crc[0] && crc1[1] == crc[1])
            {
                return true;
            }

            return false;
        }

    }
}
