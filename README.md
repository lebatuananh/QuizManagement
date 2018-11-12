# QuizManagement
## Hướng dẫn chạy Project
* Tải Project về máy bằng lênh:
```sh
git clone https://github.com/mramra3004/QuizManagement.git
```
* Sau đó khởi chạy Project bằng visual studio
* Để tạo database vào file appsettings.json trong project QuizManagement.WebApplication sửa lại tên server SQLServer
* Click chuột phải vào project QuizManagement.WebApplication chọn Set as StartUp Project.
* Trên thanh công cụ chọn Tools -> NuGet Package Manager -> Package Manager Console 
sau đó chọn default project là QuizManagement.DataEF rồi nhập lệnh
```sh
Update-Database
```
* Vào SQL Server để tìm kiếm database