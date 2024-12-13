mergeInto(LibraryManager.library, {
    UploadFileWebGL: function (uploadUrlPtr, objectNamePtr) {
        const uploadUrl = UTF8ToString(uploadUrlPtr);
        const objectName = UTF8ToString(objectNamePtr);

        const input = document.createElement('input');
        input.type = 'file';
        input.onchange = () => {
            const file = input.files[0];
            if (!file) {
                console.error("No file selected.");
                return;
            }

            const formData = new FormData();
            formData.append("file", file, file.name);

            fetch(uploadUrl, {
                method: 'POST',
                body: formData,
            })
                .then(response => response.text())
                .then(result => {
                    console.log("File uploaded successfully:", result);
                    UnityInstance.SendMessage(objectName, "OnUploadComplete", result);
                })
                .catch(error => {
                    console.error("File upload failed:", error);
                    UnityInstance.SendMessage(objectName, "OnUploadFailed", error.message);
                });
        };

        input.click();
    }
});