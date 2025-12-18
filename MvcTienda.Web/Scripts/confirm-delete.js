document.addEventListener("DOMContentLoaded", function () {
    // Usamos delegación de eventos por si agregas elementos dinámicamente
    document.body.addEventListener('click', function (e) {
        // Buscamos si el clic fue en el botón o en el ícono dentro del botón
        const button = e.target.closest('.btn-confirm-delete');

        if (button) {
            e.preventDefault(); // Detenemos cualquier acción por defecto

            const form = button.closest('form');
            const nombre = button.getAttribute('data-nombre') || "este elemento";
            const tipo = button.getAttribute('data-tipo') || "el registro";
            const mensajePersonalizado = button.getAttribute('data-mensaje');

            Swal.fire({
                title: '¿Confirmar acción?',
                text: mensajePersonalizado || `¿Estás seguro de que deseas eliminar ${tipo}: "${nombre}"?`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#6c757d',
                confirmButtonText: '<i class="bi bi-trash me-2"></i>Sí, proceder',
                cancelButtonText: 'Cancelar',
                reverseButtons: true,
                customClass: {
                    confirmButton: 'btn btn-danger px-4 mx-2 rounded-pill',
                    cancelButton: 'btn btn-light px-4 mx-2 rounded-pill'
                },
                buttonsStyling: false
            }).then((result) => {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
        }
    });
});