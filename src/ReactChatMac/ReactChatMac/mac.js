window.nativeHost = {
    quit: function () {
        $.get('/nativehost/quit');
        setTimeout(function() {
            alert('foooo');
        });
    },
    showAbout: function () {
    	$.get('/nativehost/showAbout', function (response) { 
    		window.location = "http://www.google.com";
    	});
    },
    toggleFormBorder: function () {
        //
    },
    dockLeft: function () {
        //
    },
    dockRight: function () {
        //
    },
    ready: function () {
        //
    },
    platform: 'mac'
}
