function doAjaxSubmit(action, params, callback) {
    debugger
    $.ajax({
        type: 'POST',
        url: action,
        data: params,
        cache: false,
        //contentType:'application/json',
        dataType: 'json',
        success: function (json) {
            if (json.result == 0) {
                callback.call(this, json, action, true);
            } else {
                callback.call(this, json, action, false);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            try {
                if (p.onError) p.onError(XMLHttpRequest, textStatus, errorThrown);
            } catch (e) {
            }
            callback.call(this, null, action, false);
        }
    });
}