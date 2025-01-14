using System;
using UnityEngine;

public partial class Director
{
    /// <summary>
    /// �]�w�H���ȡC
    /// </summary>
    /// <param name="args">�Ѽư}�C�Aargs[0] ���H���]�w�r��</param>
    private void SetRandomValue(object[] args)
    {
        if (args == null || args.Length == 0 || args[0] == null)
        {
            Debug.LogError("�Ѽƿ��~�Aargs[0] ����");
            return;
        }

        // �N args[0] �ର�r��
        string input = args[0].ToString();

        if (input.Contains("~"))
        {
            // �B�z "~" �H���d��
            string[] parts = input.Split('~');

            if (parts.Length == 2)
            {
                string startStr = parts[0].Trim();
                string endStr = parts[1].Trim();

                if (int.TryParse(startStr, out int startInt) && int.TryParse(endStr, out int endInt))
                {
                    // ��ƽd��
                    int randomInt = UnityEngine.Random.Range(startInt, endInt + 1);
                    PPM.Inst.Set("�H����", randomInt.ToString());
                }
                else if (float.TryParse(startStr, out float startFloat) && float.TryParse(endStr, out float endFloat))
                {
                    // �B�I�ƽd��
                    float randomFloat = UnityEngine.Random.Range(startFloat, endFloat);

                    // ���o�p�Ʀ��
                    int decimalPlaces = Math.Max(startStr.Length - startStr.IndexOf('.') - 1, endStr.Length - endStr.IndexOf('.') - 1);

                    // �榡�Ƭ��ۦP�p�Ʀ��
                    string formattedFloat = randomFloat.ToString("F" + decimalPlaces);
                    PPM.Inst.Set("�H����", formattedFloat);
                }
                else
                {
                    Debug.LogError("�H���ȽШD���e��Ȥ��O�X�k�ƭ�" + input);
                }
            }
            else
            {
                Debug.LogError("�H���ȽШD�榡���~: " + input);
            }
        }
        else if (input.Contains("&"))
        {
            // �B�z "&" �H�����
            string[] options = input.Split('&');

            if (options.Length > 0)
            {
                string randomOption = options[UnityEngine.Random.Range(0, options.Length)].Trim();
                PPM.Inst.Set("�H����", randomOption);
            }
            else
            {
                Debug.LogError("�H���ȽШD�ʤֿﶵ: " + input);
            }
        }
        else
        {
            Debug.LogError("�H���ȽШD�Ѽƿ��~: " + input);
        }
    }
}
