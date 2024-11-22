mergeInto(LibraryManager.library, {
    GetQueryStringValue: function(paramNamePtr) {
        // 將指針轉換為字符串
        var paramName = UTF8ToString(paramNamePtr);

        // 獲取網址上的 Query String
        var params = new URLSearchParams(window.location.search);

        // 返回指定參數的值（如果不存在，返回空字符串）
        var value = params.get(paramName) || "";

        // 返回 UTF8 編碼的字符串指針
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    }
});