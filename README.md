# Microservices with Service Fabric. Easy... or is it?

Service Fabric is so simple! Just click-click-deploy and we have a stateless microservice! But what if we want to store business data? We can use reliable collections and even transactions to store data inside the cluster, but what happens when our single partition goes down? We could lose everything! Now we need to partition our data… how do we do that? And how do we integrate with other PaaS offerings like Azure Service Bus? Maybe this isn’t so easy after all.

In this talk I'll walk you through Service Fabric Partitioning, a partition affinity model for Azure Service Bus queues and challenges you'll face with messaging patterns like request/response, publish/subscribe, process managers and other stateful entities running inside a Service Fabric Cluster.

Please read the [LICENSE.md](License) agreement

The font used in the slides is

[Kaffeesatz](https://www.yanone.de/fonts/kaffeesatz/)

# Links
## About me
* [Geeking out with Daniel Marbach]( http://developeronfire.com/episode-077-daniel-marbach-geeking-out)