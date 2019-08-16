<a href="https://github.com/Lutando/Entropy/"><img src="https://raw.githubusercontent.com/Lutando/Entropy/master/LaundryBooker/logo.svg?sanitize=true" width="100%" height="200"></a>

# Mr White's Laundry Booking System

This is a booking system for individuals who just want to wash their clothing in peace.

# Stack
ASPNET Core
ReactJS
Docker
Kubernetes (runs on minikube)
Postgres

The api (`LaundryBooker.Api`) is a normal aspnet core 2.2 web api that uses modern rest idioms. It is reliant on the security token server (`LaundryBooker.Sts`) which is also a webapi but its duty is to issue tokens to clients, it is powered by IdentityServer4. The client application (`LaundryBooker.Frontend`) is a react application created using created react app. it also used `react-big-calendar` to do its main duty.

# Run It

You can run the project either by using minikube, or by going barebones, but you need minikube to run postgres for this example :)

## Minikube
install minikube and then go into the `deploy` folder and run `kubectl apply -f /k8s``

navigate to `http://localhost:30505/`

## Barebones
you need .NET SDK version 2.2. To run the backend systems you need postgres running. You can run it on minikube by going to the `deploy/k8s` folder and running the `kubectl apply -f laundry-db.yaml` command which will start a postgres deployment in k8s.

To run both the backend systems, go into the `src/LaundryBooker.Api` folder and run the projects; `LaundryBooker.Api`, and `LaundryBooker.Sts` using `dotnet run` in their respective folders

To run the frontend, go into the `src/LaundryBooker.Frontend` folder and run `npm install` and then a `npm run start` your browser should navigate itself to `http://localhost:3000`

