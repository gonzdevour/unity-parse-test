mergeInto(LibraryManager.library, {
    UploadFileWebGL: function (objectNamePtr) {
        const objectName = UTF8ToString(objectNamePtr);

        const input = document.createElement('input');
        input.type = 'file';
        input.onchange = () => {
            const file = input.files[0];
            if (!file) {
                console.error("No file selected.");
                return;
            }

            const fileExtension = '.' + file.name.split('.').pop().toLowerCase(); // 提取副檔名
            const reader = new FileReader();

            reader.onload = () => {
                const base64Data = reader.result.split(',')[1]; // 取得 Base64 數據部分
                const fileName = file.name;

                // 將文件數據和資訊傳回 Unity
                UnityInstance.SendMessage(objectName, "GetFileFromPathForWebGL", JSON.stringify({
                    fileData: base64Data,
                    fileName: fileName,
                    fileExtension: fileExtension
                }));
            };

            reader.onerror = () => {
                console.error("Failed to read file.");
                UnityInstance.SendMessage(objectName, "OnUploadFailed", "Failed to read file.");
            };

            reader.readAsDataURL(file); // 將文件讀取為 Base64 URL
        };

        input.click();
    }
});
