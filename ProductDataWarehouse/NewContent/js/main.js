function getSiteUrl() {
	var hostName = window.location.hostname.toLowerCase();

	if (hostName.indexOf('paoli-test01') >= 0)
		return 'http://paoli-test01';
	if (hostName.indexOf('jamesburnes') >= 0)
		return 'http://paoli.getvitaminj.com';
	if (hostName.indexOf('getvitaminj') >= 0)
		return 'http://paoli.getvitaminj.com';
	if (hostName.indexOf('matt3400') >= 0)
		return 'http://matt3400.wdd.local:8787';
	if (hostName.indexOf('localhost') >= 0)
		return 'http://localhost:8787';
	return 'http://www.paoli.com';
}

function getLocalDate(datetime) {
	if (datetime == 0) {
		return '';
	}
	var cDate = new Date(datetime);
	var offset = new Date().getTimezoneOffset();
	cDate.setMinutes(cDate.getMinutes() - offset);
	return (cDate.getMonth() + 1) + '/' + cDate.getDate() + '/' + cDate.getFullYear();
}

function getLocalDateTime(datetime) {
	if (datetime == 0) {
		return '';
	}
	var cDate = new Date(datetime);
	var offset = new Date().getTimezoneOffset();
	cDate.setMinutes(cDate.getMinutes() - offset);
	// 1/2/14 at 8:35 a.m.
	return (cDate.getMonth() + 1) + '/' + cDate.getDate() + '/' + cDate.getFullYear() + ' at ' +
			(((cDate.getHours() % 12) == 0) ? '12' : (cDate.getHours() % 12)) + ':' +
			((cDate.getMinutes() <= 9) ? '0' : '') + cDate.getMinutes() +
			((cDate.getHours() >= 12) ? ' p.m.' : ' a.m.');
}

// this will auto convert the date/time to local - somehow...
function getDateTimeFromMilliseconds(datetime) {
	if (datetime == 0) {
		return '';
	}
	var cDate = new Date(datetime);
	// 1/2/14 at 8:35 a.m.
	return (cDate.getMonth() + 1) + '/' + cDate.getDate() + '/' + cDate.getFullYear() + ' ' +
			(((cDate.getHours() % 12) == 0) ? '12' : (cDate.getHours() % 12)) + ':' +
			((cDate.getMinutes() <= 9) ? '0' : '') + cDate.getMinutes() +
			((cDate.getHours() >= 12) ? ' p.m.' : ' a.m.');
}

$(document).ready(function () {
	$.ajaxSetup({
		// Disable caching of AJAX responses
		cache: false
	});

	$('form').bind('submit', function (e) {
		if ($(this).data('submitted') === true) {
			// Previously submitted - don't submit again
			e.preventDefault();
		} else if ($(this).valid()) {
			// Mark it so that the next submit can be ignored
			$(this).data('submitted', true);
			//$('#inProgress').show();
			if (!$(this).hasClass('noSubmitProgress')) {
				$('#btnFormSubmissionInProgress').click();
			}
		}
	});

	$("#btnFormSubmissionInProgress").fancybox({
		/*				'autoDimensions': false,
		'width': 450,
		'height': 600,*/
		'padding': 20,
		'margin': 0,
		'scrolling': 'auto',
		'titleShow': false,
		'hideOnOverlayClick': false,
		'hideOnContentClick': false,
		'showCloseButton': false,
		'onStart': function () {

		}
	});

	var cDate = new Date();
	if (cDate.getHours() < 12) {
		$('#welcomeHeading').prepend('Good morning,');
	} else if (cDate.getHours() < 17) {
		$('#welcomeHeading').prepend('Good afternoon,');
	} else {
		$('#welcomeHeading').prepend('Good evening,');
	}
});

function makeAjaxCall(url, inputData, success, error) {
	$.ajax({
		dataType: 'json',
		type: 'POST',
		url: url,
		data: inputData,
		success: function (data, textStatus, jqXHR) {
			if (success != null) {
				success(data, textStatus, jqXHR);
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			if (jqXHR.status == 403) {
				window.location = '/Import/Logon?ReturnURL=' + window.location.pathname;
				return;
			}
			if (error != null) {
				error(jqXHR, textStatus, errorThrown);
			}
		}
	})
}