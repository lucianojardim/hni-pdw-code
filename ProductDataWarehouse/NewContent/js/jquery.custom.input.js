//##############################
// jQuery Custom Radio-buttons and Checkbox; basically it's styling/theming for Checkbox and Radiobutton elements in forms
// By Dharmavirsinh Jhala - dharmavir@gmail.com
// Date of Release: 13th March 10
// Version: 0.8
/*
 USAGE:
	$(document).ready(function(){
		$(":radio").behaveLikeCheckbox();
	}
*/

var elmHeight = "12";	// should be specified based on image size

// Extend JQuery Functionality For Custom Radio Button Functionality
jQuery.fn.extend({
	dgStyle: function () {
		// Initialize with initial load time control state
		$.each($(this), function () {
			var elm = $(this).children().get(0);
			elmType = $(elm).attr("type");
			$(this).data('type', elmType);
			$(this).data('checked', $(elm).attr("checked"));
			$(this).data('disabled', $(elm).attr("disabled"));
			$(this).dgClear();
		});
		$(this).mousedown(function () { $(this).dgEffect(); });
		$(this).mouseup(function () { $(this).dgHandle(); });
	},
	dgClear: function () {
		if ($(this).data("checked") == "checked") {
			$(this).css("backgroundPosition", "left -" + (elmHeight * 2) + "px");
		} else {
			$(this).css("backgroundPosition", "left 0");
		}
	},
	dgEffect: function () {
		if (!($(this).data("disabled") == true)) {
			if ($(this).data("checked") == "checked")
				$(this).css({ backgroundPosition: "left -" + (elmHeight * 2) + "px" });
			else
				$(this).css({ backgroundPosition: "left -" + (elmHeight * 2) + "px" });
		}
	},
	dgHandle: function () {
		if (!($(this).data("disabled") == true)) {
			var elm = $(this).children().get(0);
			if ($(this).data("checked") != "checked")
				$(elm).dgCheck(this);
			else if ($(this).data('type') != 'radio')
				$(elm).dgUncheck(this);

			if ($(this).data('type') == 'radio') {
				$.each($("input[name='" + $(elm).attr("name") + "']"), function () {
					if (elm != this)
						$(this).dgUncheck(-1);
				});
			}
		}
	},
	dgCheck: function (div) {
		$(this).attr("checked", "checked");
		$(div).data('checked', "checked").css({ backgroundPosition: "left -" + (elmHeight * 2) + "px" });
	},
	dgUncheck: function (div) {
		$(this).attr("checked", false);
		if (div != -1)
			$(div).data('checked', false).css({ backgroundPosition: "left 0" });
		else
			$(this).parent().data("checked", false).css({ backgroundPosition: "left 0" });
	}
});	