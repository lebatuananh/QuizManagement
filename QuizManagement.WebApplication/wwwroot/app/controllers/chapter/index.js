var ChapterController = function () {

    this.initialize = function () {
        loadData();
        registerEvents();
    };

    function registerEvents() {
        //Init validation
        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            lang: 'en',
            rules: {
                txtNameM: {required: true},
            }
        });

        $('#txt-search-keyword').keypress(function (e) {
            if (e.which === 13) {
                e.preventDefault();
                core.configs.pageIndex = 1;
                loadData();
                $('#paginationUL').twbsPagination('destroy');
            }
        });

        $("#btn-search").on('click', function () {
            core.configs.pageIndex = 1;
            loadData();
            $('#paginationUL').twbsPagination('destroy');
        });

        $("#ddl-show-page").on('change', function () {
            core.configs.pageSize = $(this).val();
            core.configs.pageIndex = 1;
            loadData(true);
        });

        $("#btn-create").on('click', function () {
            resetFormMaintainance();
            $('#modal-add-edit').modal('show');

        });

        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $.ajax({
                type: "GET",
                url: "/Admin/Chapter/GetById",
                data: {id: that},
                dataType: "json",
                beforeSend: function () {
                    core.startLoading();
                },
                success: function (response) {
                    var data = response;
                    $('#hidIdM').val(data.Id);
                    $('#txtNameM').val(data.Name);
                    $('#txtDescriptionM').val(data.Description);
                    $('#ckStatusM').prop('checked', data.Status === 1);
                    $('#modal-add-edit').modal('show');
                    core.stopLoading();

                },
                error: function () {
                    core.notify('Has an error', 'error');
                    core.stopLoading();
                }
            });
        });

        $('#btnSave').on('click', function (e) {

            if ($('#frmMaintainance').valid()) {
                e.preventDefault();
                var id = $('#hidIdM').val();
                var name = $('#txtNameM').val();
                var description= $('#txtDescriptionM').val();
                var status = $('#ckStatusM').prop('checked') === true ? 1 : 0;

                $.ajax({
                    type: "POST",
                    url: "/Admin/Chapter/SaveEntity",
                    data: {
                        Id: id,
                        Name: name,
                        Status: status,
                        Description:description
                    },
                    dataType: "json",
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function () {
                        core.notify('Save Chapter successful', 'success');
                        $('#modal-add-edit').modal('hide');
                        resetFormMaintainance();

                        loadData(true);
                        $('#paginationUL').twbsPagination('destroy');
                        core.stopLoading();

                    },
                    error: function () {
                        core.notify('Have an error in progress', 'error');
                        core.stopLoading();
                    }
                });
                return false;
            }
            return false;
        });

        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            core.confirm('Are you sure to delete?', function () {
                $.ajax({
                    type: "POST",
                    url: "/Admin/Chapter/Delete",
                    data: {id: that},
                    dataType: "json",
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function () {
                        core.notify('Delete Chapter successful', 'success');
                        core.stopLoading();
                        loadData();
                        $('#paginationUL').twbsPagination('destroy');

                    },
                    error: function () {
                        core.notify('Have an error in progress', 'error');
                        core.stopLoading();
                    }
                });
            });
        });

        $('#btnSelectImg').on('click', function () {
            $('#fileInputImage').click();
        });
    }

    function resetFormMaintainance() {
        $('#hidIdM').val(0);
        $('#txtNameM').val('');
        $('#ckStatusM').prop('checked', true);
    }

    function loadData(isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/Chapter/GetAllPaging",
            data: {
                keyword: $('#txt-search-keyword').val(),
                page: core.configs.pageIndex,
                pageSize: core.configs.pageSize
            },
            dataType: "json",
            beforeSend: function () {
                core.startLoading();
            },
            success: function (response) {
                var template = $('#table-template').html();
                var render = "";
                if (response.RowCount > 0) {
                    $.each(response.Results, function (i, item) {
                        render += Mustache.render(template, {
                            Name: item.Name,
                            CreatedDate: core.dateFormatSubStr(item.DateCreated),
                            ModifiedDate: core.dateFormatSubStr(item.DateModified),
                            Status: core.getStatus(item.Status),
                            Id: item.Id
                        });
                    });
                    $("#lbl-total-records").text(response.RowCount);
                    if (render != undefined) {
                        $('#tbl-content').html(render);

                    }
                    wrapPaging(response.RowCount, function () {
                        loadData();
                    }, isPageChanged);


                }
                else {
                    $('#tbl-content').html('');
                }
                core.stopLoading();
            },
            error: function (status) {
                console.log(status);
            }
        });
    }

    function wrapPaging(recordCount, callBack, changePageSize) {
        var totalsize = Math.ceil(recordCount / core.configs.pageSize);
        //Unbind pagination if it existed or click change pagesize
        if ($('#paginationUL a').length === 0 || changePageSize === true) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').unbind("page");
        }
        //Bind Pagination Event
        $('#paginationUL').twbsPagination({
            totalPages: totalsize,
            visiblePages: 7,
            first: 'First',
            prev: 'Prevous',
            next: 'Next',
            last: 'Last',
            onPageClick: function (event, p) {
                if (core.configs.pageIndex != p) {
                    core.configs.pageIndex = p;
                    setTimeout(callBack(), 200);
                }
            }
        });
    }
};