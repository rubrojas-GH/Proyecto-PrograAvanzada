$(document).ready(function () {

    $.ajax({
        url: 'https://localhost:44349/api/dashboard',
        method: 'GET',
        success: function (data) {

            // KPIs
            $('#totalUsuarios').text(data.totalUsuarios);
            $('#usuariosActivos').text(data.usuariosActivos);
            $('#usuariosInactivos').text(data.usuariosInactivos);
            $('#totalProductos').text(data.totalProductos);
            $('#productosBajoInventario').text(data.productosBajoInventario);

            $('#ventasUltimaSemana').text('₡ ' + data.ventasUltimaSemana.toLocaleString());

            let promedio = data.ventasUltimaSemana / 7;
            $('#promedioVentas').text('₡ ' + promedio.toLocaleString());

            // ====== GRÁFICO USUARIOS ======
            new Chart(document.getElementById('usuariosChart'), {
                type: 'doughnut',
                data: {
                    labels: ['Activos', 'Inactivos'],
                    datasets: [{
                        data: [data.usuariosActivos, data.usuariosInactivos],
                        backgroundColor: ['#28a745', '#dc3545']
                    }]
                }
            });

            // ====== GRÁFICO VENTAS (SIMULADO) ======
            new Chart(document.getElementById('ventasChart'), {
                type: 'bar',
                data: {
                    labels: ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom'],
                    datasets: [{
                        label: 'Ventas (₡)',
                        data: [
                            promedio * 0.8,
                            promedio * 1.1,
                            promedio * 0.9,
                            promedio * 1.3,
                            promedio,
                            promedio * 1.5,
                            promedio * 1.2
                        ],
                        backgroundColor: '#007bff'
                    }]
                }
            });

        },
        error: function () {
            alert('Error cargando estadísticas del dashboard');
        }
    });

});
