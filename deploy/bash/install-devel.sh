#!/bin/bash

#EXISTS=0
#if test -f "/etc/systemd/system/beer-commander-dev.service"; then
#    echo "$FILE exists."
#    EXISTS=1
#fi
systemctl stop bithday-bot-devel.service
systemd disable bithday-bot-devel.service
systemctl daemon-reload
cp deploy/systemd/bithday-bot-devel.service /etc/systemd/system/
systemd enable bithday-bot-devel.service
systemctl daemon-reload
