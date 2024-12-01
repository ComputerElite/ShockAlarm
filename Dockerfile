# Include dotnet6 sdk

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
RUN apt-get update && \
    apt-get install -y curl && \
    curl -sL https://deb.nodesource.com/setup_21.x | bash - && \
    apt-get install -y nodejs && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*
# Run the build frontend bash script in frontend
COPY . .
WORKDIR /app/ShockAlarm
RUN rm -rf bin/ obj/
WORKDIR /app/ShockAlarm/frontend
RUN bash ./build_frontend.sh
WORKDIR /app/ShockAlarm
RUN dotnet build
RUN mkdir -p bin/Debug/net8.0/
RUN mkdir -p bin/Release/net8.0/
RUN dotnet tool install --global --version 9.0.0 dotnet-ef
RUN cp /app/ShockAlarm/docker_config.json /app/ShockAlarm/bin/Debug/net8.0/config.json
RUN cp /app/ShockAlarm/docker_config.json /app/ShockAlarm/bin/Release/net8.0/config.json
RUN cp /app/ShockAlarm/docker_config.json /app/ShockAlarm/config.json
WORKDIR /app/ShockAlarm
CMD ["bash", "migrate_db_and_start.sh"]