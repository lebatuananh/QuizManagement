var QuestionController = function () {

    this.initialize = function () {

        loadData();
        loadCategoriesChapter();
        loadCategoriesSubject();
        registerEvents();

    };

    function registerEvents() {

        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            rules: {
                txtNameQuestion: {required: true},
                txtOptionA: {required: true},
                txtOptionB: {required: true},
                txtOptionC: {required: true},
                txtOptionD: {required: true},
                ddlCategorySubjectId: {required: true},
                ddlCategoryChapterId: {required: true},
                txtScore: {
                    number: true,
                    min: 0
                }
            }
        });

        //TODO: binding event to controll

        $('#ddlShowPage').on('change', function () {
            core.configs.pageSize = $(this).val();
            core.configs.pageIndex = 1;
            loadData(true);
        });

        $('#btnSearch').on('click', function () {
            core.configs.pageIndex = 1;
            loadData();
            $('#paginationUL').twbsPagination('destroy');
        });

        $('#txtKeyword').on('keypress', function (e) {
            if (e.which == 13) {
                e.preventDefault();
                core.configs.pageIndex = 1;
                loadData();
                $('#paginationUL').twbsPagination('destroy');
            }
        });

        $('#btnCreate').on('click', function (e) {
            e.preventDefault();
            $('#modal-add-edit').modal('show');
            resetFormMaintainance();
            $(".combo").css("width", "100%");
        });

        $('body').on('click', '.btnDelete', function (e) {
            e.preventDefault();

            var that = $(this).data('id');

            deleteItem(that);

        });

        $('#btnSave').on('click', function () {
            saveProduct();
        });

        $('body').on('click', '.btnEdit', function (e) {
            e.preventDefault();

            var that = $(this).data('id');

            loadDetails(that);
        });
    }


    function loadData(isPageChanged) {
        var template = $('#table-template').html();
        var render = '';
        $.ajax({
            type: 'GET',
            data: {
                chapterId: $('#ddlCategoryChapter').val(),
                subjectId: $('#ddlCategorySubject').val(),
                keyword: $('#txtKeyword').val(),
                page: core.configs.pageIndex,
                pageSize: core.configs.pageSize
            },
            url: '/admin/question/GetAllPaging',
            dataType: 'json',
            success: function (res) {
                if (res.Results.length > 0) {
                    $.each(res.Results, function (i, item) {
                        render += Mustache.render(template, {
                            Id: item.Id,
                            QuestionName: item.QuestionName,
                            // Chapter: item.Chapter.Name,
                            // Subject: item.Subject.Name,
                            ScoreQuestion: core.formatNumber(item.ScoreQuestion, 0),
                            CreatedDate: core.dateFormatSubStr(item.DateCreated),
                            Status: core.getStatus(item.Status)
                        });

                        $('#lblTotalRecords').text(res.RowCount);

                        if (render != '') {
                            $('#tbl-content').html(render);
                        }

                        wrapPaging(res.RowCount, function () {
                            loadData();
                        }, isPageChanged);

                    });
                } else {
                    $('#tbl-content').html('');
                    core.notify('Not found item', 'error');
                }
            },
            error: function (status) {
                console.log(status);
                core.notify('Cannot loading data', 'error');
            }
        });
    }

    function loadCategoriesChapter() {
        $.ajax({
            type: 'GET',
            url: '/admin/Chapter/GetAll',
            dataType: 'json',
            success: function (res) {
                var render = '<option>---Select Chapter---</option>';
                $.each(res, function (i, item) {
                    render += '<option value="' + item.Id + '">' + item.Name + '</option>';
                });
                $('#ddlCategoryChapter').html(render);
            },
            error: function (status) {
                console.log(status);
                core.notify('Cannot loading chapter category data', 'error');
            }
        });
    }

    function loadCategoriesSubject() {
        $.ajax({
            type: 'GET',
            url: '/admin/Subject/GetAll',
            dataType: 'json',
            success: function (res) {
                var render = '<option>---Select Subject---</option>';
                $.each(res, function (i, item) {
                    render += '<option value="' + item.Id + '">' + item.Name + '</option>';
                });
                $('#ddlCategorySubject').html(render);
            },
            error: function (status) {
                console.log(status);
                core.notify('Cannot loading subject category data', 'error');
            }
        });
    }

    function initTreeDropDownSubjectCategory(isSelected) {
        $.ajax({
            url: '/Admin/Subject/GetAll',
            type: 'GET',
            dataType: 'JSON',
            async: false,
            success: function (res) {
                var data = [];

                $.each(res, function (i, item) {

                    data.push({
                        id: item.Id,
                        text: item.Name,
                        parentId: null
                    });
                });
                var arr = core.unflattern(data);
                $('#ddlCategorySubjectId').combotree({
                    data: arr
                });
                $('#ddlCategoryIdImportExcel').combotree({
                    data: arr
                });
                if (isSelected != undefined) {
                    $('#ddlCategorySubjectId').combotree('setValue', isSelected);
                }
            }
        });
    }

    function initTreeDropDownChapterCategory(isSelected) {
        $.ajax({
            url: '/Admin/Chapter/GetAll',
            type: 'GET',
            dataType: 'JSON',
            async: false,
            success: function (res) {
                var data = [];

                $.each(res, function (i, item) {

                    data.push({
                        id: item.Id,
                        text: item.Name,
                        parentId: null
                    });
                });
                var arr = core.unflattern(data);
                $('#ddlCategoryChapterId').combotree({
                    data: arr
                });
                $('#ddlCategoryIdImportExcel').combotree({
                    data: arr
                });
                if (isSelected != undefined) {
                    $('#ddlCategoryChapterId').combotree('setValue', isSelected);
                }
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
            prev: 'Previous',
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

    function resetFormMaintainance() {

        $('#hidIdM').val(0);
        $('#txtNameQuestion').val('');
        initTreeDropDownSubjectCategory('');
        initTreeDropDownChapterCategory('');
        $('#txtOptionA').val('');
        $('#txtOptionB').val('');
        $('#txtOptionC').val('');
        $('#txtOptionD').val('');

        $('#txtAnswer').val('');
        $('#txtScore').val('');
        $('#ckStatusM').attr('checked', false);
        $(".combo").css("width", "100%");

    }

    function saveProduct() {
        if ($('#frmMaintainance').valid()) {
            var id = $('#hidIdM').val();
            var nameQuestion = $('#txtNameQuestion').val();
            var subjectId = $('#ddlCategorySubjectId').combotree('getValue');
            var chapterId = $('#ddlCategoryChapterId').combotree('getValue');

            var optionA = $('#txtOptionA').val();
            var optionB = $('#txtOptionB').val();
            var optionC = $('#txtOptionC').val();
            var optionD = $('#txtOptionD').val();

            var score = $('#txtScore').val();
            var answer = $('#txtAnswer').val();

            var status = $('#ckStatusM').prop('checked') == true ? 1 : 0;

            $.ajax({
                type: 'POST',
                url: '/Admin/Question/SaveEntity',
                dataType: 'JSON',
                data: {
                    Id: id,
                    QuestionName: nameQuestion,
                    Option1: optionA,
                    Option2: optionB,
                    Option3: optionC,
                    Option4: optionD,
                    Answer: answer,
                    ScoreQuestion: score,
                    SubjectId: subjectId,
                    ChapterId: chapterId,
                    Status: status
                },
                beforeSend: function () {
                    core.startLoading();
                },
                success: function (res) {

                    core.notify('Save or update success', 'success');

                    core.stopLoading();
                    $('#paginationUL').twbsPagination('destroy');
                    loadData();
                    resetFormMaintainance();
                    $('#modal-add-edit').modal('hide');

                },
                error: function (res) {
                    core.notify('Has a error in save product progress', 'error');
                    core.stopLoading();
                }
            });
            return false;
        }
    }

    function loadDetails(that) {

        $.ajax({
            type: "GET",
            url: "/Admin/Question/GetById",
            data: {id: that},
            dataType: "json",
            beforeSend: function () {
                core.startLoading();
            },
            success: function (res) {
                $('#hidIdM').val(res.Id);
                $('#txtNameQuestion').val(res.QuestionName);
                initTreeDropDownChapterCategory(res.ChapterId);
                initTreeDropDownSubjectCategory(res.SubjectId);

                $('#txtOptionA').val(res.Option1);
                $('#txtOptionB').val(res.Option2);
                $('#txtOptionC').val(res.Option3);
                $('#txtOptionD').val(res.Option4);

                $('#txtAnswer').val(res.Answer);

                $('#txtScore').val(res.ScoreQuestion);
                $('#ckStatusM').prop('checked', res.Status === 1);

                $('#modal-add-edit').modal('show');
                core.stopLoading();
                $(".combo").css("width", "100%");
                $(".textbox-text").css("width", "100%");

            },
            error: function (status) {
                core.notify('Có lỗi xảy ra', 'error');
                core.stopLoading();
            }
        });
    }

    function deleteItem(that) {
        core.confirm('Are you sure to delete?', function () {

            $.ajax({
                url: '/Admin/Question/Delete',
                type: 'POST',
                data: {
                    id: that
                },
                beforeSend: function () {
                    core.startLoading();
                },
                success: function () {
                    core.notify('Deleted success', 'success');
                    core.stopLoading();
                    $('#paginationUL').twbsPagination('destroy');
                    loadData();
                },
                error: function () {
                    core.notify('Deleted fail', 'error');
                    core.stopLoading();
                }
            });

        });
    }

    // function exportExcel() {
    //     $.ajax({
    //         type: "POST",
    //         url: "/Admin/Product/ExportExcel",
    //         beforeSend: function () {
    //             core.startLoading();
    //         },
    //         success: function (response) {
    //             window.location.href = response;
    //             core.stopLoading();
    //         },
    //         error: function () {
    //             core.notify('Has an error in progress', 'error');
    //             core.stopLoading();
    //         }
    //     });
    // }

    // function importExcel() {
    //     var fileUpload = $("#fileInputExcel").get(0);
    //     var files = fileUpload.files;
    //
    //     // Create FormData object  
    //     var fileData = new FormData();
    //     // Looping over all files and add it to FormData object  
    //     for (var i = 0; i < files.length; i++) {
    //         fileData.append("files", files[i]);
    //     }
    //     // Adding one more key to FormData object  
    //     fileData.append('categoryId', $('#ddlCategoryIdImportExcel').combotree('getValue'));
    //     $.ajax({
    //         url: '/Admin/Product/ImportExcel',
    //         type: 'POST',
    //         data: fileData,
    //         processData: false,  // tell jQuery not to process the data
    //         contentType: false,  // tell jQuery not to set contentType
    //         success: function (data) {
    //             $('#modal-import-excel').modal('hide');
    //             loadData();
    //         }
    //     });
    //     return false;
    //     $('#modal-import-excel').modal('hide');
    // }

};