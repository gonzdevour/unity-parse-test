using UnityEngine;

namespace story
{
    public class PositionParser
    {
        public static Vector2[] ParsePos(string input, RectTransform panelAVG, string pivot = "x")
        {
            // �ѪR��m���D���
            Vector2[] positions = new Vector2[2];

            // �P�_�O�_�]�t "-"
            if (input.Contains("~"))
            {
                string[] parts = input.Split('~');
                string startStr = parts[0].Trim();
                string endStr = parts[1].Trim();

                positions[0] = ParsePoint(startStr, panelAVG, pivot);
                positions[1] = ParsePoint(endStr, panelAVG, pivot);
            }
            else
            {
                string endStr = input.Trim();
                Vector2 point = ParsePoint(endStr, panelAVG, pivot);
                positions[0] = point; // �_�I���w�q���q�{�����I�ۦP
                positions[1] = point;
            }

            return positions;
        }

        public static Vector2 ParsePoint(string pointStr, RectTransform panelAVG, string pivot = "x")
        {
            float pWidth = panelAVG.rect.width;
            float pHeight = panelAVG.rect.height;

            // �p�G�]�t�r���A�B�z�h���y��
            if (pointStr.Contains(","))
            {
                string[] coords = pointStr.Split(',');
                float x = ParseCoordinate(coords[0].Trim(), pWidth, -0.5f*pWidth); //left
                float y = ParseCoordinate(coords[1].Trim(), pHeight, 0); //y��anchor��0�ҥH�����վ�
                return new Vector2(x, y);
            }

            // ��@�ȳB�z
            if (pivot == "x")
            {
                float xValue = ParseCoordinate(pointStr.Trim(), pWidth, -0.5f * pWidth);//left
                return new Vector2(xValue, 0);
            }
            else
            {
                float yValue = ParseCoordinate(pointStr.Trim(), pHeight, 0);////y��anchor��0�ҥH�����վ�
                return new Vector2(0, yValue);
            }

        }

        public static float ParseCoordinate(string coordStr, float panelSize, float origin)
        {
            // �B�z "��", "��", "�k"
            if (coordStr == "��") return origin + 0.25f * panelSize;
            if (coordStr == "��") return origin + 0.5f * panelSize;
            if (coordStr == "�k") return origin + 0.75f * panelSize;

            // �B�z�ƭ�
            if (float.TryParse(coordStr, out float coordValue))
            {
                // �p�G�O 0~1 �������B�I�ơA�p��۹��m
                if (coordValue >= 0 && coordValue <= 1)
                {
                    //Debug.Log($"��m�ѼƬ��G{coordValue}");
                    return origin + coordValue * panelSize;
                }
            }

            // �L�k�ѪR�h�^�Ǥ���
            return origin + 0.5f * panelSize;
        }
    }

}