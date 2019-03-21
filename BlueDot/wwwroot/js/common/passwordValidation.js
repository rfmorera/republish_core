// TODO: Have the page script invoke this plugin to avoid defining 2 sets of data on each page using the plugin.
$(document).ready(initializeValidationPlugin);

function initializeValidationPlugin() {
    var passRequirements = commonPassRequirements;
    passRequirements.containerId = "#password-popover-container";

    $('#Input_Password').PassRequirements(passRequirements);

    var newPassRequirements = commonPassRequirements;
    newPassRequirements.containerId = "#new-password-popover-container";

    $('#New_Password').PassRequirements(newPassRequirements);

    // Move the popover to the bottom left of the input.
    $('#New_Password').on('inserted.bs.popover', function (e) {
        var popoverId = $(this).attr("aria-describedby");
        var popover = $("#" + popoverId);
        var offset = parseInt((popover.width() - $(this).width()) / 2, 10);
        popover.data()['bs.popover'].config.offset = offset + 'px';
    });
}

var commonPassRequirements = {
    rules: {
        containSpecialChars: {
            text: "La contraseña debe contener al menos un caracter especial",
            minLength: 1,
            regex: new RegExp('([^!,%,&,@,#,$,^,*,?,_,~])', 'g')
        },
        containLowercase: {
            text: "La contraseña debe contener al menos una letras minúsculas ('a'-'z'). ",
            minLength: 3,
            regex: new RegExp('[^a-z]', 'g')
        },
        containUppercase: {
            text: "La contraseña debe contener al menos una letra mayúscula('A'-'Z'). ",
            minLength: 1,
            regex: new RegExp('[^A-Z]', 'g')
        },
        containNumbers: {
            text: "La contraseña debe contener al menos un dígito ('0'-'9').",
            minLength: 2,
            regex: new RegExp('[^0-9]', 'g')
        },
        minlength: {
            text: "La contraseña debe contener al menos 6 caracteres.",
            minLength: 8
        }
    }
};
