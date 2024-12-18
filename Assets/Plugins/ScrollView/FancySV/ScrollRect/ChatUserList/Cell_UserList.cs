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

            // 從 Resources 資料夾中加載新的 Sprite           
            string imagePath = $"Sprites/KennyArt/round_nodetails".Trim(); // 構建完整路徑
            Sprite[] loadedSprite = Resources.LoadAll<Sprite>(imagePath); // 加載圖片
            //Debug.Log($"Loaded sprites count: {(loadedSprite != null ? loadedSprite.Length : 0)} from path: {imagePath}");

            // 確認 portrait 和 loadedSprite 均有效
            if (portrait != null && loadedSprite != null && data.ImageUrl != null)
            {
                portrait.sprite = loadedSprite[int.Parse(data.ImageUrl)]; // 更改圖像
            }
            else
            {
                //Debug.LogWarning($"Failed to load sprite or portrait is null. Path: {imagePath}");
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
