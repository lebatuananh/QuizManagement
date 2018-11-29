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
## Cấu trúc Project
* Project QuizManagement.Infrastructure:
    * Chua cac kho phuong thuc dung chung cho ca he thong
    * Chua cac hang so, cac tien ich, cac dong mo ket noi voi database dung chung cho he thong
* Project QuizManagement.Data
    * Chua cac thuc the co trong he thong
* Project QuizManagement.DataEF
    * Chua lop ket noi database
    * Project nay dam nhien generate database bang entity framework
* Project QuizManagement.Application
    * Chua cac lop thuc hien chuc nang cua tung thuc the
    * Chua cac viewmodel du lieu de hien thi ra ben ngoai web 
* Project QuizManagement.WebApplicatiom
    * Chua cac API de thuc hien cac request cua nguoi dung gui den server de hien thi ket qua ra cho nguoi dung mong muon
    * Chua cac lop giao dien
### Project QuizManagement.Infrastructure