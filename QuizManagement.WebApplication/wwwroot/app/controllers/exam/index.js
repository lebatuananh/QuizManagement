var ExamController = function () {

    var cachedObj = {
        questions: []
    };

    this.initialize = function () {
        $.when(
            loadQuestions())
            .done(function () {
                loadData();
            });
        registerEvents();
    };

    function registerEvents() {

        //Init validation
        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            rules: {
                txtCustomerName: {required: true},
                txtCustomerAddress: {required: true},
                txtCustomerMobile: {required: true},
                txtCustomerMessage: {required: true},
                ddlBillStatus: {required: true}
            }
        });

        $('#txtSearchKeyword').keypress(function (e) {
            if (e.which === 13) {
                core.configs.pageIndex = 1;
                loadData();
            }
        });

        $("#btn-search").on('click', function () {
            core.configs.pageIndex = 1;
            loadData();
        });

        $("#btn-create").on('click', function () {
            resetFormMaintainance();
            $('#modal-detail').modal('show');
        });

        $("#ddl-show-page").on('change', function () {
            core.configs.pageSize = $(this).val();
            core.configs.pageIndex = 1;
            loadData(true);
        });

        $('body').on('click', '.btn-view', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $.ajax({
                type: "GET",
                url: "/Admin/Exam/GetById",
                data: {id: that},
                beforeSend: function () {
                    core.startLoading();
                },
                success: function (response) {
                    var data = response;
                    $('#hidId').val(data.Id);
                    $('#txtExamName').val(data.ExamName);
                    $('#txtTime').val(data.Time);
                    $('#txtExaminer').val(data.Examiner);
                    $('#ckStatusM').prop('checked', data.Status === 1);

                    var questionDetails = data.QuestionExamDetailViewModels;
                    if (data.QuestionExamDetailViewModels != null && data.QuestionExamDetailViewModels.length > 0) {
                        var render = '';
                        var templateDetails = $('#template-table-bill-details').html();

                        $.each(questionDetails, function (i, item) {

                            var questions = getQuestionOptions(item.QuestionId);

                            render += Mustache.render(templateDetails,
                                {
                                    Id: item.Id,
                                    Questions: questions
                                });
                        });
                        $('#tbl-bill-details').html(render);
                    } else {
                        $('#tbl-bill-details').html('');
                    }
                    $('#modal-detail').modal('show');
                    core.stopLoading();

                },
                error: function (e) {
                    core.notify('Has an error in progress', 'error');
                    core.stopLoading();
                }
            });
        });

        $('#btnSave').on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                e.preventDefault();
                var id = $('#hidId').val();
                var examName = $('#txtExamName').val();
                var time = $('#txtTime').val();
                var examiner = $('#txtExaminer').val();
                var status = $('#ckStatusM').prop('checked') === true ? 1 : 0;
                //bill detail

                var questionDetails = [];
                $.each($('#tbl-bill-details tr'), function (i, item) {
                    questionDetails.push({
                        Id: $(item).data('id'),
                        QuestionId: $(item).find('select.ddlProductId').first().val(),
                        ExamId: id
                    });
                });

                $.ajax({
                    type: "POST",
                    url: "/Admin/Exam/SaveEntity",
                    data: {
                        Id: id,
                        ExamName: examName,
                        Time: time,
                        Examiner: examiner,
                        Status: status,
                        QuestionExamDetailViewModels: questionDetails
                    },
                    dataType: "json",
                    beforeSend: function () {
                        core.startLoading();
                    },
                    success: function (response) {
                        core.notify('Save order successful', 'success');
                        $('#modal-detail').modal('hide');
                        resetFormMaintainance();

                        core.stopLoading();
                        loadData(true);
                    },
                    error: function () {
                        core.notify('Has an error in progress', 'error');
                        core.stopLoading();
                    }
                });
                return false;
            }

        });

        $('#btnAddDetail').on('click', function () {
            var template = $('#template-table-bill-details').html();
            var questions = getQuestionOptions(null);
            var render = Mustache.render(template,
                {
                    Id: 0,
                    Questions: questions
                });
            $('#tbl-bill-details').append(render);
        });

        $('body').on('click', '.btn-delete-detail', function () {
            $(this).parent().parent().remove();
        });

    }


    function loadQuestions() {
        return $.ajax({
            type: "GET",
            url: "/Admin/Question/GetAll",
            dataType: "json",
            success: function (response) {
                cachedObj.questions = response;
            },
            error: function () {
                core.notify('Has an error in progress', 'error');
            }
        });
    }

    function getQuestionOptions(selectedId) {
        var questions = "<select class='form-control ddlProductId'>";
        $.each(cachedObj.questions, function (i, question) {
            if (selectedId === question.Id)
                questions += '<option value="' + question.Id + '" selected="select">' + question.QuestionName + '</option>';
            else
                questions += '<option value="' + question.Id + '">' + question.QuestionName + '</option>';
        });
        questions += "</select>";
        return questions;
    }

    function resetFormMaintainance() {
        $('#hidId').val(0);
        $('#txtExamName').val('');
        $('#ckStatusM').prop('checked', true);

        $('#txtTime').val('');
        $('#txtExaminer').val('');
        $('#tbl-bill-details').html('');
    }

    function loadData(isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/exam/GetAllPaging",
            data: {
                keyword: $('#txtSearchKeyword').val(),
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
                            ExamName: item.ExamName,
                            Id: item.Id,
                            Time: item.Time,
                            Examiner: item.Examiner,
                            Status: core.getStatus(item.Status),
                            DateCreated: core.dateFormatSubStr(item.DateCreated),
                        });
                    });
                    $("#lbl-total-records").text(response.RowCount);
                    if (render != undefined) {
                        $('#tbl-content').html(render);

                    }
                    wrapPaging(response.RowCount, function () {
                        loadData();
                    }, isPageChanged);


                } else {
                    $("#lbl-total-records").text('0');
                    $('#tbl-content').html('');
                }
                core.stopLoading();
            },
            error: function (status) {
                console.log(status);
            }
        });
    };


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
};