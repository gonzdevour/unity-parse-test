mergeInto(LibraryManager.library, {
  CheckUserAgent: function (keywordPtr) {
      var keyword = UTF8ToString(keywordPtr);
      return navigator.userAgent.includes(keyword) ? 1 : 0;
  }
});
