## Questions

 - Is it OK to assume basic Service Fabric knowledge?
 - Is it OK to assume basic ASB knowledge?

 ## Outline

 - Quick ramp up on Service Fabric
 - Stateful Services focus
 - What is partitioning and why would I care?
  - Uniform hashing
  - Business data defines partitioning / sharding
  - Visualize how data is sticky to a given partition
 - Service Fabric patterns to access partitioned data from withing the cluster and outside the cluster
  - Within: use Service Partition resolver to get to a service on a given partition
  - Outside: Either use Reverse Proxy which is built in or expose stateless service which maps to statefu service
 - Brokered queues such as RabbitMQ or Azure Service Bus, with endpoint communication listener and scaled/partitioned service you can do the following:
  - Clients can be inside the cluster or outside the cluster
  - Competing consumer on the same queue
   - Take a message out of the queue on any instance, see if key matches local partition, if not put it back into the queue
   - Worst case a lot of back and forth
   - No internal rerouting is efficiently possible because instanes are not uniquely addressable
  - Uniquely addressable queue per instance
 - Some messaging patterns do not require stickyness. For example when you just want a command being handled by any available worker.Complexity increases when you a worker wants to reply back to the command server instance because it uses a process manager to orchestrate the work process.
