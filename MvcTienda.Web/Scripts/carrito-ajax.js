$(document).ready(function () {
    const API_VALIDAR = 'https://localhost:44349/api/carrito/validar-item';

    // 1. Ejecutar al cargar la página para mostrar qué hay en el carrito actualmente
    actualizarInfoStockVisual();

    $(document).off('submit', 'form[action*="ShoppingCart/AddItem"]').on('submit', 'form[action*="ShoppingCart/AddItem"]', function (e) {
        e.preventDefault();

        const $form = $(this);
        const $btn = $form.find('button[type="submit"]');
        const originalHtml = $btn.html();

        const idProducto = parseInt($form.find('input[name="idProducto"]').val());
        const cantidadNueva = parseInt($form.find('input[name="cantidad"]').val() || 1);

        // UI: Estado de carga
        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span>');

        // PASO 1: Consultar al servidor MVC cuántos tiene YA de este producto específico
        $.get('/ShoppingCart/GetQuantityInCart', { idProducto: idProducto }, function (cantidadPrevia) {

            const dataValidacion = {
                idProducto: idProducto,
                cantidad: cantidadNueva + (cantidadPrevia || 0)
            };

            // PASO 2: Validar contra la API de Inventario
            $.ajax({
                url: API_VALIDAR,
                type: 'POST',
                data: JSON.stringify(dataValidacion),
                contentType: 'application/json',
                success: function (res) {
                    if (res.success) {
                        // PASO 3: Guardar en la sesión MVC
                        $.post($form.attr('action'), $form.serialize(), function (mvcRes) {
                            if (mvcRes.success) {
                                // Actualizar el badge
                                $('#cart-count').text(mvcRes.count).hide().fadeIn();

                                // Notificación exitosa
                                Swal.fire({
                                    icon: 'success',
                                    title: '¡Añadido!',
                                    text: 'Producto agregado correctamente',
                                    toast: true,
                                    position: 'top-end',
                                    showConfirmButton: false,
                                    timer: 3000
                                });

                                // ACTUALIZAR LA VISTA VISUAL DEL STOCK
                                actualizarInfoStockVisual();
                            }
                        }).always(function () {
                            restaurarBoton($btn, originalHtml);
                        });
                    } else {
                        // MEJORA: Alerta de stock más detallada
                        Swal.fire({
                            title: 'Límite de Stock',
                            html: `<div class="text-start">
                                    <p>${res.message}</p>
                                    <small class="text-muted">Ya tienes ${cantidadPrevia} unidades en tu carrito.</small>
                                   </div>`,
                            icon: 'warning'
                        });
                        restaurarBoton($btn, originalHtml);
                    }
                },
                error: function () {
                    Swal.fire('Error', 'La API de inventario no responde', 'error');
                    restaurarBoton($btn, originalHtml);
                }
            });
        });
    });

    // FUNCIÓN PARA ACTUALIZAR LOS TEXTOS DE "Ya tienes X en el carrito"
    function actualizarInfoStockVisual() {
        $('.ajax-cart-form').each(function () {
            const $form = $(this);
            const id = $form.find('input[name="idProducto"]').val();
            const $infoSpan = $(`#stock-info-${id}`);
            const stockTotal = parseInt($(`#stock-total-${id}`).text());

            if (id) {
                $.get('/ShoppingCart/GetQuantityInCart', { idProducto: id }, function (enCarrito) {
                    if (enCarrito > 0) {
                        $infoSpan.html(`<i class="bi bi-cart-check"></i> Ya tienes ${enCarrito} en el carrito`);

                        // Si alcanzó el máximo, destacar en rojo
                        if (enCarrito >= stockTotal) {
                            $infoSpan.removeClass('text-info').addClass('text-danger');
                        } else {
                            $infoSpan.removeClass('text-danger').addClass('text-info');
                        }
                    } else {
                        $infoSpan.empty(); // Limpiar si no hay nada
                    }
                });
            }
        });
    }

    function restaurarBoton($btn, html) {
        $btn.prop('disabled', false).html(html);
    }
});