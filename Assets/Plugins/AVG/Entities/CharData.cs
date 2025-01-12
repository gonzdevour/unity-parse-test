namespace Story
{
    // 定義表格資料結構
    public class CharData
    {
        public string UID { get; set; } // 唯一識別碼

        public string 姓 { get; set; } // 姓
        public string 名 { get; set; } // 名
        public string 敬稱 { get; set; } // 敬稱 (如：先生/小姐)
        public string 職稱 { get; set; } // 職稱

        public string 暱稱1 { get; set; } // 暱稱1
        public string 暱稱2 { get; set; } // 暱稱2

        public float Scale { get; set; } // 縮放比例
        public int YAdd { get; set; } // Y 軸位移

        public string AssetID { get; set; } // 資產 ID

        // 表情相關屬性
        public string 無 { get; set; } // 無表情
        public string 喜 { get; set; } // 喜
        public string 怒 { get; set; } // 怒
        public string 樂 { get; set; } // 樂
        public string 驚 { get; set; } // 驚
        public string 疑 { get; set; } // 疑
        public string 暈 { get; set; } // 暈
    }
}