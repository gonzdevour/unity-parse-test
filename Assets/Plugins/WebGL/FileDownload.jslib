mergeInto(LibraryManager.library, {
    WebGLDownloadFile: function (url, fileName) {
        var link = document.createElement('a');
        link.href = UTF8ToString(url);
        link.download = UTF8ToString(fileName);
        link.click();
    }
});