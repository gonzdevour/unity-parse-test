using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Leaderboard
{
    class Cell_UserList : FancyScrollRectCell<ChatUserData, Context>
    {
        [SerializeField] Text txName = default;
        [SerializeField] Image portrait = default;
        [SerializeField] Image cellImage = default;
        [SerializeField] Button cellButton = default;

        public override void Initialize()
        {
            cellButton.onClick.AddListener(() => Context.OnCellClicked?.Invoke(Index));
        }

        public override void UpdateContent(ChatUserData data) //這個函數應該是在UpdateContents(items)時由plugin自動執行
        {

            txName.text = data.UserName;

            if (portrait != null && data.ImageUrl != null)
            {
                var imgUrl = data.ImageUrl;

                SpriteCacher spriteCacher = FindObjectOfType<SpriteCacher>();
                Image image = portrait.GetComponent<Image>();

                //string address;

                //multiple sprite測試
                //address = "Resources://Sprites/KennyArt/square" + "|" + imgUrl;

                //single sprite測試
                //imgUrl = "green_checkmark";
                //address = "Resources://Sprites/KennyArt" + "/" + imgUrl;

                ////網路圖片測試
                //imgUrl = "https://playoneapps.com.tw/File/Stand/Hero/image01.png";
                //address = imgUrl;

                //sa圖片測試
                //imgUrl = "StreamingAssets://Image/duck.png";
                //address = imgUrl;

                spriteCacher.GetSprite(imgUrl, (sprite) =>
                {
                    if (image != null) //避免回傳時物件已刪除
                    {
                        image.sprite = sprite;
                    };
                });
            }
            else
            {
                Debug.LogWarning($"Failed to load sprite or portrait is null. Path: {data.ImageUrl}");
            }

            var selected = Context.SelectedIndex == Index;
            cellImage.color = selected
                ? new Color32(0, 255, 255, 100)
                : new Color32(255, 255, 255, 77);
        }

        protected override void UpdatePosition(float normalizedPosition, float localPosition)
        {
            base.UpdatePosition(normalizedPosition, localPosition);

            //設定左右位移量
            var wave = Mathf.Sin(normalizedPosition * Mathf.PI * 2) * 0;
            transform.localPosition += Vector3.right * wave;
        }
    }
}
