#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p f25d87e2-b70d-458f-b4d9-16edc40d1751 -t
    fi
    cd ../
fi

docker-compose up -d
