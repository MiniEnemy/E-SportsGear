var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({ // <- this was the issue
        "ajax": {
            url: '/admin/Product/getall'
        },
        "columns": [
            { data: 'productName', "width": "18%" },
            { data: 'companyName', "width": "18%" },
            { data: 'description', "width": "22%" },
            { data: 'price', "width": "10%" },
            { data: 'category.name', "width": "12%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="text-center d-flex justify-content-center gap-2">
                            <a href="/admin/product/upsert?id=${data}" class="btn btn-sm btn-success text-white">
                                <i class="fas fa-edit"></i> Edit
                            </a>
                            <a onClick="Delete('/admin/product/delete/${data}')" class="btn btn-sm btn-danger text-white">
                                <i class="fas fa-trash-alt"></i> Delete
                            </a>
                        </div>`;
                },
                "width": "20%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload(); // now it works!
                    toastr.success(data.message);
                }
            });
        }
    });
}
