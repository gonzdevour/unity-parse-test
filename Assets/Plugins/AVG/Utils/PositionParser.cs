using UnityEngine;

namespace story
{
    public class PositionParser
    {
        public static Vector2[] ParsePos(string input, RectTransform panelAVG, string pivot = "x")
        {
            // 解析位置的主函數
            Vector2[] positions = new Vector2[2];

            // 判斷是否包含 "-"
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
                positions[0] = point; // 起點未定義時默認為終點相同
                positions[1] = point;
            }

            return positions;
        }

        public static Vector2 ParsePoint(string pointStr, RectTransform panelAVG, string pivot = "x")
        {
            float pWidth = panelAVG.rect.width;
            float pHeight = panelAVG.rect.height;

            // 如果包含逗號，處理多維座標
            if (pointStr.Contains(","))
            {
                string[] coords = pointStr.Split(',');
                float x = ParseCoordinate(coords[0].Trim(), pWidth, -0.5f*pWidth); //left
                float y = ParseCoordinate(coords[1].Trim(), pHeight, 0); //y的anchor為0所以不須調整
                return new Vector2(x, y);
            }

            // 單一值處理
            if (pivot == "x")
            {
                float xValue = ParseCoordinate(pointStr.Trim(), pWidth, -0.5f * pWidth);//left
                return new Vector2(xValue, 0);
            }
            else
            {
                float yValue = ParseCoordinate(pointStr.Trim(), pHeight, 0);////y的anchor為0所以不須調整
                return new Vector2(0, yValue);
            }

        }

        public static float ParseCoordinate(string coordStr, float panelSize, float origin)
        {
            // 處理 "左", "中", "右"
            if (coordStr == "左") return origin + 0.25f * panelSize;
            if (coordStr == "中") return origin + 0.5f * panelSize;
            if (coordStr == "右") return origin + 0.75f * panelSize;

            // 處理數值
            if (float.TryParse(coordStr, out float coordValue))
            {
                // 如果是 0~1 之間的浮點數，計算相對位置
                if (coordValue >= 0 && coordValue <= 1)
                {
                    //Debug.Log($"位置參數為：{coordValue}");
                    return origin + coordValue * panelSize;
                }
            }

            // 無法解析則回傳中央
            return origin + 0.5f * panelSize;
        }
    }

}