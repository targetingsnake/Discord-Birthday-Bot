#!/bin/bash

if [ "$(dpkg -l | awk '/microsoft/ {print }'|wc -l)" -ge 1 ]; then
  exit 0
else
  wget https://packages.microsoft.com/config/debian/13/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
fi

apt update
apt install -y dotnet-runtime-8.0
apt clean