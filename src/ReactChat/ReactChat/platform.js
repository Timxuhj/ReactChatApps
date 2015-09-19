/* web */
document.documentElement.className += ' web';
window.nativeHost = {
    quit: function() {
        window.close();
    },
    showAbout: function() {
        alert('ReactChat - ServiceStack + ReactJS');
    },
    ready: function() {
        //
    },
    platform: 'web'
};
