// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Delete confirmation modal handling
$(function () {
    var deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));

    $(document).on('click', '.btn-delete', function (e) {
        var id = $(this).data('id');
        var description = $(this).data('description');
        $('#deleteId').val(id);
        $('#deleteModalBody').text("Are you sure you want to delete: '" + description + "'?");
        deleteModal.show();
    });
});

// AJAX delete handler: intercept form submit and call DeleteAjax endpoint
$(function () {
    $('#deleteForm').on('submit', function (e) {
        e.preventDefault();
        var id = $('#deleteId').val();
        var token = $('input[name="__RequestVerificationToken"]', this).val();

        $.ajax({
            url: '/MaintenanceRequests/DeleteAjax/' + id,
            method: 'POST',
            headers: { 'RequestVerificationToken': token, 'X-Requested-With': 'XMLHttpRequest' },
            success: function (res) {
                if (res && res.success) {
                    // Remove the table row
                    $('tr[data-id="' + res.id + '"]').fadeOut(300, function () { $(this).remove(); });
                }
                // Hide modal
                var modalEl = document.getElementById('deleteModal');
                var modal = bootstrap.Modal.getInstance(modalEl);
                modal.hide();
            },
            error: function () {
                alert('Delete failed.');
            }
        });
    });
});
