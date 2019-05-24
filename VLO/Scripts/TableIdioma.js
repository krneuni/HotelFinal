
$(document).ready(function () {
    var espanol =
    {
    "sProcessing": "Procesando...",
"sLengthMenu": "Cantidad de registros _MENU_",
"sZeroRecords": "No se encontraron resultados",
"sEmptyTable": "Ningún dato disponible en esta tabla",
"sInfo": "Registros del _START_ al _END_ de un total de _TOTAL_ registros",
"sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
"sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
"sInfoPostFix": "",
"sSearch": "Buscar:",
"sUrl": "",
"sInfoThousands": ",",
"sLoadingRecords": "Cargando...",
        "oPaginate": {
    "sFirst": "Primero",
"sLast": "Último",
"sNext": "Siguiente",
"sPrevious": "Anterior"
},
        "oAria": {
    "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
"sSortDescending": ": Activar para ordenar la columna de manera descendente"
}

};

    $('#dtTable').DataTable({
    responsive: true,
language: espanol,
        "dom": '<"dt-buttons"Bf><"clear">lirtp',
"paging": true,
"autoWidth": true,
"buttons": [
    'colvis',
    'copyHtml5',
    'csvHtml5',
    'excelHtml5',
    'pdfHtml5',
    'print'
]
});


});