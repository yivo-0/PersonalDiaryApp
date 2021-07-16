// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.



jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                $('#myModal').modal('hide');
                getFilteredData((parseInt($("#current-page").text(), 10) - 1), 5);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

jQueryAjaxPostAccount = form => {
    console.log(new FormData(form));
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                $('#myModal').modal('hide');

            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}






