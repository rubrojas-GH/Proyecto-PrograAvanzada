$(document).ready(function () {
    // Sincronizamos con el puerto 44349 que usa el dashboard y es el correcto de la API
    const API_URL = (typeof API_BASE_URL !== 'undefined') ? API_BASE_URL : 'https://localhost:44349/api/resenas';

    // Acción: APROBAR (Delegación de eventos)
    $(document).on('click', '.btn-approve', function () {
        const $btn = $(this);
        const id = $btn.data('id');
        const $row = $btn.closest('tr');

        // Deshabilitar botón para evitar doble clic
        $btn.prop('disabled', true);

        $.ajax({
            // Usamos concatenación simple para asegurar compatibilidad si API_URL termina en /
            url: API_URL + '/approve/' + id,
            type: 'PUT',
            success: function (response) {
                $row.addClass('table-success');
                $row.fadeOut(400, function () {
                    $(this).remove();
                    verificarTablaVacia();
                });
            },
            error: function (xhr) {
                $btn.prop('disabled', false);
                console.error("Error en API:", xhr);
                alert('Error al intentar aprobar la reseña. Verifique que la API en el puerto 44349 esté iniciada.');
            }
        });
    });

    // Acción: RECHAZAR
    $(document).on('click', '.btn-reject', function () {
        const id = $(this).data('id');
        const $row = $(this).closest('tr');
        const $btn = $(this);

        if (!confirm('¿Está seguro de eliminar esta reseña?')) return;

        $btn.prop('disabled', true);

        $.ajax({
            url: API_URL + '/reject/' + id,
            type: 'DELETE',
            success: function (response) {
                $row.addClass('table-danger');
                $row.fadeOut(400, function () {
                    $(this).remove();
                    verificarTablaVacia();
                });
            },
            error: function (xhr) {
                $btn.prop('disabled', false);
                console.error("Error en API:", xhr);
                alert('Error al intentar rechazar la reseña.');
            }
        });
    });

    function verificarTablaVacia() {
        const restantes = $('tbody tr').length;

        // Actualizamos el badge si existe en el layout
        const $badge = $('#badge-count');
        if ($badge.length) {
            $badge.text(restantes);
        }

        if (restantes === 0) {
            setTimeout(function () {
                location.reload();
            }, 500);
        }
    }
});