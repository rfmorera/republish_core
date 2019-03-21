/*!
 * Toggle Password Visibility
 */
$(document).ready(initializeHideShowPassword);
function initializeHideShowPassword() {
    $('.form-group').find('.form-control').each(function (index, input) {
        var $input = $(input);
        $input.parent().find('.glyphicon').click(function () {
            $input.parent().find(':password').focus();

            if ($(input).prop('type') === "password") {
                $(input).prop('type', 'text');
                $(this).text("Ocultar");
            }
            else {
                $(input).prop('type', 'password');
                $(this).text("Mostrar");
            }
        });
    });
}