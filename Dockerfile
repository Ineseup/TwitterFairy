FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

RUN apt-get update && apt-get install make

WORKDIR /

COPY ./ ./twitterFairy
WORKDIR /twitterFairy
RUN make

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS run

RUN apt-get update && apt-get install make

WORKDIR app-root/twitterFairy
COPY --from=build-env /twitterFairy/TwitterFairy/dist ./

ENV Logging__Console__FormatterName=

ENTRYPOINT ["dotnet", "TwitterFairy.dll"]