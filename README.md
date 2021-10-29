brasscoin

the certificate will eventually expire, so when that happens you have to install dotnet and run

MAKING SURE YOUR WORKING DIRECTORY IS SOMETHING YOU HAVE FULL ACCESS TO (didn't work with c:/ for me)
dotnet dev-certs https -ep ./brasscoin.pfx -p hunter2

then copy that into the folder with the .exe, replacing the previous one
