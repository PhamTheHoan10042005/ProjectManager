# Bước 1: Sử dụng SDK để Build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy các file project và phục hồi các gói thư viện (Restore)
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ code còn lại và Publish ra thư mục 'out'
COPY . ./
RUN dotnet publish -c Release -o out

# Bước 2: Tạo Image chạy gọn nhẹ (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Thiết lập cổng chạy cho ứng dụng (thường là 80 hoặc 8080)
EXPOSE 80
ENTRYPOINT ["dotnet", "CommentNotifyService.dll"]