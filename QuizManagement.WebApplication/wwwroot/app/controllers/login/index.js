var LoginController = function () {
    this.initialize = function () {
        registerEvents();
    };

    var registerEvents = function () {

        $('#frmLogin').validate({
            errorClass: 'red',
            ignore: [],
            rules: {
                username: {
                    required: true
                },
                password: {
                    required: true
                }
            },
            messages: {
                username: {
                    required: 'Username is required'
                },
                password: {
                    required: 'Password is required'
                }
            }
        });

        $('#btnLogin').on('click', function (e) {
            if ($('#frmLogin').valid()) {
                e.preventDefault();
                var username = $('#txtUsername').val();
                var password = $('#txtPassword').val();
                login(username, password);
            }
        })

    }

    var login = function (username, password) {

        $.ajax({
            type: 'POST',
            data: {
                Username: username,
                Password: password
            },
            dataType: 'JSON',
            url: '/Admin/Login/Authen',
            success: function (res) {
                if (res.Success)
                    window.location.href = core.getParamUrl('ReturnUrl') ||'/Admin/Home/Index';
                else
                    core.notify('Đăng nhập không thành công', 'error');
            }
        });
    }
}