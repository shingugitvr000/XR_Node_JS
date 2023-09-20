using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protocols 
{
    public class Packets
    {
        public class common
        {
            public int cmd;             //Ŀ��Ʈ ��ȣ
            public string message;      //�޼���
        }

        public class req_data : common  //Common ���
        {
            public int id;
            public string data;
        }

        public class res_data : common  //���� �迭 ��Ŷ ���� 
        {
            public req_data[] result;
        }

        public class ConstructionStatusResponse     //�Ǽ� ��Ŷ ����
        {
            public string message;
        }

    }
}
