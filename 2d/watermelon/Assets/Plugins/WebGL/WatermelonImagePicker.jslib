mergeInto(LibraryManager.library, {
  WatermelonPickFruitImage: function (tier, targetObjectPtr, callbackMethodPtr) {
    var targetObject = UTF8ToString(targetObjectPtr);
    var callbackMethod = UTF8ToString(callbackMethodPtr);
    var input = document.createElement("input");
    input.type = "file";
    input.accept = "image/png,image/jpeg";
    input.style.display = "none";

    input.onchange = function () {
      var file = input.files && input.files[0];
      if (!file) {
        document.body.removeChild(input);
        return;
      }

      var reader = new FileReader();
      reader.onload = function () {
        var result = reader.result || "";
        var comma = result.indexOf(",");
        var base64 = comma >= 0 ? result.substring(comma + 1) : result;
        SendMessage(targetObject, callbackMethod, tier + "|" + base64);
        document.body.removeChild(input);
      };

      reader.readAsDataURL(file);
    };

    document.body.appendChild(input);
    input.click();
  }
});
