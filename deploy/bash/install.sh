#!/bin/bash

#EXISTS=0
#if test -f "/etc/systemd/system/beer-commander-dev.service"; then
#    echo "$FILE exists."
#    EXISTS=1
#fi
systemctl stop birthday-bot.service
systemd disable birthday-bot.service
systemctl daemon-reload
cp deploy/systemd/birthday-bot.service /etc/systemd/system/
systemd enable birthday-bot.service
systemctl daemon-reload
