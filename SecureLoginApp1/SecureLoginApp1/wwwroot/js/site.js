// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    // Auto-dismiss the toast-style TempData status message.
    var toast = document.getElementById("statusToast");
    if (toast) {
        setTimeout(function () {
            toast.classList.add("toast-status--hide");
            setTimeout(function () { toast.remove(); }, 300);
        }, 5000);
    }

    // Show a loading/disabled state on the submit button of any form that passes validation.
    document.querySelectorAll("form").forEach(function (form) {
        form.addEventListener("submit", function () {
            if (form.checkValidity && !form.checkValidity()) {
                return;
            }
            var submitBtn = form.querySelector("button[type='submit']");
            if (submitBtn) {
                submitBtn.classList.add("is-loading");
                submitBtn.disabled = true;
            }
        });
    });
});
