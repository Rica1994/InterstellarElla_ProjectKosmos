mergeInto(LibraryManager.library, {
  // Function to set up the visibility change event listener
  RegisterVisibilityChangeCallback: function(callback) {
    var callbackFunc = Module.cwrap('OnVisibilityChanged', 'void', ['int']);

    function visibilityChanged() {
      if (document.visibilityState === 'visible') {
        callbackFunc(1); // The tab is active
      } else {
        callbackFunc(0); // The tab is not active
      }
    }

    document.addEventListener('visibilitychange', visibilityChanged);
  },
});
