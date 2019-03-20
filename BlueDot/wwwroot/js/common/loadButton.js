/*!
 * Show loading spinner for Ajax request
 */
var laddaButton;
function startLadda(parent) {
    var btn = $(parent).find('.ladda-button');
    if (btn.length > 0) {
        laddaButton = Ladda.create(btn[0]);
        laddaButton.start();
    }
}

// TODO: Figure out why this function has to be called completed in order for the redirect to work on the password page.
var completed = function () {
    if (laddaButton) {
        laddaButton.stop();
    }
};