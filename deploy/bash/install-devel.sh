#!/bin/bash

#EXISTS=0
#if test -f "/etc/systemd/system/beer-commander-dev.service"; then
#    echo "$FILE exists."
#    EXISTS=1
#fi
systemctl stop birthday-bot-devel.service
systemd disable birthday-bot-devel.service
systemctl daemon-reload
cp deploy/systemd/birthday-bot-devel.service /etc/systemd/system/
systemd enable birthday-bot-devel.service
systemctl daemon-reload
