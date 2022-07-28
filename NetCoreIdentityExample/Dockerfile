#FROM mcr.microsoft.com/dotnet/aspnet as DockerAspNet
FROM mcr.microsoft.com/dotnet/sdk:5.0 as DockerAspNetSdk
WORKDIR /App
#COPY /bin/Release/net5.0/publish /App/
COPY *.csproj .
RUN dotnet restore
#Tüm CSPROJ dosyalarını kopyalayıp tekrar restore etmek dosya değişirken tekrar cache'lemek anlamına gelir.

COPY . .
#Birinci . dockerfile'nin bulunduğu konum, ikinci nokta image'deki workdir'in konumunu ifade eder (App)
RUN dotnet publish NetCoreIdentityExample.csproj -c Release -o out
#-o komutu output klasörü anlamına gelir ve out isimli bir klasöre publish edecek

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /App
COPY --from=DockerAspNetSdk /App/out .
#Copy komutu üstteki sdk imajında oluşan publish edilen klasörleri kopyalar

ENV ASPNETCORE_URLS http://*:4500
#Container hangi ip'den çalışırsa çalışsın 4500 portu üzerinden ayağa kalkacak

ENTRYPOINT ["dotnet","NetCoreIdentityExample.dll"]

#docker run -p 5000:80 netcoreimage:v1 komutu bu imajın localhost:5000 portu üzerinden çalışmasını sağlar. Default olarak 80 portu verildiği için 80'i 5000'e çeviriyoruz
#docker run -d -p 5000:80 netcoreimage:v1 komutu bu imajın localhost:5000 portu üzerinden çalışmasını sağlar. Default olarak 80 portu verildiği için 80'i 5000'e çeviriyoruz. -d komutu detach anlamına gelir
#docker run -d --name netcorecontainer -p 5000:80 netcoreimage:v1 imaja netcorecontainer ismi verdik
