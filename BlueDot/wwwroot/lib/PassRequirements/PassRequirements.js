if (typeof jQuery === 'undefined') {
    throw new Error('PassRequirements requires jQuery')
}

+(function ($) {

    $.fn.PassRequirements = function (options) {
        /*
         * TODO
         * ====
         * 
         * store regexes in variables so they can be used by users through string 
         * specifications,ex 'number', 'special', etc
         * 
         */

        var container;

        if (options && options.hasOwnProperty("containerId")) {
            container = options.containerId;
        }
        else {
            container = "body";
        }

        var defaults = {
            //            defaults: true
        };

        if (
            !options ||                     //if no options are passed                                  /*
            options.defaults == true ||     //if default option is passed with defaults set to true      * Extend options with default ones
            options.defaults == undefined   //if options are passed but defaults is not passed           */
        ) {
            if (!options) {                   //if no options are passed, 
                options = {};               //create an options object
            }
            defaults.rules = $.extend({
                minlength: {
                    text: "be at least minLength characters long",
                    minLength: 8,
                },
                containSpecialChars: {
                    text: "Your input should contain at least minLength special character",
                    minLength: 1,
                    regex: new RegExp('([^!,%,&,@,#,$,^,*,?,_,~])', 'g')
                },
                containLowercase: {
                    text: "Your input should contain at least minLength lower case character",
                    minLength: 1,
                    regex: new RegExp('[^a-z]', 'g')
                },
                containUppercase: {
                    text: "Your input should contain at least minLength upper case character",
                    minLength: 1,
                    regex: new RegExp('[^A-Z]', 'g')
                },
                containNumbers: {
                    text: "Your input should contain at least minLength number",
                    minLength: 1,
                    regex: new RegExp('[^0-9]', 'g')
                }
            }, options.rules);
        } else {
            defaults = options;     //if options are passed with defaults === false
        }



        var i = 0;

        return this.each(function () {

            if (!defaults.defaults && !defaults.rules) {
                console.error('You must pass in your rules if defaults is set to false. Skipping this input with id:[' + this.id + '] with class:[' + this.classList + ']');
                return false;
            }

            var requirementList = "";
            $(this).data('pass-req-id', i++);
            var ok = 0;
            var popoverVisible = true;
            var field = $(this);
            $(this).keyup(function () {
                var this_ = $(this);

                Object.getOwnPropertyNames(defaults.rules).forEach(function (val, idx, array) {
                    if (this_.val().replace(defaults.rules[val].regex, "").length <= defaults.rules[val].minLength - 1) {
                        if (popoverVisible === false) {
                            $(container).find(".popover-header").css('background-color', '#f7f7f7');
                            $(container).find(".popover-header").css('color', 'rgb(103, 106, 108)');
                            popoverVisible = true;
                        }
                    }
                })
                if (popoverVisible === true) {
                    Object.getOwnPropertyNames(defaults.rules).forEach(function (val, idx, array) {
                        if (this_.val().replace(defaults.rules[val].regex, "").length > defaults.rules[val].minLength - 1) {
                            $(container).find(".popover").find('#' + val).css('text-decoration', 'line-through');
                            ok++;
                        } else {
                            if (popoverVisible == false) {
                                $(field).popover('show');
                                popoverVisible = true;
                            }
                            $(container).find(".popover").find('#' + val).css('text-decoration', 'none');
                            ok = 0;
                        }
                    })
                }

                if (ok >= 5) {
                    popoverVisible = false;
                    $(container).find(".popover-header").css('background-color', '#18a689');
                    $(container).find(".popover-header").css('color', '#ffffff');
                }
            });

            Object.getOwnPropertyNames(defaults.rules).forEach(function (val, idx, array) {
                requirementList += (("<li id='" + val + "'>" + defaults.rules[val].text).replace("minLength", defaults.rules[val].minLength));
            })
            try {
                $(this).popover({
                    title: 'Requisitos',
                    trigger: options.trigger ? options.trigger : 'focus',
                    html: true,
                    placement: options.popoverPlacement ? options.popoverPlacement : 'bottom',
                    content: 'Your password should:<ul>' + requirementList + '</ul>',
                    //      + '<p>The confirm field is actived only if all criteria are met</p>'
                    container: container
                });
            } catch (e) {
                throw new Error('PassRequirements requires Bootstraps Popover plugin');
            }
            $(this).focus(function () {
                $(this).keyup();
            });
        });
    };

}(jQuery));
