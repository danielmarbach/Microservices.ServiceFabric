## Questions

 - Is it OK to assume basic Service Fabric knowledge? Fair assumption
 - Is it OK to assume basic ASB knowledge? Fair assumption
 - Basic messaging knowledge? Fair assumption

 ## Outline

 - Quick ramp up on Service Fabric
 - Stateful Services focus
 - What is partitioning and why would I care?
  - Uniform hashing (maybe only relevant for Actors)
  - Business data defines partitioning / sharding (might be tricky if you want to uniformly distribute the data, you don't want to end up with imbalances on the cluster)
  - Mention: Repartitioning is hard in SF
  - Why not a single partition: Memory consumption, Performance, loosing data?
  - Visualize how data is sticky to a given partition
 - Service Fabric patterns to access partitioned data from withing the cluster and outside the cluster
  - Within: use Service Partition resolver to get to a service on a given partition
  - Outside: Either use Reverse Proxy which is built in or expose stateless service which maps to stateful service
  - Transactionality does not work cross service
  - Service needs to expose the data in the reliable collection over the communication listener

 - Brokered queues such as RabbitMQ or Azure Service Bus, with endpoint communication listener and scaled/partitioned service you can do the following:
  - Clients can be inside the cluster or outside the cluster
  - Competing consumer on the same queue
   - Take a message out of the queue on any instance, see if key matches local partition, if not put it back into the queue
   - Worst case a lot of back and forth
   - No internal rerouting is efficiently possible because instanes are not uniquely addressable
  - Uniquely addressable queue per instance
  - Reliable Queues (mention can't be used really)
 - Some messaging patterns do not require stickyness. For example when you just want a command being handled by any available worker. Complexity increases when you a worker wants to reply back to the command server instance because it uses a process manager to orchestrate the work process.
 - Routing and rerouting into the cluster and within the cluster

