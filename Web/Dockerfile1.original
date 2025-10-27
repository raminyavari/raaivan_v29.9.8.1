#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat 

FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2019
ARG source
WORKDIR /inetpub/wwwroot
COPY ${source:-obj/Docker/publish} .


WORKDIR /install

ADD http://kmacademy.ir/rewrite_amd64.msi rewrite_amd64.msi
RUN Write-Host 'Installing URL Rewrite' ; \
    Start-Process msiexec.exe -ArgumentList '/i', 'rewrite_amd64.msi', '/quiet', '/norestart' -NoNewWindow -Wait

ADD http://kmacademy.ir/gs9533w64.exe gs9533w64.exe
RUN Write-Host 'Installing Ghost Script' ; \
    Start-Process msiexec.exe -ArgumentList '/i', 'gs9533w64.exe', '/quiet', '/norestart' -NoNewWindow -Wait
