$(document).ready(function () {
    var envId;
    $(".rightClick a").mouseover(function () {
        envId = $(this).attr("id");
    });


    context.init({
        preventDoubleContext: false,
        fadeSpeed: 200
    });

    context.attach('.rightClick', [
		{ header: "²Ù×÷" },
		{
		    text: "±à¼­", action: function (e) {
		        location.href = '/Env/Edit?id=' + envId;
		    }
		}
    ]);

    context.settings({ compress: true });

    $(document).on('mouseover', '.me-codesta', function () {
        $('.finale h1:first').css({ opacity: 0 });
        $('.finale h1:last').css({ opacity: 1 });
    });

    $(document).on('mouseout', '.me-codesta', function () {
        $('.finale h1:last').css({ opacity: 0 });
        $('.finale h1:first').css({ opacity: 1 });
    });

});