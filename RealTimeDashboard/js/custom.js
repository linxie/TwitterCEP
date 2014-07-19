$(document).ready(function () {
    $("#orderedlist").find("li").each(function (i) {
        $(this).append(" BAM! " + i);
    });
});
