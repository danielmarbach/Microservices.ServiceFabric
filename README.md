# Microservices with Service Fabric. Easy... or is it?

Service Fabric is so simple! Just click-click-deploy and we have a stateless microservice! But what if we want to store business data? We can use reliable collections and even transactions to store data inside the cluster, but what happens when our single partition goes down? We could lose everything! Now we need to partition our data… how do we do that? And how do we integrate with other PaaS offerings like Azure Service Bus? Maybe this isn’t so easy after all.

In this talk I'll walk you through Service Fabric Partitioning, a partition affinity model for Azure Service Bus queues and challenges you'll face with messaging patterns like request/response, publish/subscribe, process managers and other stateful entities running inside a Service Fabric Cluster.

Please read the [LICENSE.md](License) agreement

The font used in the slides is

[Kaffeesatz](https://www.yanone.de/fonts/kaffeesatz/)

# Links
## About me
* [Geeking out with Daniel Marbach]( http://developeronfire.com/episode-077-daniel-marbach-geeking-out)

## Service Fabric

* [Routing Network traffic to stateful services in Service Fabric](https://www.opsgility.com/blog/2016/11/02/routing-network-traffic-to-stateful-services-in-service-fabric/)
* [Service Fabric Reverse Proxy](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reverseproxy)
* [C3 LS HTTP based Routing library](https://github.com/c3-ls/ServiceFabric-Http)
* [Another HTTP based routing library by Khenidak](https://github.com/khenidak/Router)

## NServiceBus

* [Getting started](https://docs.particular.net/tutorials/intro-to-nservicebus/)
* [Service Fabric Routing])(https://docs.particular.net/samples/azure/azure-service-fabric-routing/)

## Orleans

* [Orleans: Distributed Virtual Actors for Programmability and Scalability
](https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/Orleans-MSR-TR-2014-41.pdf)