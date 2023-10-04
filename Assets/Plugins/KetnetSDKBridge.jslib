var KetnetSDKBridge = {
    SaveState: function (stateId, value) {
        // Convert the pointer strings back to normal strings.
        var stateIdStr = Pointer_stringify(stateId);
        var valueStr = Pointer_stringify(value);

        // Check for SDK instance and save state.
        if (typeof KetnetSDKBridge.sdkInstance !== 'undefined' && KetnetSDKBridge.sdkInstance.state) {
            KetnetSDKBridge.sdkInstance.state.saveState(stateIdStr, null, valueStr, null)
                .then(function (isSaved) {
                    console.log('Data saved: ' + isSaved);
                })
                .catch(function (error) {
                    console.error('Save Error: ', error);
                });
        } else {
            console.error('SDK instance is not available.');
        }
    },

    LoadState: function (stateId, callbackPtr) {
        var stateIdStr = Pointer_stringify(stateId);
        
        // Check for SDK instance and load state.
        if (typeof KetnetSDKBridge.sdkInstance !== 'undefined' && KetnetSDKBridge.sdkInstance.state) {
            KetnetSDKBridge.sdkInstance.state.loadState(stateIdStr, null, false)
                .then(function (loadedData) {
                    // Callback to Unity with the loaded data.
                    Runtime.dynCall('vi', callbackPtr, [stateId, loadedData]);
                })
                .catch(function (error) {
                    console.error('Load Error: ', error);
                });
        } else {
            console.error('SDK instance is not available.');
        }
    },

    InitializeSDK: function (eventHandlerPtr) {
        // Check for the existence of createSDK and ketnetSDK.
        if (typeof ketnetSDK !== 'undefined' && typeof ketnetSDK.createSDK === 'function') {
            // Initialize the SDK and handle events.
            KetnetSDKBridge.sdkInstance = ketnetSDK.createSDK(function(event) {
                // Convert string event type to a pointer for Unity.
                var eventTypePtr = allocate(intArrayFromString(event.type), 'i8', ALLOC_STACK);
                // Event detail values might need to be handled differently based on the event type.
                var eventDetail = event.detail || {};
                var eventDetailBool = eventDetail.initialized || eventDetail.loggedIn || false;
                // Callback to Unity with the event data.
                Runtime.dynCall('vii', eventHandlerPtr, [eventTypePtr, eventDetailBool]);
            });
        } else {
            console.error('ketnetSDK or createSDK method is not defined.');
        }
    }
};

mergeInto(LibraryManager.library, KetnetSDKBridge);