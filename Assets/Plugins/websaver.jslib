mergeInto(LibraryManager.library, {

  DoSetString: function (key, str) {
    localStorage.setItem(UTF8ToString(key), UTF8ToString(str));
  },

  DoGetString: function (key, defaultValue = null) {
    var str = localStorage.getItem(UTF8ToString(key));
    if(!str) str = UTF8ToString(defaultValue);
    var bufferSize = lengthBytesUTF8(str) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(str, buffer, bufferSize);
    return buffer;
  },
  
  DoSetInt: function (key, value) {
    localStorage.setItem(UTF8ToString(key), value);
  },

  DoGetInt: function (key, defaultValue = null) {
    var value = localStorage.getItem(UTF8ToString(key));
    return value !== null ? value : defaultValue;
  },
  
  DoHasKey: function (key) {
    return localStorage.getItem(UTF8ToString(key)) !== null;
  },

});