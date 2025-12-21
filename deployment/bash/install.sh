#!/bin/bash

#EXISTS=0
#if test -f "/etc/systemd/system/beer-commander-dev.service"; then
#    echo "$FILE exists."
#    EXISTS=1
#fi
systemctl stop bithday-bot.service
systemd disable bithday-bot.service
systemctl daemon-reload
cp deploy/systemd/bithday-bot.service /etc/systemd/system/
systemd enable bithday-bot.service
systemctl daemon-reload
