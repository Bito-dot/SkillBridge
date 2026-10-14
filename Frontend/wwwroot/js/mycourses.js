
document.addEventListener("DOMContentLoaded", function () {
    const dialog = document.getElementById("czCourseImageDialog");
    const preview = document.getElementById("czCourseDialogImage");

    if (!dialog || !preview) return;

    const closeButton = dialog.querySelector(".cz-course-dialog-close");

    document.querySelectorAll(".cz-mine-image-link").forEach(function (button) {
        button.addEventListener("click", function () {
            const thumbnail = button.querySelector("img");

            if (!thumbnail) return;

            preview.src = thumbnail.currentSrc || thumbnail.src;
            preview.alt = thumbnail.alt;

            if (!dialog.open) {
                dialog.showModal();
            }
        });
    });

    if (closeButton) {
        closeButton.addEventListener("click", function () {
            dialog.close();
        });
    }

    dialog.addEventListener("click", function (event) {
        if (event.target === dialog) {
            dialog.close();
        }
    });

    dialog.addEventListener("close", function () {
        preview.removeAttribute("src");
        preview.alt = "";
    });
});

// Confirm before submitting; the server always restricts deletion to the signed-in member.
document.querySelectorAll('.cz-mine-remove-form').forEach(form => {
    form.addEventListener('submit', event => {
        if (!window.confirm('Remove this course from My Courses? Your learning history will be kept.')) event.preventDefault();
    });
});
