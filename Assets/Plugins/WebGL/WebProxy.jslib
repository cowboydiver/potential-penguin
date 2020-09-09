
mergeInto(LibraryManager.library, {
	_Post: function(url, params, key, callback_id = -1)
	{
		var request = new XMLHttpRequest();
		request.open("POST", Pointer_stringify(url), true);
		request.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
		request.setRequestHeader("api-key", Pointer_stringify(key));

		if (callback_id >= 0){
			request.onreadystatechange = function() {
				if (request.readyState != 4)
					return;
				SendMessage('WebProxy', 'OnCallback', JSON.parse({
					success: request.status == 200,
					data: request.responseText,
					id: callback_id,
					text: request.statusText
				}));
			}
		}

		request.send(Pointer_stringify(params));
	}
});
