$(document).ready(function () {
    $("#showPassword").click(function () {
        var passwordInput = $("#Password");
        if (passwordInput.attr("type") === "password") {
            passwordInput.attr("type", "text");
            $(this).text("Hide Password");
        } else {
            passwordInput.attr("type", "password");
            $(this).text("Show Password");
        }
    });
});
