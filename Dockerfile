FROM debian:13

COPY Container/install-dotnet.sh /root
RUN chmod +x /root/install-dotnet.sh && /root/install-dotnet.sh
RUN mkdir -p /opt/bot
COPY Birthday-Bot-Sourcecode/Birthday-Bot-Sourcecode/bin/Release/net8.0 /opt/bot
CMD ["/usr/bin/dotnet","/opt/bot/Birthday-Bot-Sourcecode.dll"]