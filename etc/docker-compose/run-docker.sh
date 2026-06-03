#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p d096eb9c-687a-4f32-ba00-23607ce89868 -t
    fi
    cd ../
fi

docker-compose up -d
