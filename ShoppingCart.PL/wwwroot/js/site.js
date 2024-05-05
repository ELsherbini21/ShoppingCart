$(document).ready(function () {
    $(".wish-icon i").click(function () {
        $(this).toggleClass("fa-heart fa-heart-o");
    });
});





$(document).ready(function () {

    dataTable = $('#productTable').DataTable({
        "ajax": { "url": "/Product/GetAllProducts" },
        "Columns": [
            { "data": "Name" },
            { "data": "Description" },
            { "data": "Price" },
            { "data": "Category" },

        ]
    })
});







